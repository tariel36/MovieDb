using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemLink
    {
        public MediaItemLink()
        {

        }

        public MediaItemLink(string link)
        {
            Link = link;
        }

        [Key]
        public int Id { get; set; }

        public string Link { get; set; }
    }
}
