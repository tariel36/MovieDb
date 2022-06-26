using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Services.Abstract;

namespace MovieDbApi.Common.Domain.Media.Services.Specific
{
    public class PathsService
        : IPathsService
    {
        private readonly MediaContext _mediaContext;

        public PathsService(MediaContext mediaContext)
        {
            _mediaContext = mediaContext;
        }

        public IQueryable<ScannedPath> ScannedPaths { get { return _mediaContext.ScannedPaths; } }

        public IQueryable<IgnoredPath> IgnoredPaths { get { return _mediaContext.IgnoredPaths; } }

        public ICollection<ScannedPath> GetPathToAnalyze()
        {
            return ScannedPaths.ToList();
        }
    }
}
