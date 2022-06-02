using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Dto;
using MovieDbApi.Common.Domain.Media.Services.Abstract;

namespace MovieDbApi.Common.Domain.Media.Services.Specific
{
    public class FakeMediaService
        : IMediaService
    {
        public FakeMediaService()
        {
            ScannedPathsCollection = new List<ScannedPath>();
            MediaItemsCollection = new List<MediaItem>();
        }

        public List<ScannedPath> ScannedPathsCollection { get; set; }
        
        public List<MediaItem> MediaItemsCollection { get; set; }

        public IQueryable<ScannedPath> ScannedPaths { get { return ScannedPathsCollection.AsQueryable(); } }
        
        public IQueryable<MediaItem> MediaItems { get { return MediaItemsCollection.AsQueryable(); } }

        public List<MediaItem> GetAll(MediaItemType[] types)
        {
            throw new NotImplementedException();
        }

        public MediaItem GetById(int id)
        {
            throw new NotImplementedException();
        }

        public GroupMediaItem GetGroup(int id)
        {
            throw new NotImplementedException();
        }

        public MediaItemImage GetImage(int id)
        {
            throw new NotImplementedException();
        }

        public List<MediaItem> GetSingleElementsByTypes(MediaItemType[] types)
        {
            throw new NotImplementedException();
        }

        public void SaveMediaItem(MediaItem mediaItem)
        {
            MediaItemsCollection.Add(mediaItem);
        }

        public List<MediaItem> SearchElements(string queryText, MediaItemType[] types)
        {
            throw new NotImplementedException();
        }

        public void SetNotificationEmail(string email, MediaItemType[] types)
        {
            throw new NotImplementedException();
        }
    }
}
