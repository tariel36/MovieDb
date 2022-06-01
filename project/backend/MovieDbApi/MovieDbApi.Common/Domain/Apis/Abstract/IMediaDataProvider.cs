using MovieDbApi.Common.Domain.Apis.Models;

namespace MovieDbApi.Common.Domain.Apis.Abstract
{
    public interface IMediaDataProvider
    {
        int Order { get; }

        SearchResult SearchByTitle(string title);

        ApiMediaItemDetails SearchDetailsByTitle(string title);
    }
}
