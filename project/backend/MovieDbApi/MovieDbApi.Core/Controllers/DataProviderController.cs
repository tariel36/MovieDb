using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Apis.Converters.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Dto;
using MovieDbApi.Common.Domain.Media.Services.Abstract;

namespace MovieDbApi.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataProviderController
        : ControllerBase
    {
        private const string Language = nameof(Language);
        private const string MediaItemTypes = nameof(MediaItemTypes);
        private const string SourceLanguage = "en";

        private readonly IMediaService _mediaService;
        private readonly IBackendToFrontendConverter _converter;

        public DataProviderController(IMediaService mediaService, IBackendToFrontendConverter converter)
        {
            _mediaService = mediaService;
            _converter = converter;
        }

        [HttpPost("options/NotificationEmail")]
        public void SetNotificationEmail()
        {
            MediaItemType[] types = GetMediaItemTypes();

            string email;

            using (TextReader tr = new StreamReader(Request.Body))
            {
                email = tr.ReadToEndAsync().Result;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            _mediaService.SetNotificationEmail(email, types);
        }

        [HttpPost("search")]
        public List<MediaItem> GetSearch()
        {
            string targetLang = GetTargetLanguage();
            MediaItemType[] types = GetMediaItemTypes();

            string queryText;

            using (TextReader tr = new StreamReader(Request.Body))
            {
                queryText = tr.ReadToEndAsync().Result;
            }

            List<MediaItem> result = new List<MediaItem>();

            foreach (var grouping in _mediaService.SearchElements(queryText, types).GroupBy(x => x.Group))
            {
                MediaItem groupingItem = grouping.FirstOrDefault(x => x.IsGrouping);

                if (groupingItem != null)
                {
                    result.Add(groupingItem);
                }
                else
                {
                    result.AddRange(grouping);
                }
            }

            return result.Select(x => _converter.Convert(new BackendToFrontendConverterContex(x, SourceLanguage, targetLang))).ToList();
        }

        [HttpGet("random")]
        public int GetRandom()
        {
            MediaItemType[] types = GetMediaItemTypes();

            List<MediaItem> items = _mediaService.GetSingleElementsByTypes(types);

            int idx = new Random().Next(0, items.Count);

            MediaItem item = items[idx];

            return item.Id;
        }

        [HttpGet()]
        public List<MediaItem> GetAllMediaItems()
        {
            string targetLang = GetTargetLanguage();
            MediaItemType[] types = GetMediaItemTypes();

            List<MediaItem> result = new List<MediaItem>();

            foreach (var grouping in _mediaService.GetAll(types).GroupBy(x => x.Group))
            {
                MediaItem groupingItem = grouping.FirstOrDefault(x => x.IsGrouping);

                if (groupingItem != null)
                {
                    result.Add(groupingItem);
                }
                else
                {
                    result.AddRange(grouping);
                }
            }

            return result.Select(x => _converter.Convert(new BackendToFrontendConverterContex(x, SourceLanguage, targetLang))).ToList();
        }

        [HttpGet("{id:int}")]
        public MediaItem GetMediaItem(int id)
        {
            string targetLang = GetTargetLanguage();

            return _converter.Convert(new BackendToFrontendConverterContex(_mediaService.GetById(id), SourceLanguage, targetLang));
        }

        [HttpGet("group/{id:int}")]
        public GroupMediaItem GetGroup(int id)
        {
            string targetLang = GetTargetLanguage();

            GroupMediaItem grp = _mediaService.GetGroup(id);

            if (grp != null)
            {
                grp.Items = grp.Items.Select(x => _converter.Convert(new BackendToFrontendConverterContex(x, SourceLanguage, targetLang))).ToList();
                grp.GroupingItem = _converter.Convert(new BackendToFrontendConverterContex(grp.GroupingItem, SourceLanguage, targetLang));
            }

            return grp;
        }

        [HttpGet("details/{id:int}")]
        public GroupMediaItem GetDetails(int id)
        {
            string targetLang = GetTargetLanguage();

            MediaItem item = _mediaService.GetById(id);

            if (item == null)
            {
                return null;
            }

            return new GroupMediaItem()
            {
                GroupingItem = _converter.Convert(new BackendToFrontendConverterContex(item, SourceLanguage, targetLang)),
                Items = new List<MediaItem>()
            };
        }

        [HttpGet("image/{id:int}")]
        public IActionResult GetImage(int id)
        {
            MediaItemImage image = _mediaService.GetImage(id);

            return File(System.IO.File.OpenRead(image.Image), $"image/{System.IO.Path.GetExtension(image.Image).Replace(".", string.Empty)}");
        }

        private string GetTargetLanguage()
        {
            return GetHeader(Language);
        }

        private MediaItemType[] GetMediaItemTypes()
        {
            return GetHeader(MediaItemTypes).Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => (MediaItemType) int.Parse(x)).ToArray();
        }

        private string GetHeader(string key)
        {
            if (Request.Headers.TryGetValue(key, out StringValues headerValues))
            {
                return headerValues.FirstOrDefault();
            }

            return string.Empty;
        }
    }
}
