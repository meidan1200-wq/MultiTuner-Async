using System.Text.RegularExpressions;

class SpotifyUriResolver
{
    private static readonly Regex _uriRegex = new Regex("http(s)?\\:\\/\\/open\\.spotify\\.com\\/playlist\\/(?<Uri>[0-9A-Za-z]{22})(\\?si=[a-zA-z0-9]{15,22})?");

    private static readonly Regex _uriRegexAlbums = new Regex("http(s)?\\:\\/\\/open\\.spotify\\.com\\/album\\/(?<Uri>[0-9A-Za-z]{22})(\\?si=[a-zA-z0-9]{15,22})?");

    public static bool isvalid(string uri)
    {
        return _uriRegex.IsMatch(uri);
    }

    public static string validUrl(string uri)
    {
        if (_uriRegex.IsMatch(uri))
        {
            return uri.Substring(uri.LastIndexOf("/") + 1, 22);
        }
        else
        {
            return string.Empty;
        }
    }
}

