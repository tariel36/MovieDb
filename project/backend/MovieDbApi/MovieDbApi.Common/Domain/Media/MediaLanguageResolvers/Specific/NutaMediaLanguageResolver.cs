using MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Abstract;
using MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Specific
{
    public class NutaMediaLanguageResolver
        : IMediaLanguageResolver
    {
        public List<MediaItemLanguage> Resolve(MediaLanguageResolverContext ctx)
        {
            List<MediaItemLanguage> languages = new List<MediaItemLanguage>();

            if (ctx.Path.Contains("anime_pl/"))
            {
                languages.Add(new MediaItemLanguage(MediaLanguageType.Voice, "Japanese"));
                languages.Add(new MediaItemLanguage(MediaLanguageType.Subtitles, "Polish"));
            }
            else if (ctx.Path.Contains("_pl/"))
            {
                languages.Add(new MediaItemLanguage(MediaLanguageType.Voice, "Polish"));
            }
            else if (ctx.Path.Contains("anime/"))
            {
                languages.Add(new MediaItemLanguage(MediaLanguageType.Voice, "Japanese"));
                languages.Add(new MediaItemLanguage(MediaLanguageType.Subtitles, "English"));
            }
            else if (!ctx.Path.Contains("concert/"))
            {
                languages.Add(new MediaItemLanguage(MediaLanguageType.Voice, "English"));
                languages.Add(new MediaItemLanguage(MediaLanguageType.Subtitles, "Polish"));
            }

            return languages;
        }
    }
}
