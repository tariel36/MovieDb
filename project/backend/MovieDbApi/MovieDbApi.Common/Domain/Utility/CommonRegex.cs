using System.Linq;
using System.Text.RegularExpressions;
using MovieDbApi.Common.Domain.Files;

namespace MovieDbApi.Common.Domain.Utility
{
    public static class CommonRegex
    {
        public static readonly Regex OrderedTitleRegex = new Regex("^(?<num>[0-9]+)(?<sub>[a-zA-Z]+)?\\.", RegexOptions.Compiled);

        public static readonly Regex InvalidPathChars = new Regex($"({Paths.InvalidPathChars.Select(x => Regex.Escape(x.ToString())).Join("|")})", RegexOptions.Compiled);
    }
}
