using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using MovieDbApi.Common.Domain.Apis.Models;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Utility;
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

        protected TValue Get<TValue>(string uri, params string[] headers)
        {
            try
            {
                string json = Query(uri, null, "get", headers);
                return JsonConvert.DeserializeObject<TValue>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default(TValue);
            }
        }

        protected TValue Post<TValue>(string uri, object body, params string[] headers)
        {
            try
            {
                string json = Query(uri, body, "post", headers);
                return JsonConvert.DeserializeObject<TValue>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default(TValue);
            }
        }

        protected string Query(string uri, object body, string method, params string[] headers)
        {
            int tries = 3;

            while (tries > 0)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
                    request.Method = method;
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    if (headers?.Length > 0)
                    {
                        headers.ForEach(x =>
                        {
                            request.Headers.Add(x);
                        });
                    }

                    if (body != null)
                    {
                        string serialized = JsonConvert.SerializeObject(body);
                        byte[] bytes = Encoding.UTF8.GetBytes(serialized);

                        request.ContentLength = bytes.Length;

                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex);
                    tries--;
                    Throttle().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return null;
                }
            }

            return null;
        }

        protected async Task Throttle(int miliseconds = 1000)
        {
            await Task.Delay(miliseconds);
        }
    }
}
