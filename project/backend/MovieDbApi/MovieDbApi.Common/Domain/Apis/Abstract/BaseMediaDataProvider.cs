using System.Net;
using MovieDbApi.Common.Domain.Apis.Models;
using Newtonsoft.Json;

namespace MovieDbApi.Common.Domain.Apis.Abstract
{
    public abstract class BaseMediaDataProvider
        : IMediaDataProvider
    {
        protected BaseMediaDataProvider(int order)
        {
            Order = order;
        }

        public int Order { get; }

        public abstract SearchResult SearchByTitle(string title);

        public abstract ApiMediaItemDetails SearchDetailsByTitle(string title);

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
