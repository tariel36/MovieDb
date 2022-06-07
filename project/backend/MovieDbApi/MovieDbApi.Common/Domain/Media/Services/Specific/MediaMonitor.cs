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

namespace MovieDbApi.Common.Domain.Media.Services.Specific
{
    public class MediaMonitor
        : IMediaMonitor
    {
        private readonly IMediaService _mediaService;

        public MediaMonitor(IConfiguration configuration,
            IMediaService mediaContextService)
        {
            _mediaService = mediaContextService;

            Apis = new MediaApiFactory().GetAllApis(configuration);
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
            HashSet<string> existingFiles = _mediaService.MediaItems.ToHashSet(x => x.Path);

            List<MediaMonitorIntermediateMediaItem> result = new List<MediaMonitorIntermediateMediaItem>();

            MediaCrawlerService crawler = new MediaCrawlerService();

            foreach (string rootPath in _mediaService.ScannedPaths.Select(x => x.Path))
            {
                MediaCrawlContext ctx = new MediaCrawlContext() { Path = rootPath };

                List<MediaIntermediateItem> sortedItems = crawler.Crawl(ctx).ToList();

                foreach (IGrouping<string, MediaIntermediateItem> grouping in sortedItems.GroupBy(x => x.Group))
                {
                    List<MediaIntermediateItem> itemsToProcess = GetItemsToProcess(grouping);

                    foreach (MediaIntermediateItem itemToProcess in itemsToProcess)
                    {
                        if (existingFiles.Contains(itemToProcess.FilePath))
                        {
                            continue;
                        }

                        MediaTypeResolverContext mediaTypeResolverCtx = new MediaTypeResolverContext()
                        {
                            Path = itemToProcess.FilePath
                        };

                        MediaItemType type = MediaTypeResolverCollection.Select(x => x.Resolve(mediaTypeResolverCtx)).FirstOrDefault(x => x != MediaItemType.Unknown);

                        int groupCount = grouping.Count();

                        foreach (IMediaDataProvider? api in Apis.Where(x => type == MediaItemType.Unknown || x.IsSupported(type)))
                        {
                            if (itemsToProcess.Count > 1)
                            {
                                string title = ClearTitle(Path.GetFileName(Path.GetDirectoryName(itemToProcess.FilePath)));

                                // TODO we can scan for readme files and attach the links/ids from that place to reduce queries
                                ApiMediaItemDetails searchResult = api.SearchDetailsByTitle(title) ?? GetDefault(title, itemToProcess);

                                if (searchResult != null)
                                {
                                    result.Add(ToMediaMonitorIntermediateMediaItem(1, searchResult, 1, itemToProcess));

                                    break;
                                }
                            }
                            else
                            {
                                string title = ClearTitle(Path.GetFileName(grouping.Key));

                                // TODO we can scan for readme files and attach the links/ids from that place to reduce queries
                                ApiMediaItemDetails searchResult = api.SearchDetailsByTitle(title) ?? GetDefault(title, grouping.First());

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
                    MediaItem mediaItem = ToMediaItem(item, isGroup);

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
                FilePath = item.FilePath,
                Group = item.Group,
                Images = (item.Images ?? new List<string>()).Concat(new[] { searchResult.Poster, item.MainImage }).Where(x => !string.IsNullOrWhiteSpace(x)).ToList(),
                MainImage = item.MainImage,
                Plot = searchResult.Plot,
                Rated = searchResult.Rated,
                Rating = searchResult.Rating,
                ReleaseDate = searchResult.ReleaseDate,
                SelectedPoster = Extensions.GetNonEmpty(searchResult.Poster, item.MainImage, item.Images),
                Staff = searchResult.Staff,
                Title = searchResult.Title,
                Titles = searchResult.Titles,
                Type = searchResult.Type,
                WebPoster = searchResult.Poster,
                Url = searchResult.Url,
                Year = searchResult.Year,
                MediaType = searchResult.MediaType,
                FileType = item.Type,
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

        private MediaItem ToMediaItem(MediaMonitorIntermediateMediaItem item, bool isGroup, int? groupId = null)
        {
            ICollection<MediaItemImage> images = item.Images.Select(x => new MediaItemImage(x)).ToList();
            ICollection<MediaItemLink> links = string.IsNullOrWhiteSpace(item.Url) ? null : new List<MediaItemLink>() { new MediaItemLink(item.Url) };
            ICollection<MediaItemTitle> titles = (item.Titles ?? new List<string>()).Select(x => new MediaItemTitle(x)).ToList();
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

            MediaItem mediaItem = new MediaItem()
            {
                Title = item.Title,
                ChapterTitle = item.ChapterTitle,
                Description = item.Plot,
                Image = item.SelectedPoster,
                Path = isGroup ? item.Directory : item.FilePath,
                IsGrouping = isGroup,
                DateAdded = DateTime.UtcNow,
                Instructions = InstructionsProvider.Get(item.Directory, item.FilePath),
                ExternalId = item.ExternalId,
                Attributes = attributes,
                Images = images,
                Languages = item.MediaLanguages,
                Links = links,
                Titles = titles,
                Group = item.Group,
                GroupId = groupId,
                Type = item.MediaType,
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
