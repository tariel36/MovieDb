using Microsoft.Extensions.Configuration;
using MovieDbApi.Common.Domain.Apis;
using MovieDbApi.Common.Domain.Apis.Abstract;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Crawling.Services;
using MovieDbApi.Common.Domain.Files;
using MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Abstract;
using MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Models;
using MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Specific;
using MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Abstract;
using MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Models;
using MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Specific;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Monitoring;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Media.Services.Specific
{
    public class MediaMonitor
        : IMediaMonitor
    {
        private readonly ILoggerService _logger;
        private readonly IMediaService _mediaService;

        public MediaMonitor(ILoggerService logger,
            IConfiguration configuration,
            IMediaService mediaContextService)
        {
            _logger = logger;
            _mediaService = mediaContextService;

            Apis = new MediaApiFactory().GetAllApis(_logger, configuration);
            InstructionsProvider = new FileInstructionsProvider();
            
            MediaLanguageResolvers = new List<IMediaLanguageResolver>()
            {
                new NutaMediaLanguageResolver()
            };

            MediaTypeResolverCollection = new List<IMediaTypeResolver>()
            {
                new NutaMediaTypeResolver()
            };
        }

        private List<IMediaDataProvider> Apis { get; }

        private FileInstructionsProvider InstructionsProvider { get; }

        private List<IMediaLanguageResolver> MediaLanguageResolvers { get; }

        private List<IMediaTypeResolver> MediaTypeResolverCollection { get; }

        public void Work()
        {
            Dictionary<string, MediaItem> existingFiles = _mediaService.MediaItems.ToDictionary(k => k.Path);

            MediaCrawlerService crawler = new MediaCrawlerService(_logger, _mediaService);

            Dictionary<string, List<MediaIntermediateItem>> files = _mediaService.ScannedPaths
                .Select(x => x.Path)
                .ToList() // ToList closes db connection so it can be reused later on
                .Select<string, (string path, List<MediaIntermediateItem> items)>(x => (x, crawler.Crawl(new MediaCrawlContext() { Path = x })))
                .ToDictionary(k => k.path, v => v.items);

            RemoveOldItems(existingFiles, files);
            FindNewItems(existingFiles, files);
        }

        private void RemoveOldItems(Dictionary<string, MediaItem> existingFiles, Dictionary<string, List<MediaIntermediateItem>> files)
        {
            Dictionary<string, MediaIntermediateItem> itemsDict = files.SelectMany(x => x.Value).ToDictionary(k => k.FilePath);
            
            Dictionary<int, List<MediaItem>> groupsDict = existingFiles.Where(x => x.Value.GroupId.HasValue)
                .GroupBy(x => x.Value.GroupId.Value)
                .ToDictionary(k => k.Key, v => v.Select(x => x.Value).ToList());

            foreach (KeyValuePair<string, MediaItem> kvExistingFile in existingFiles.ToList())
            {
                MediaItem item = kvExistingFile.Value;

                // If item is not grouping and no longer exis on hard drive
                if (!item.IsGrouping && !itemsDict.ContainsKey(kvExistingFile.Key))
                {
                    // Then delete the item from db
                    _mediaService.Delete(item);

                    existingFiles.Remove(kvExistingFile.Key);

                    // If item belonged to group
                    if (item.GroupId.HasValue)
                    {
                        // Then remove it from group in memory
                        groupsDict[item.GroupId.Value].Remove(item);
                        
                        // If inmemory group contains only grouping items
                        // then most probably the group is empty
                        // so remove the group too.
                        if (groupsDict[item.GroupId.Value].All(x => x.IsGrouping))
                        {
                            groupsDict[item.GroupId.Value].ForEach(x => _mediaService.Delete(x));
                        }
                    }
                }
            }
        }

        private void FindNewItems(Dictionary<string, MediaItem> existingFiles, Dictionary<string, List<MediaIntermediateItem>> files)
        {
            List<MediaMonitorIntermediateMediaItem> result = new List<MediaMonitorIntermediateMediaItem>();

            foreach (KeyValuePair<string, List<MediaIntermediateItem>> kvPair in files)
            {
                foreach (IGrouping<string, MediaIntermediateItem> grouping in kvPair.Value.GroupBy(x => x.Group))
                {
                    List<MediaIntermediateItem> itemsToProcess = GetItemsToProcess(grouping);

                    foreach (MediaIntermediateItem itemToProcess in itemsToProcess)
                    {
                        if (existingFiles.ContainsKey(itemToProcess.FilePath))
                        {
                            continue;
                        }

                        MediaTypeResolverContext mediaTypeResolverCtx = new MediaTypeResolverContext()
                        {
                            Path = itemToProcess.FilePath
                        };

                        MediaItemType type = MediaTypeResolverCollection.Select(x => x.Resolve(mediaTypeResolverCtx)).FirstOrDefault(x => x != MediaItemType.Unknown);

                        int groupCount = grouping.Count();

                        foreach (IMediaDataProvider api in Apis.Where(x => type == MediaItemType.Unknown || x.IsSupported(type)))
                        {
                            if (itemsToProcess.Count > 1)
                            {
                                string title = ClearTitle(Path.GetFileName(Path.GetDirectoryName(itemToProcess.FilePath)));

                                ApiMediaItemDetails searchResult = api.GetByUrl(itemToProcess.Url)
                                    ?? api.SearchDetailsByTitle(title)
                                    ;

                                if (searchResult != null)
                                {
                                    result.Add(ToMediaMonitorIntermediateMediaItem(1, searchResult, 1, itemToProcess));

                                    break;
                                }
                            }
                            else
                            {
                                string title = ClearTitle(Path.GetFileName(grouping.Key));

                                ApiMediaItemDetails searchResult = api.GetByUrl(itemToProcess.Url)
                                    ?? api.SearchDetailsByTitle(title)
                                    ;

                                if (searchResult != null)
                                {
                                    int currentChapter = 1;

                                    foreach (MediaIntermediateItem item in grouping)
                                    {
                                        result.Add(ToMediaMonitorIntermediateMediaItem(groupCount, searchResult, currentChapter, item));
                                        currentChapter++;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (IGrouping<string, MediaMonitorIntermediateMediaItem> grouping in result.GroupBy(x => x.Group))
            {
                bool isGroup = grouping.Count() > 1;
                int? groupId = null;

                if (isGroup)
                {
                    MediaMonitorIntermediateMediaItem item = grouping.First();
                    MediaItem mediaItem = ToMediaItem(item, isGroup, null, true);

                    _mediaService.SaveMediaItem(mediaItem);
                    groupId = mediaItem.Id;

                    if (mediaItem.Id == 0)
                    {
                        throw new InvalidOperationException("Saved media item but id is 0");
                    }
                }

                foreach (MediaMonitorIntermediateMediaItem item in grouping)
                {
                    _mediaService.SaveMediaItem(ToMediaItem(item, false, groupId));
                }
            }
        }

        private MediaMonitorIntermediateMediaItem ToMediaMonitorIntermediateMediaItem(int groupCount,
            ApiMediaItemDetails searchResult,
            int currentChapter,
            MediaIntermediateItem item)
        {
            MediaLanguageResolverContext mediaLanguageResolverCtx = new MediaLanguageResolverContext()
            {
                Path = item.FilePath
            };

            string selectedPoster = Extensions.GetNonEmpty(searchResult.Poster, item.MainImage, item.Images);

            return new MediaMonitorIntermediateMediaItem()
            {
                ApiSource = searchResult.ApiSource,
                // This is naive numbering, because by default, the results may not be sorted properly.
                ChapterTitle = groupCount == 1 ? searchResult.Title : $"{searchResult.Title} - {currentChapter}/{groupCount}",
                Directory = item.Directory,
                Duration = searchResult.Duration,
                DurationPerEpisode = searchResult.DurationPerEpisode,
                ExternalId = searchResult.ExternalId,
                Genre = searchResult.Genre,
                GroupCount = groupCount.ToString(),
                GroupCustomCover = item.GroupCustomCover,
                FilePath = item.FilePath,
                Group = item.Group,
                Images = (item.Images ?? new List<string>()).Concat(new[] { searchResult.Poster, item.MainImage }).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList(),
                MainImage = item.MainImage,
                Plot = searchResult.Plot,
                Rated = searchResult.Rated,
                Rating = searchResult.Rating,
                ReleaseDate = searchResult.ReleaseDate,
                SelectedPoster = selectedPoster,
                Staff = searchResult.Staff,
                Title = searchResult.Title,
                Titles = searchResult.Titles,
                Type = searchResult.Type,
                WebPoster = searchResult.Poster,
                Url = searchResult.Url,
                Year = searchResult.Year,
                MediaType = searchResult.MediaType,
                FileType = item.Type,
                Links = searchResult.Links,
                DirectoryOrder = item.DirectoryOrder ?? string.Empty,
                CustomTitle = item.CustomTitle,
                MediaLanguages = MediaLanguageResolvers.SelectMany(x => x.Resolve(mediaLanguageResolverCtx)).ToList()
            };
        }

        private List<MediaIntermediateItem> GetItemsToProcess(IGrouping<string, MediaIntermediateItem> grouping)
        {
            MediaIntermediateItem firstItem = grouping.First();

            return firstItem.Type == MediaType.Franchise
                ? grouping.ToList()
                : new List<MediaIntermediateItem>() { firstItem }
                ;
        }

        private MediaItem ToMediaItem(MediaMonitorIntermediateMediaItem item, bool isGroup, int? groupId = null, bool useDirectoryForGroupName = false)
        {
            ICollection<MediaItemLink> links = (string.IsNullOrWhiteSpace(item.Url)
                ? new List<MediaItemLink>()
                : new List<MediaItemLink>() { new MediaItemLink(item.Url) })
                .Concat((item.Links ?? new List<string>()).Select(x => new MediaItemLink(item.Url)))
                .ToList()
                ;

            ICollection<MediaItemImage> images = item.Images.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new MediaItemImage(x)).ToList();
            ICollection<MediaItemTitle> titles = (item.Titles ?? new List<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new MediaItemTitle(x)).ToList();
            ICollection<MediaItemAttribute> attributes = new List<MediaItemAttribute>()
            {
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.ApiSource), item.ApiSource),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Duration), item.Duration),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.DurationPerEpisode), item.DurationPerEpisode),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Genre), item.Genre),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.GroupCount), item.GroupCount),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Rated), item.Rated),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Rating), item.Rating),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.ReleaseDate), item.ReleaseDate),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Staff), item.Staff),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Type), item.Type),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.Year), item.Year),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.MediaType), item.MediaType.ToString()),
                new MediaItemAttribute(nameof(MediaMonitorIntermediateMediaItem.FileType), item.FileType.ToString()),
            };

            string title = ClearTitle(useDirectoryForGroupName ? Path.GetFileName(item.Directory) : (item.CustomTitle ?? item.Title));
            string chapterTitle = ClearTitle(useDirectoryForGroupName ? Path.GetFileName(item.Directory) : (item.CustomTitle ?? item.ChapterTitle));

            MediaItem mediaItem = new MediaItem()
            {
                Title = title,
                ChapterTitle = chapterTitle,
                Description = item.Plot,
                Image = new MediaItemImage((isGroup ? item.GroupCustomCover : item.SelectedPoster) ?? item.SelectedPoster),
                Path = isGroup ? item.Directory : item.FilePath,
                IsGrouping = isGroup,
                DateAdded = DateTime.UtcNow,
                Instructions = InstructionsProvider.Get(item.Directory, item.FilePath),
                ExternalId = item.ExternalId,
                Attributes = attributes.Where(x => !string.IsNullOrWhiteSpace(x.Value)).ToList(),
                Images = images,
                Languages = item.MediaLanguages.Where(x => !string.IsNullOrWhiteSpace(x.Language)).ToList(),
                Links = links.Where(x => !string.IsNullOrWhiteSpace(x.Link)).ToList(),
                Titles = titles,
                Group = item.Group,
                GroupId = groupId,
                Type = item.MediaType,
                DirectoryOrder = item.DirectoryOrder
            };

            return mediaItem;
        }

        private string ClearTitle(string title)
        {
            return (title ?? string.Empty)
                .Replace(CommonRegex.OrderedTitleRegex, string.Empty)
                .Trim()
                ;
        }

        private ApiMediaItemDetails GetDefault(string title, MediaIntermediateItem item)
        {
            return new ApiMediaItemDetails()
            {
                ApiSource = "Default",
                Duration = string.Empty,
                DurationPerEpisode = string.Empty,
                ExternalId = string.Empty,
                Genre = string.Empty,
                Plot = string.Empty,
                Poster = item.MainImage ?? item.Images?.FirstOrDefault(),
                Rated = string.Empty,
                Rating = string.Empty,
                ReleaseDate = string.Empty,
                Staff = string.Empty,
                Title = title,
                Titles = new List<string>(),
                Type = string.Empty,
                Url = string.Empty,
                Year = string.Empty,
                MediaType = MediaItemType.Unknown,
            };
        }
    }
}
