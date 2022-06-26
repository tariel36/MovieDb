using Microsoft.EntityFrameworkCore;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Compression.Abstract;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Dto;
using MovieDbApi.Common.Domain.Media.Services.Abstract;

namespace MovieDbApi.Common.Domain.Media.Services.Specific
{
    public class MediaService
        : IMediaService
    {
        private readonly MediaContext _mediaContext;
        private readonly IHashProvider _hashProvider;

        public MediaService(MediaContext context, IHashProvider hashProvider)
        {
            _mediaContext = context;
            _hashProvider = hashProvider;
        }

        public IQueryable<MediaItem> MediaItems { get { return _mediaContext.MediaItems; } }

        public TranslationCache SaveTranslationCache(TranslationCache item)
        {
            if (string.IsNullOrWhiteSpace(item.SourceHash))
            {
                item.SourceHash = _hashProvider.Get(item.Source);
            }

            _mediaContext.Add(item);
            _mediaContext.SaveChanges();

            return item;
        }

        public TranslationCache GetTranslationCache(string targetLanguage, string value)
        {
            string sourceHash = _hashProvider.Get(value);
            return _mediaContext.TranslationCache.FirstOrDefault(x => x.Language == targetLanguage && x.SourceHash == sourceHash);
        }

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

        public void Delete(MediaItem item)
        {
            MediaItem itemToDelete = QueryMediaItemsDetails().First(x => x.Id == item.Id);

            _mediaContext.Remove(itemToDelete);
            _mediaContext.SaveChanges();
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
            return QueryMediaItemsDetails().FirstOrDefault(x => x.Id == id);
        }

        public GroupMediaItem GetGroup(int id)
        {
            MediaItem group = GetById(id);

            if (group == null)
            {
                return null;
            }

            List<MediaItem> groupItems = QueryMediaItemsDetails()
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
            IQueryable<MediaItem>? query = QueryMediaItemsDetails().Where(x => x.IsGrouping == true || x.GroupId == null);

            if (types?.Length > 0)
            {
                query = query.Where(x => types.Contains(x.Type));
            }

            return query.ToList();
        }

        private IQueryable<MediaItem> QueryMediaItems()
        {
            return _mediaContext.MediaItems
                .Include(x => x.Image)
                .OrderBy(x => x.Title)
                ;
        }

        private IQueryable<MediaItem> QueryMediaItemsDetails()
        {
            return _mediaContext.MediaItems
                .Include(x => x.Attributes)
                .Include(x => x.Titles)
                .Include(x => x.Links)
                .Include(x => x.Languages)
                .Include(x => x.Images)
                .Include(x => x.Image)
                .OrderBy(x => x.Title)
                ;
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
