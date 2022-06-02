using Microsoft.EntityFrameworkCore;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Dto;
using MovieDbApi.Common.Domain.Media.Services.Abstract;

namespace MovieDbApi.Common.Domain.Media.Services.Specific
{
    public class MediaService
        : IMediaService
    {
        private readonly MediaContext _mediaContext;

        public MediaService(MediaContext context)
        {
            _mediaContext = context;
        }

        public IQueryable<ScannedPath> ScannedPaths { get { return _mediaContext.ScannedPaths; } }

        public IQueryable<MediaItem> MediaItems { get { return _mediaContext.MediaItems; } }

        public void SetNotificationEmail(string email, string language, MediaItemType[] types)
        {
            Subscriber subscriber = _mediaContext.Subscribers.Include(x => x.MediaItemTypes).FirstOrDefault(x => x.Email == email);

            if (subscriber == null)
            {
                subscriber = new Subscriber()
                {
                    Name = email,
                    Email = email,
                    MediaItemTypes = null
                };
            }

            if (subscriber.MediaItemTypes?.Count > 0)
            {
                _mediaContext.RemoveRange(subscriber.MediaItemTypes);
                _mediaContext.SaveChanges();
            }

            subscriber.Language = language;
            subscriber.MediaItemTypes = types?.Select(x => new SubscriberMediaItemType() { Type = x }).ToArray() ?? new SubscriberMediaItemType[0];

            if (subscriber.Id == 0)
            {
                _mediaContext.Add(subscriber);
            }
            else
            {
                _mediaContext.Update(subscriber);
            }

            _mediaContext.SaveChanges();
        }

        public void SaveMediaItem(MediaItem mediaItem)
        {
            mediaItem.DateAdded = mediaItem.DateAdded.ToUniversalTime();

            _mediaContext.Add(mediaItem);
            _mediaContext.SaveChanges();

            mediaItem.DateAdded = mediaItem.DateAdded.ToLocalTime();
        }

        public List<MediaItem> GetAll(MediaItemType[] types)
        {
            IQueryable<MediaItem> query = QueryMediaItems();

            if (types?.Length > 0)
            {
                query = query.Where(x => types.Contains(x.Type));
            }

            return query.ToList();
        }

        public MediaItem GetById(int id)
        {
            return QueryMediaItems().FirstOrDefault(x => x.Id == id);
        }

        public GroupMediaItem GetGroup(int id)
        {
            MediaItem group = GetById(id);

            if (group == null)
            {
                return null;
            }

            List<MediaItem> groupItems = QueryMediaItems()
                .Where(x => x.Group == group.Group)
                .Where(x => !x.IsGrouping)
                .ToList();

            return new GroupMediaItem
            {
                GroupingItem = group,
                Items = groupItems
            };
        }

        public MediaItemImage GetImage(int id)
        {
            return _mediaContext.MediaItemImages.Find(id);
        }

        public List<MediaItem> SearchElements(string queryText, MediaItemType[] types)
        {
            IQueryable<MediaItem> query = QueryMediaItems();

            if (types?.Length > 0)
            {
                query = query.Where(x => types.Contains(x.Type));
            }

            IEnumerable<MediaItem> result = query.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(queryText))
            {
                string[] qParts = queryText.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                if (qParts.Length > 0)
                {
                    result = result.Where(x =>
                        SearchTextFilter(x.Title, qParts)
                        || SearchTextFilter(x.Description, qParts)
                        || SearchTextFilter(x.ChapterTitle, qParts)
                        || (x.Titles != null && x.Titles.Any(y => SearchTextFilter(y.Title, qParts)))
                    );
                }
            }

            return result.ToList();
        }

        public List<MediaItem> GetSingleElementsByTypes(MediaItemType[] types)
        {
            IQueryable<MediaItem>? query = QueryMediaItems().Where(x => x.IsGrouping == true || x.GroupId == null);

            if (types?.Length > 0)
            {
                query = query.Where(x => types.Contains(x.Type));
            }

            return query.ToList();
        }

        private IQueryable<MediaItem> QueryMediaItems()
        {
            return _mediaContext.MediaItems
                .Include(x => x.Attributes)
                .Include(x => x.Titles)
                .Include(x => x.Links)
                .Include(x => x.Languages)
                .Include(x => x.Images);
        }

        private bool SearchTextFilter(string qSource, string[] qParts)
        {
            return SearchTextSplit(qSource).Any(y => SearchTextContainsFilter(qParts, y));
        }

        private bool SearchTextContainsFilter(string[] qParts, string word)
        {
            return qParts != null && qParts.Contains(word, StringComparer.InvariantCultureIgnoreCase);
        }

        private string[] SearchTextSplit(string text)
        {
            return (text ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
