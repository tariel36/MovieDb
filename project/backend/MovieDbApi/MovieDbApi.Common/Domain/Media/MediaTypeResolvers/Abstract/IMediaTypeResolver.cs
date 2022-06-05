using MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.MediaTypeResolvers.Abstract
{
    public interface IMediaTypeResolver
    {
        MediaItemType Resolve(MediaTypeResolverContext ctx);
    }
}
