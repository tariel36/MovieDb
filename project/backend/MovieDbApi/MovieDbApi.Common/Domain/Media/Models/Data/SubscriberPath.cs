using System.ComponentModel.DataAnnotations;

namespace MovieDbApi.Common.Domain.Media.Models.Data
{
    public class SubscriberPath
    {
        [Key]
        public int Id { get; set; }

        //public int IdSubscriber { get; set; }

        public string Path { get; set; }
    }
}
