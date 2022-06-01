namespace MovieDbApi.Common.Domain.Files
{
    public static class Paths
    {
        public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars()
            .Concat(Path.GetInvalidFileNameChars())
            .Where(x => !string.IsNullOrWhiteSpace(x.ToString()))
            .Distinct()
            .ToArray();
    }
}
