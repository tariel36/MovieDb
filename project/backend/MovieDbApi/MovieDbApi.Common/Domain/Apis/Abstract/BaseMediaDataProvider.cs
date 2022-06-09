using System.Collections.ObjectModel;
using System.Net;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Abstract
{
    public abstract class BaseMediaDataProvider
        : IMediaDataProvider
    {
        protected BaseMediaDataProvider(int order, IEnumerable<MediaItemType> supportedTypes = null)
        {
            Order = order;
            SupportedTypes = new ReadOnlyCollection<MediaItemType>(supportedTypes?.ToList() ?? new List<MediaItemType>());
        }

        public int Order { get; }

        private IReadOnlyCollection<MediaItemType> SupportedTypes { get; }

        public abstract SearchResult SearchByTitle(string title);

        public abstract ApiMediaItemDetails SearchDetailsByTitle(string title);

        public abstract ApiMediaItemDetails GetByUrl(string url);

        public bool IsSupported(MediaItemType type)
        {
            return SupportedTypes.Contains(type);
        }

        protected TValue Get<TValue>(string uri, string header = null)
        {
            try
            {
                string json = Get(uri, header);
                return JsonConvert.DeserializeObject<TValue>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default(TValue);
            }
        }
        
        protected string Get(string uri, string header = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (!string.IsNullOrWhiteSpace(header))
                {
                    request.Headers.Add(header);
                }

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
