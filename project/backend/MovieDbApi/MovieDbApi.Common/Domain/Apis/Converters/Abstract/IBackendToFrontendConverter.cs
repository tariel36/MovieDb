using MovieDbApi.Common.Domain.Apis.Converters.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Apis.Converters.Abstract
{
    public interface IBackendToFrontendConverter
    {
        MediaItem Convert(BackendToFrontendConverterContex ctx);
    }
}
