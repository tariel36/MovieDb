using System.Collections.ObjectModel;

namespace MovieDbApi.Common.Domain.Files
{
    public static class FileExtensions
    {
        public static IReadOnlyCollection<string> Mirror = new ReadOnlyCollection<string>(new List<string>()
        {
            ".iso"
        });

        public static IReadOnlyCollection<string> Video = new ReadOnlyCollection<string>(new List<string>()
        {
            ".mkv", ".mp4", ".avi", ".vob"
        });

        public static IReadOnlyCollection<string> Images = new ReadOnlyCollection<string>(new List<string>()
        {
            ".png", ".bmp", ".jpg", ".jpeg", ".webp", ".jp2"
        });

        public static IReadOnlyCollection<string> Subtitles = new ReadOnlyCollection<string>(new List<string>()
        {
            ".txt", ".srt", ".ass", ".mks", ".sub"
        });

        public static bool IsVideo(string path)
        {
            return IsExtension(Video, path);
        }

        public static bool IsImage(string path)
        {
            return IsExtension(Images, path);
        }

        public static bool IsBdmv(string path)
        {
            return Path.GetFileName(path).Equals("BDMV", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsExtension(IEnumerable<string> extensions, string path)
        {
            string extension = Path.GetExtension(path);
            return extensions.Any(x => string.Equals(x, extension, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
