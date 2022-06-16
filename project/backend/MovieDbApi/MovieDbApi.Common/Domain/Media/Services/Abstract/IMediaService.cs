﻿using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Dto;

namespace MovieDbApi.Common.Domain.Media.Services.Abstract
{
    public interface IMediaService
    {
        IQueryable<ScannedPath> ScannedPaths { get; }
        
        IQueryable<IgnoredPath> IgnoredPaths { get; }

        IQueryable<MediaItem> MediaItems { get; }

        void SaveMediaItem(MediaItem mediaItem);

        List<MediaItem> GetAll(MediaItemType[] types);

        MediaItem GetById(int id);

        GroupMediaItem GetGroup(int id);

        MediaItemImage GetImage(int id);

        List<MediaItem> GetSingleElementsByTypes(MediaItemType[] types);
        
        List<MediaItem> SearchElements(string queryText, MediaItemType[] types);

        void SetNotificationEmail(string email, string language, MediaItemType[] types);

        TranslationCache GetTranslationCache(string targetLanguage, string value);

        void Delete(MediaItem item);
    }
}
