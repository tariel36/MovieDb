using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class MediaItemAttribute
    {
        public MediaItemAttribute()
        {

        }

        public MediaItemAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
