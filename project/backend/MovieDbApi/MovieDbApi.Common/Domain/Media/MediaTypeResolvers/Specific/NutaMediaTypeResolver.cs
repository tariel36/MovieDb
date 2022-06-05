using MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Abstract;
using MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Specific
{
    public class NutaMediaTypeResolver
        : IMediaTypeResolver
    {
        public MediaItemType Resolve(MediaTypeResolverContext ctx)
        {
            if (ctx.Path.Contains("anime", StringComparison.InvariantCultureIgnoreCase))
            {
                return MediaItemType.Anime;
            }

            if (ctx.Path.Contains("cartoons", StringComparison.InvariantCultureIgnoreCase))
            {
                return MediaItemType.Cartoon;
            }

            if (ctx.Path.Contains("concert", StringComparison.InvariantCultureIgnoreCase))
            {
                return MediaItemType.Concert;
            }

            if (ctx.Path.Contains("movies", StringComparison.InvariantCultureIgnoreCase))
            {
                return MediaItemType.Movie;
            }

            if (ctx.Path.Contains("series", StringComparison.InvariantCultureIgnoreCase))
            {
                return MediaItemType.Series;
            }

            return MediaItemType.Unknown;
        }
    }
}
