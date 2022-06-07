using System.Text.RegularExpressions;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Files;
using MovieDbApi.Common.Domain.Utility;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Crawling.Services
{
    public class MediaCrawlerService
    {
        private readonly Regex OrderedDirectoryRegex = new Regex("^[0-9]+\\.", RegexOptions.Compiled);

        public MediaCrawlerService()
        {
            CrawlingContextProvider = (ctx) =>
            {
                InnerCrawl(ctx);
                return ctx;
            };
        }

        private Func<MediaCrawlContext, MediaCrawlContext> CrawlingContextProvider { get; }

        public List<MediaIntermediateItem> Crawl(MediaCrawlContext ctx)
        {
            // We have some kind of tree structure here.
            // Now we have to squash that to the topmost level,
            // so the single movies and seasons are the only deeper
            // items.
            ctx = CrawlingContextProvider(ctx);

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

                    ctx.Items.Add(new MediaCrawlerItem()
                    {
                        Directory = group.Value.directory,
                        Groups = null,
                        Images = images,
                        IsGrouping = true,
                        MainImage = images.FirstOrDefault(),
                        Type = MediaType.Franchise,
                        Videos = group.Value.items.SelectMany(x => x.Videos).Distinct().ToList()
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

                foreach (string vidPath in item.Videos)
                {
                    MediaIntermediateItem intermediateItem = new MediaIntermediateItem
                    {
                        Directory = item.Directory,
                        FilePath = vidPath,
                        MainImage = item.MainImage,
                        Images = item.Images,
                        Group = group,
                        Type = item.Type,
                    };

                    groupItems.Add(intermediateItem);
                    result.Add(intermediateItem);
                }
            }

            return result;
        }

        private Dictionary<string, (string directory, List<MediaCrawlerItem> items)> GetPotentialGroups(MediaCrawlContext ctx)
        {
            return ctx.Items
                .Select<MediaCrawlerItem, (string path, MediaCrawlerItem item)>(x => (x.Directory.Replace(ctx.Path, string.Empty), x))
                .GroupBy(x => Path.GetDirectoryName(x.path).Replace("\\", string.Empty).Replace("/", string.Empty))
                .Where(x => x.All(y => CommonRegex.OrderedTitleRegex.IsMatch(Path.GetFileName(y.item.Directory))))
                .ToDictionary(k => k.Key, v => (Path.GetDirectoryName(v.First().item.Directory), v.Select(x => x.item).ToList()))
                ;
        }

        private void InnerCrawl(MediaCrawlContext ctx)
        {
            ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));

            if (!Directory.Exists(ctx.Path))
            {
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
                                .ToList();

                            List<string> images = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                                .Where(x => FileExtensions.IsImage(x) && !string.Equals(x, mainImage))
                                .ToList();

                            MediaCrawlerItem item = new MediaCrawlerItem()
                            {
                                Directory = directory,
                                MainImage = mainImage,
                                Images = images,
                                Videos = videos,
                                Type = videos.Count > 1 ? MediaType.Season : MediaType.Video
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
                            string directory = Path.GetDirectoryName(grouping.First());

                            string mainImage = grouping.Where(FileExtensions.IsImage).FirstOrDefault();

                            List<string> images = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
                                .Where(x => FileExtensions.IsImage(x) && !string.Equals(x, mainImage))
                                .ToList();

                            MediaCrawlerItem item = new MediaCrawlerItem()
                            {
                                Directory = directory,
                                MainImage = mainImage,
                                Images = images,
                                Type = MediaType.Bdvm,
                            };

                            ctx.Items.Add(item);
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
        }

        private string ExtendGroup(Dictionary<string, (string directory, List<MediaIntermediateItem> items)> groups, MediaCrawlerItem item, string group)
        {
            while (groups.ContainsKey(group))
            {
                (string directory, List<MediaIntermediateItem> items) oldGroupItems = groups[group];
                string newGroupKey = GetGroupName(oldGroupItems.directory, group);
                oldGroupItems.items.ForEach(x => x.Group = newGroupKey);
                groups.Remove(group);
                groups[newGroupKey] = oldGroupItems;

                group = GetGroupName(item.Directory, group);
            }

            return group;
        }

        private string GetGroupName(string directory, string group)
        {
            return directory.Split('\\').Reverse().Take(group.Split('\\').Count() + 1).Reverse().Join("\\");
        }
    }
}
