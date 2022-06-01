using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemTitle
    {
        public MediaItemTitle()
        {

        }

        public MediaItemTitle(string title)
        {
            Title = title;
        }

        [Key]
        public int Id { get; set; }

        public string Title { get; set; }
    }
}
