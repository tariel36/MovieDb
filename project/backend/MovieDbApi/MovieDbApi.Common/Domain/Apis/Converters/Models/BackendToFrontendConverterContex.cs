using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDbApi.Common.Domain.Media.Models.Data;

namespace MovieDbApi.Common.Domain.Apis.Converters.Models
{
    public class BackendToFrontendConverterContex
    {
        public BackendToFrontendConverterContex()
        {

        }

        public BackendToFrontendConverterContex(MediaItem item, string fromLang, string toLang)
        {
            MediaItem = item;
            FromLanguage = fromLang;
            ToLanguage = toLang;
        }

        public MediaItem MediaItem { get; set; }

        public string FromLanguage { get; set; }
        
        public string ToLanguage { get; set; }
    }
}
