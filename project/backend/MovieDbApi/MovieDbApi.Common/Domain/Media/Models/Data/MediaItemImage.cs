using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemImage
    {
        public MediaItemImage()
        {

        }

        public MediaItemImage(string image)
        {
            Image = image;
        }

        [Key]
        public int Id { get; set; }

        public string Image { get; set; }
    }
}
