namespace MovieDbApi.Common.Domain.Files.Decoders.NutaReadMe.Models
{
    public class NutaReadMeFile
    {
        public string FilePath { get; set; }

        public Dictionary<string, NutaReadMeEntry> Entries { get; set; }
    }
}
