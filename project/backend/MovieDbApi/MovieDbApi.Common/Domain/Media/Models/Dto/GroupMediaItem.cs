using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Media.Models.Dto
{
    public sealed class GroupMediaItem
    {
        public MediaItem GroupingItem { get; set; }

        public List<MediaItem> Items { get; set; }
    }
}
