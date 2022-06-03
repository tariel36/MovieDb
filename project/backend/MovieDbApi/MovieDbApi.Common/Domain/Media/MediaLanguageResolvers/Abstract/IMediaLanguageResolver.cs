using MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.MediaLanguageResolvers.Abstract
{
    public interface IMediaLanguageResolver
    {
        List<MediaItemLanguage> Resolve(MediaLanguageResolverContext ctx);
    }
}
