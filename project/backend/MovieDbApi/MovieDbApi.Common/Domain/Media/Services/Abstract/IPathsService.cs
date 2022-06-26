using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.Services.Abstract
{
    public interface IPathsService
    {
        IQueryable<ScannedPath> ScannedPaths { get; }

        IQueryable<IgnoredPath> IgnoredPaths { get; }

        ICollection<ScannedPath> GetPathToAnalyze();
    }
}
