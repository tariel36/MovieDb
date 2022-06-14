using System.Text.RegularExpressions;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Files;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Domain.Files.Decoders.NutaReadMe;
using MovieDbApi.Common.Domain.Files.Decoders.NutaReadMe.Models;
using MovieDbApi.Common.Domain.Media.Services.Abstract;

namespace MovieDbApi.Common.Domain.Crawling.Services
{
    public class MediaCrawlerService
    {
        private readonly Regex OrderedDirectoryRegex = new Regex("^[0-9]+\\.", RegexOptions.Compiled);

        public MediaCrawlerService(IMediaService mediaContextService)
        {
            MediaContextService = mediaContextService;

            IgnoredPaths = new HashSet<string>();

            CrawlingContextProvider = (ctx) =>
            {
                InnerCrawl(ctx);
                return ctx;
            };
        }

        private Func<MediaCrawlContext, MediaCrawlContext> CrawlingContextProvider { get; }

        private HashSet<string> IgnoredPaths { get; }

        private IMediaService MediaContextService { get; }

        private Regex IgnoredPathKeywords { get; set; }

        public List<MediaIntermediateItem> Crawl(MediaCrawlContext ctx)
        {
            IgnoredPaths.Clear();

            IgnoredPathKeywords = new Regex("(\\\\|\\/)([0-9]+([a-zA-Z]+)?\\.[ ]*)?((" + string.Join("|", MediaContextService.IgnoredPaths.Select(x => x.Path)) + "))(\\\\|\\/)?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            // We have some kind of tree structure here.
            // Now we have to squash that to the topmost level,
            // so the single movies and seasons are the only deeper
            // items.
            ctx = CrawlingContextProvider(ctx);

            Console.WriteLine("Finished crawling");

            List<MediaIntermediateItem> result = new List<MediaIntermediateItem>();

            Dictionary<string, (string directory, List<MediaIntermediateItem> items)> groups = new Dictionary<string, (string directory, List<MediaIntermediateItem> items)>();

            Dictionary<string, (string directory, List<MediaCrawlerItem> items)>  potentialGroups = GetPotentialGroups(ctx);

            if (potentialGroups?.Count > 0)
            {
                foreach (KeyValuePair<string, (string directory, List<MediaCrawlerItem> items)> group in potentialGroups)
                {
                    foreach (MediaCrawlerItem item in group.Value.items)
                    {
                        ctx.Items.Remove(item);
                    }

                    List<string> images = group.Value
                        .items
                        .Where(x => x.Images != null)
                        .SelectMany(x => x.Images)
                        .Distinct()
                        .ToList();

                    string mainImage = Directory.EnumerateFiles(group.Value.directory).FirstOrDefault(FileExtensions.IsImage);
                    bool hasCustomCover = !string.IsNullOrWhiteSpace(mainImage);
                    mainImage ??= images.FirstOrDefault();

                    ctx.Items.Add(new MediaCrawlerItem()
                    {
                        Directory = group.Value.directory,
                        Groups = null,
                        Images = images,
                        IsGrouping = true,
                        MainImage = mainImage,
                        Type = MediaType.Franchise,
                        Videos = group.Value.items.SelectMany(x => x.Videos).Distinct().ToList(),
                        HasCustomCover = hasCustomCover
                    });
                }
            }

            foreach (MediaCrawlerItem item in ctx.Items)
            {
                if (item.Groups?.Count > 0)
                {
                    throw new InvalidOperationException($"Failed to properly flatten the structure for path `{item.Directory}`");
                }

                string group = Path.GetFileName(item.Directory);

                if (!string.IsNullOrWhiteSpace(group) && groups.ContainsKey(group))
                {
                    group = ExtendGroup(groups, item, group);
                }

                List<MediaIntermediateItem> groupItems = new List<MediaIntermediateItem>();
                groups.Add(group, (item.Directory, groupItems));

                string localReadMeFilePath = FindReadMeFile(item.Directory);
                NutaReadMeFile readMeFile = new NutaReadMeDecoder().Deserialize(localReadMeFilePath);

                foreach (string vidPath in item.Videos)
                {
                    string url = null;

                    if (string.IsNullOrWhiteSpace(url) && item.Videos.Count == 1 && !string.IsNullOrWhiteSpace(item.Url))
                    {
                        url = item.Url;
                    }

                    if (string.IsNullOrWhiteSpace(url) && item.Videos.Count == 1)
                    {
                        url = TryToGetUrlFromMeadMe(readMeFile, Path.GetFileName(item.Directory));
                    }

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = TryToGetUrlFromMeadMe(readMeFile, Path.GetFileName(Path.GetDirectoryName(vidPath)));
                    }

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = item.Url;
                    }

                    MediaIntermediateItem intermediateItem = new MediaIntermediateItem
                    {
                        Directory = item.Directory,
                        FilePath = vidPath,
                        MainImage = item.HasCustomCover ? null : item.MainImage,
                        Images = item.Images.Distinct().ToList(),
                        Group = group,
                        Type = item.Type,
                        Url = url,
                        GroupCustomCover = item.HasCustomCover ? item.MainImage : null
                    };

                    groupItems.Add(intermediateItem);
                    result.Add(intermediateItem);
                }
            }

            // Apply directory ordering
            result
                .Where(x => CommonRegex.OrderedTitleRegex.IsMatch(Path.GetFileName(Path.GetDirectoryName(x.FilePath))))
                .GroupBy(x => Path.GetFileName(Path.GetDirectoryName(x.FilePath)))
                .ForEach(x =>
                {
                    string dirOrder = $"{x.Key.Split('.').First()}.";
                    Match match = CommonRegex.OrderedTitleRegex.Match(dirOrder);
                    
                    string num = match.Groups["num"].Value;
                    string sub = match.Groups["sub"].Value.ToLower();

                    if (string.IsNullOrWhiteSpace(sub))
                    {
                        sub = "0000";
                    }
                    else
                    {
                        sub = (((int) sub[0]) - 97).ToString("0000");
                    }

                    x.ForEach(y => y.DirectoryOrder = $"{num.PadLeft(4, '0')}-{sub}");
                })
                ;

            return result;
        }

        private Dictionary<string, (string directory, List<MediaCrawlerItem> items)> GetPotentialGroups(MediaCrawlContext ctx)
        {
            return ctx.Items
                .Select<MediaCrawlerItem, (string path, MediaCrawlerItem item)>(x => (x.Directory.Replace(ctx.Path, string.Empty), x))
                .GroupBy(x => Path.GetDirectoryName(x.path).Replace("\\", string.Empty).Replace("/", string.Empty))
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .Where(x => x.All(y => CommonRegex.OrderedTitleRegex.IsMatch(Path.GetFileName(y.item.Directory))))
                .Where(x => x.All(y =>
                {
                    return Directory.EnumerateFiles(y.item.Directory).Count(FileExtensions.IsVideo) == 1
                        || (y.item.Videos.Count == 1 && y.item.Type == MediaType.Bdvm)
                        ;
                }))
                .ToDictionary(k => k.Key, v => (Path.GetDirectoryName(v.First().item.Directory), v.Select(x => x.item).ToList()))
                ;
        }

        private void InnerCrawl(MediaCrawlContext ctx)
        {
            ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));

            Console.WriteLine($"Crawling {ctx.Path}");

            if (!Directory.Exists(ctx.Path))
            {
                Console.WriteLine($"Skipped path because directory `{ctx.Path}` does not exist.");
                return;
            }

            if (IgnoredPathKeywords.IsMatch(ctx.Path))
            {
                Console.WriteLine($"Skipped path because path `{ctx.Path}` contains ignored keyword.");
                return;
            }

            foreach (IGrouping<FileAttributes, string> grouping in Directory.EnumerateFileSystemEntries(ctx.Path)
                .GroupBy(x => File.GetAttributes(x).HasFlag(FileAttributes.Directory) ? FileAttributes.Directory : FileAttributes.Normal)
                .OrderByDescending(x => x.Key))
            {
                switch (grouping.Key)
                {
                    case FileAttributes.Normal:
                    {
                        if (grouping.Any(FileExtensions.IsVideo))
                        {
                            string directory = Path.GetDirectoryName(grouping.First());
                            string mainImage = grouping.Where(FileExtensions.IsImage).FirstOrDefault();

                            List<string> videos = grouping.Where(FileExtensions.IsVideo)
                                .Where(x => !x.Contains("NCOP"))
                                .OrderBy(x => x.Length)
                                .ThenBy(x => x, StringComparer.InvariantCultureIgnoreCase)
                                .ToList();

                            List<string> images = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                                .Where(x => FileExtensions.IsImage(x) && !string.Equals(x, mainImage))
                                .ToList();

                            string localReadMeFilePath = FindReadMeFile(directory);
                            NutaReadMeFile readMeFile = new NutaReadMeDecoder().Deserialize(localReadMeFilePath);

                            MediaCrawlerItem item = new MediaCrawlerItem()
                            {
                                Directory = directory,
                                MainImage = mainImage,
                                Images = images,
                                Videos = videos,
                                Type = videos.Count > 1 ? MediaType.Season : MediaType.Video,
                                Url = videos.Count == 1 && !string.IsNullOrWhiteSpace(localReadMeFilePath) && Path.GetDirectoryName(localReadMeFilePath).Equals(Path.GetDirectoryName(videos.First()))
                                    ? (readMeFile?.Entries?.Values?.FirstOrDefault()?.Url ?? TryToGetUrlFromMeadMe(readMeFile, Path.GetFileName(directory)))
                                    : TryToGetUrlFromMeadMe(readMeFile, Path.GetFileName(directory))
                            };

                            ctx.Items.Add(item);

                            return;
                        }

                        break;
                    }
                    case FileAttributes.Directory:
                    {
                        if (grouping.Any(x => OrderedDirectoryRegex.IsMatch(Path.GetFileName(x))))
                        {
                            string directory = Path.GetDirectoryName(grouping.First());

                            foreach (string path in grouping)
                            {
                                MediaCrawlContext innerCtx = new MediaCrawlContext() { Path = path };
                                
                                InnerCrawl(innerCtx);

                                ctx.Items.AddRange(innerCtx.Items);
                            }
                        }
                        else if (grouping.Any(FileExtensions.IsBdmv))
                        {
                            // There are 2 cases.
                            // * Directory with single BDMV directory
                            // * Directory with multiple BDMVS - directories
                            // The 2nd case is tricky, because it will be traversed
                            // like normal directory tree, but we need to capture
                            // it like single directory with multiple files.

                            string video = Path.GetDirectoryName(grouping.First());
                            string directory = Path.GetDirectoryName(video);

                            List<string> videos = new List<string>();

                            if (Directory.EnumerateDirectories(directory).All(x => Directory.EnumerateDirectories(x).Any(FileExtensions.IsBdmv)))
                            {
                                videos.AddRange(Directory.EnumerateDirectories(directory));
                            }
                            else
                            {
                                videos.Add(video);
                            }

                            string mainImage = grouping.Where(FileExtensions.IsImage).FirstOrDefault();

                            List<string> images = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                                    .Where(x => FileExtensions.IsImage(x) && !string.Equals(x, mainImage))
                                    .ToList();

                            var item = new MediaCrawlerItem()
                            {
                                Directory = directory,
                                MainImage = mainImage ?? images.FirstOrDefault(),
                                Images = images,
                                Type = MediaType.Bdvm,
                                Videos = new List<string>() { video }
                            };

                            if (!IgnoredPaths.Contains(item.Directory) && item.Videos.All(y => !IgnoredPaths.Contains(y)))
                            {
                                ctx.Items.Add(item);

                                IgnoredPaths.Add(item.Directory);
                                item.Videos.ForEach(x => IgnoredPaths.Add(x));
                            }
                        }
                        else
                        {
                            foreach (string path in grouping)
                            {
                                MediaCrawlContext innerCtx = new MediaCrawlContext() { Path = path };

                                InnerCrawl(innerCtx);

                                ctx.Items.AddRange(innerCtx.Items);
                            }
                        }

                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException($"Unsupported key `{grouping.Key}`.");
                    }
                }
            }

            string readMeFilePath = FindReadMeFile(ctx.Path);
            if (!string.IsNullOrWhiteSpace(readMeFilePath))
            {
                NutaReadMeFile readMeFile = new NutaReadMeDecoder().Deserialize(readMeFilePath);

                foreach (MediaCrawlerItem item in ctx.Items)
                {
                    item.Url = TryToGetUrlFromMeadMe(readMeFile, Path.GetFileName(item.Directory));
                }
            }
        }

        private string FindReadMeFile(string directory)
        {
            return Directory.EnumerateFiles(directory).FirstOrDefault(x => string.Equals(Path.GetFileName(x), "READ ME.txt"));
        }

        private string TryToGetUrlFromMeadMe(NutaReadMeFile readMeFile, string dirName)
        {
            if (readMeFile == null || string.IsNullOrWhiteSpace(dirName))
            {
                return null;
            }

            if (readMeFile.Entries.TryGetValue(dirName, out NutaReadMeEntry entry)
                        || (entry = readMeFile.Entries
                            .Values
                            .FirstOrDefault(x => string.Equals(
                                CommonRegex.OrderedTitleRegexWithTitle.Match(x.Header).Groups["title"].Value,
                                dirName))
                        ) != null)
            {
                return entry.Url;
            }

            return null;
        }

        private string ExtendGroup(Dictionary<string, (string directory, List<MediaIntermediateItem> items)> groups, MediaCrawlerItem item, string group)
        {
            int maxIterations = 10;

            while (groups.ContainsKey(group))
            {
                (string directory, List<MediaIntermediateItem> items) oldGroupItems = groups[group];
                string newGroupKey = GetGroupName(oldGroupItems.directory, group);
                oldGroupItems.items.ForEach(x => x.Group = newGroupKey);
                groups.Remove(group);
                groups[newGroupKey] = oldGroupItems;

                group = GetGroupName(item.Directory, group);

                if (maxIterations-- < 0)
                {
                    throw new InvalidOperationException($"Failed to extend group item `{item.Directory}`");
                }
            }

            return group;
        }

        private string GetGroupName(string directory, string group)
        {
            return directory.Split('\\').Reverse().Take(group.Split('\\').Count() + 1).Reverse().Join("\\");
        }
    }
}
