using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Apis.Abstract
{
    public interface IMediaDataProvider
    {
        int Order { get; }

        SearchResult SearchByTitle(string title);

        ApiMediaItemDetails SearchDetailsByTitle(string title);

        bool IsSupported(MediaItemType type);
    }
}
