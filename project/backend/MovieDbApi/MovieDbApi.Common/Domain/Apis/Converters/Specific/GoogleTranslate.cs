using System.Linq;
using System.Net;
using System.Web;
using MovieDbApi.Common.Data.Specific;
using MovieDbApi.Common.Domain.Apis.Converters.Abstract;
using MovieDbApi.Common.Domain.Media.Models.Data;
using MovieDbApi.Common.Domain.Media.Services.Abstract;
using MovieDbApi.Common.Domain.Media.Services.Specific;
using MovieDbApi.Common.Domain.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieDbApi.Common.Domain.Apis.Converters.Specific
{
    public class GoogleTranslate
        : ITranslator
    {
        private readonly MediaContext _mediaContext;
        private readonly IMediaService _mediaService;

        public GoogleTranslate(MediaContext mediaContext, IMediaService mediaService)
        {
            _mediaContext = mediaContext;
            _mediaService = mediaService;
        }

        public string Translate(string from, string to, string value)
        {
            if (new [] { from, to, value }.Any(string.IsNullOrWhiteSpace))
            {
                return value;
            }

            if (string.Equals(from, to, StringComparison.InvariantCultureIgnoreCase))
            {
                return value;
            }

            string result = GetCachedValue(to, value);

            if (string.IsNullOrWhiteSpace(result))
            {
                result = QueryTranslation(from, to, value);
            }

            return string.IsNullOrWhiteSpace(result)
                ? value
                : result
                ;
        }

        private string GetCachedValue(string targetLanguage, string value)
        {
            return _mediaService.GetTranslationCache(targetLanguage, value)?.Target;
        }

        private string QueryTranslation(string from, string to, string value)
        {
            try
            {
                string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                        + from
                        + "&tl="
                        + to
                        + "&dt=t&q="
                        + HttpUtility.UrlEncode(value)
                        ;

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                string translation;

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    translation = reader.ReadToEnd();
                    translation = ExtractResponse(translation);
                }

                if (!string.IsNullOrWhiteSpace(translation))
                {
                    _mediaContext.Add(new TranslationCache() { Language = to, Source = value, Target = translation });
                    _mediaContext.SaveChanges();
                }

                return translation;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return string.Empty;
        }

        private string ExtractResponse(string response)
        {
            JArray obj = JsonConvert.DeserializeObject(response) as JArray;

            string res = obj[0].Select(x => x.FirstOrDefault()?.ToString()?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Join(" ")
                .Trim();

            return res;
        }
    }
}
