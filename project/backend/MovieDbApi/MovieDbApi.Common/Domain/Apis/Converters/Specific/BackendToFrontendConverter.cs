using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Apis.Converters.Models;
using MovieDbApi.Common.Domain.Crawling.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Models.Monitoring;
using MovieDbApi.Common.Domain.Utility;

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

        public BackendToFrontendConverter(ITranslator translator)
        {
            _translator = translator;
        }

        public MediaItem Convert(BackendToFrontendConverterContex ctx)
        {
            MediaItem item = ctx.MediaItem;

            MediaItem result = ObjectCopy.ShallowCopy(item);

            result.Description = _translator.Translate(ctx.FromLanguage, ctx.ToLanguage, item.Description);
            result.Instructions = GetInstructions(item);

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
                    TimeSpan ts = TimeSpan.FromMinutes(double.Parse(attr.Value.Replace(',', '.'), CultureInfo.InvariantCulture));

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
                    return DateTime.Parse(attr.Value).ToString("yyyy-MM-dd");
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

            return item.Type switch
            {
                MediaItemType.Anime => "anime",
                MediaItemType.Cartoon => "generic",
                MediaItemType.Movie => "generic",
                MediaItemType.Series => "generic",
            };
        }
    }
}
