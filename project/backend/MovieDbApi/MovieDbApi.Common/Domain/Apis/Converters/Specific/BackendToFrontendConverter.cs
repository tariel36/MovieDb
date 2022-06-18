using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Apis.Converters.Models;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Monitoring;
using MovieDbApi.Common.Domain.Utility;
using MovieDbApi.Common.Maintenance.Logging.Abstract;

namespace MovieDbApi.Common.Domain.Apis.Converters.Specific
{
    public class BackendToFrontendConverter
        : IBackendToFrontendConverter
    {
        private readonly string[] TranslatableKeys = new string[]
        {
            nameof(MediaMonitorIntermediateMediaItem.MediaType),
            nameof(MediaMonitorIntermediateMediaItem.Duration),
            nameof(MediaMonitorIntermediateMediaItem.DurationPerEpisode),
            nameof(MediaMonitorIntermediateMediaItem.Genre),
            nameof(MediaMonitorIntermediateMediaItem.GroupCount),
            nameof(MediaMonitorIntermediateMediaItem.Rated),
            nameof(MediaMonitorIntermediateMediaItem.Rating),
            nameof(MediaMonitorIntermediateMediaItem.ReleaseDate),
            nameof(MediaMonitorIntermediateMediaItem.Staff),
            nameof(MediaMonitorIntermediateMediaItem.Type),
            nameof(MediaMonitorIntermediateMediaItem.Year)
        };

        private readonly string[] TranslatableValues = new string[]
        {
            nameof(MediaMonitorIntermediateMediaItem.Genre),
            nameof(MediaMonitorIntermediateMediaItem.MediaType),
        };

        private readonly string[] ReturnableKeys = new string[]
        {
            nameof(MediaMonitorIntermediateMediaItem.Duration),
            nameof(MediaMonitorIntermediateMediaItem.DurationPerEpisode),
            nameof(MediaMonitorIntermediateMediaItem.Genre),
            nameof(MediaMonitorIntermediateMediaItem.GroupCount),
            nameof(MediaMonitorIntermediateMediaItem.Rated),
            nameof(MediaMonitorIntermediateMediaItem.Rating),
            nameof(MediaMonitorIntermediateMediaItem.ReleaseDate),
            nameof(MediaMonitorIntermediateMediaItem.Staff),
            nameof(MediaMonitorIntermediateMediaItem.Type),
            nameof(MediaMonitorIntermediateMediaItem.Year),
            nameof(MediaMonitorIntermediateMediaItem.MediaType)
        };

        private readonly ITranslator _translator;
        private readonly ILoggerService _logger;
        private readonly MediaContext _mediaContext;

        public BackendToFrontendConverter(ITranslator translator,
            ILoggerService logger,
            MediaContext mediaContext)
        {
            _translator = translator;
            _logger = logger;
            _mediaContext = mediaContext;
        }

        private ICollection<ScannedPath> ScannedPaths { get; set; }

        public MediaItem Convert(BackendToFrontendConverterContex ctx)
        {
            if (ScannedPaths == null)
            {
                ScannedPaths = _mediaContext.ScannedPaths.ToList();
            }

            MediaItem item = ctx.MediaItem;

            MediaItem result = ObjectCopy.ShallowCopy(item);

            if (result.Titles == null)
            {
                result.Titles = new List<MediaItemTitle>();
            }

            result.Titles.Add(new MediaItemTitle(_translator.Translate(ctx.FromLanguage, ctx.ToLanguage, item.Title)));
            result.Description = _translator.Translate(ctx.FromLanguage, ctx.ToLanguage, item.Description);
            result.Instructions = GetInstructions(item);
            result.ItemsCount = item.ItemsCount;
            result.DirectoryOrder = item.DirectoryOrder;
            
            try
            {
                result.DisplayPath = item.Path;

                foreach (ScannedPath scannedPath in ScannedPaths)
                {
                    result.DisplayPath = result.DisplayPath.Replace(scannedPath.Path, scannedPath.DisplayPath);
                }

                if (File.Exists(item.Path))
                {
                    result.DirectoryPath = Path.GetDirectoryName(item.Path);
                }
                else if (File.Exists(result.DisplayPath))
                {
                    result.DirectoryPath = Path.GetDirectoryName(result.DisplayPath);
                }

                if (!string.IsNullOrWhiteSpace(result.DirectoryPath))
                {
                    foreach (ScannedPath scannedPath in ScannedPaths)
                    {
                        result.DirectoryPath = result.DirectoryPath.Replace(scannedPath.Path, scannedPath.DisplayPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            if (item.Attributes?.Count > 0)
            {
                List<MediaItemAttribute> attributes = new List<MediaItemAttribute>();

                foreach (MediaItemAttribute attr in item.Attributes.Where(x => ReturnableKeys.Contains(x.Name)))
                {
                    string name = ConvertKeyToText(attr);
                    name = TranslatableKeys.Contains(attr.Name)
                        ? _translator.Translate(ctx.FromLanguage, ctx.ToLanguage, name)
                        : name
                        ;
                    string value = TranslateValue(ctx, attr);

                    MediaItemAttribute newAttr = new MediaItemAttribute()
                    {
                        Name = name,
                        Value = value
                    };

                    attributes.Add(newAttr);
                }

                result.Attributes = attributes;
            }

            if (item.Languages?.Count > 0)
            {
                List<MediaItemLanguage> langs = new List<MediaItemLanguage>();

                foreach (MediaItemLanguage lang in item.Languages)
                {
                    langs.Add(new MediaItemLanguage()
                    {
                        Language = _translator.Translate(ctx.FromLanguage, ctx.ToLanguage, lang.Language),
                        Type = lang.Type,
                    });
                }

                result.Languages = langs;
            }

            if (item.Images?.Count > 0)
            {
                result.Images = item.Images.ToList();
            }

            return result;
        }

        private string TranslateValue(BackendToFrontendConverterContex ctx, MediaItemAttribute attr)
        {
            if (!TranslatableValues.Contains(attr.Name))
            {
                return HumanizeValue(attr);
            }

            switch (attr.Name)
            {
                case nameof(MediaMonitorIntermediateMediaItem.Genre):
                {
                    return (attr.Value ?? string.Empty).Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .Select(x => _translator.Translate(ctx.FromLanguage, ctx.ToLanguage, x))
                        .Join(", ");
                }
            }

            return _translator.Translate(ctx.FromLanguage, ctx.ToLanguage, attr.Value);
        }

        private string ConvertKeyToText(MediaItemAttribute attr)
        {
            StringBuilder sb = new StringBuilder();

            string key;

            switch (attr.Name)
            {
                case nameof(MediaMonitorIntermediateMediaItem.DurationPerEpisode):
                {
                    return "Duration (Episode)";
                }
                case nameof(MediaMonitorIntermediateMediaItem.GroupCount):
                {
                    return "Episodes";
                }
                case nameof(MediaMonitorIntermediateMediaItem.Rating):
                {
                    return "Rating";
                }
                default:
                {
                    key = attr.Name;
                    break;
                }
            }

            for (int i = 0; i < key.Length; ++i)
            {
                if (char.IsUpper(key[i]) && i > 0)
                {
                    sb.Append(' ').Append(char.ToLower(key[i]));
                }
                else
                {
                    sb.Append(key[i]);
                }
            }

            return sb.ToString();
        }

        private string HumanizeValue(MediaItemAttribute attr)
        {
            switch (attr.Name)
            {
                case nameof(MediaMonitorIntermediateMediaItem.Duration):
                case nameof(MediaMonitorIntermediateMediaItem.DurationPerEpisode):
                {
                    if (!double.TryParse(attr.Value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out double minutes))
                    {
                        return string.Empty;
                    }

                    TimeSpan ts = TimeSpan.FromMinutes(minutes);

                    return string.Equals("0", ts.TotalHours.ToString("0.##", CultureInfo.InvariantCulture).Split('.').FirstOrDefault())
                        ? ts.ToString("mm\\m")
                        : ts.ToString("%h\\h\\ mm\\m")
                        ;
                }
                case nameof(MediaMonitorIntermediateMediaItem.Rated):
                {
                    return attr.Value.ToUpper();
                }
                case nameof(MediaMonitorIntermediateMediaItem.ReleaseDate):
                {
                    if (!DateTime.TryParse(attr.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    {
                        return string.Empty;
                    }

                    return dt.ToString("yyyy-MM-dd");
                }
                case nameof(MediaMonitorIntermediateMediaItem.Type):
                {
                    return attr.Value.ToUpper();
                }
                default:
                {
                    return attr.Value;
                }
            }
        }

        private string GetInstructions(MediaItem item)
        {
            if (item.Attributes?.Any(x =>
                string.Equals(x.Name, nameof(MediaMonitorIntermediateMediaItem.FileType))
                && string.Equals(x.Value, MediaType.Bdvm.ToString())) == true)
            {
                return "bdvm";
            }

            if (item.IsGrouping)
            {
                return null;
            }

            return item.Type switch
            {
                MediaItemType.Anime => "anime",
                MediaItemType.Cartoon => "generic",
                MediaItemType.Movie => "generic",
                MediaItemType.Series => "generic",
                _ => null
            };
        }
    }
}
