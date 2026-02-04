using System.Text.RegularExpressions;

namespace MultiTuner.RegexHelpers
{
    public static class VideoTitleCleaner
    {
        private static readonly Regex JunkRegex = new Regex(
            @"(?i)(\[|\()(\s*official\s*video|official\s*audio|lyrics|official|video|4k|hd|hq|music\s*video)(\s*(\)|\]))",
            RegexOptions.Compiled);

        private static readonly Regex TrailingMetadataRegex = new Regex(
            @"\s*[\(\[].*?[\)\]]\s*$", RegexOptions.Compiled);

        public static (string Artist, string Title) Clean(string videoTitle, string channelTitle)
        {
            if (string.IsNullOrWhiteSpace(videoTitle))
                return ("Unknown Artist", "Unknown Title");

            string cleanTitle = JunkRegex.Replace(videoTitle, "").Trim();
            cleanTitle = TrailingMetadataRegex.Replace(cleanTitle, "").Trim();
            cleanTitle = Regex.Replace(cleanTitle, @"\s*\|.*$", "").Trim();

            string artist = CleanArtistName(channelTitle);
            string title = cleanTitle;

            // --- NEW: Channel name appears in title ---
            if (!string.IsNullOrWhiteSpace(artist))
            {
                var normalizedArtist = Regex.Escape(artist);

                // Matches: "Narvent - Fainted", "Narvent: Fainted", "Narvent – Fainted"
                var prefixRegex = new Regex(
                    $"^(?i){normalizedArtist}\\s*[-–—:]\\s*",
                    RegexOptions.Compiled);

                if (prefixRegex.IsMatch(cleanTitle))
                {
                    title = prefixRegex.Replace(cleanTitle, "").Trim();
                    return (artist, title);
                }
            }

            // --- Fallback: split by separators ---
            var separators = new[] { " - ", " – ", " — ", ":" };
            foreach (var sep in separators)
            {
                if (cleanTitle.Contains(sep))
                {
                    var parts = cleanTitle.Split(new[] { sep }, 2, StringSplitOptions.None);
                    artist = CleanArtistName(parts[0]);
                    title = parts[1].Trim();
                    break;
                }
            }

            return (artist, title);
        }

        private static string CleanArtistName(string rawArtist)
        {
            if (string.IsNullOrWhiteSpace(rawArtist))
                return "Unknown Artist";

            return rawArtist.Replace("VEVO", "", StringComparison.OrdinalIgnoreCase)
                            .Replace("- Topic", "", StringComparison.OrdinalIgnoreCase)
                            .Trim();
        }
    }
}
