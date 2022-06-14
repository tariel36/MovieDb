using System.Text.RegularExpressions;
using MovieDbApi.Common.Domain.Files.Decoders.NutaReadMe.Models;

namespace MovieDbApi.Common.Domain.Files.Decoders.NutaReadMe
{
    public class NutaReadMeDecoder
    {
        private static readonly Regex FileRegex = new Regex("((?<header>[0-9]+([a-zA-Z]+)?\\. .*)\r?\n([ ]|\t)+(?<url>https?:\\/\\/.*))+", RegexOptions.Compiled);

        public NutaReadMeFile Deserialize(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                string fileContent = File.ReadAllText(filePath);

                MatchCollection matches = FileRegex.Matches(fileContent);

                List<NutaReadMeEntry> entries = matches.Select(x => new NutaReadMeEntry
                {
                    Header = x.Groups["header"].Value.Trim(),
                    Url = x.Groups["url"].Value.Trim()
                }).ToList();

                return new NutaReadMeFile
                {
                    FilePath = filePath,
                    Entries = entries.ToDictionary(k => k.Header)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(filePath);
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
