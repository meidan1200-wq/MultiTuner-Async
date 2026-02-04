using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MultiTuner.Model.JsonData.SpotifyJsonDeserializer
{
    // --- 1. SEARCH RESPONSE MODEL ---
    public class SpotifySearchResponse
    {
        [JsonProperty("tracks")]
        public SpotifySearchResult Tracks { get; set; }
    }

    public class SpotifySearchResult : PagedResponse<TrackInfo>
    {
        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public class UserPlaylistsResponse
    {
        [JsonProperty("items")]
        public List<PlaylistItem> Items { get; set; }
    }

    public class PlaylistItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls ExternalUrls { get; set; }

        [JsonProperty("owner")]
        public Artist Artist { get; set; }
    }

    // Use this for getting tracks IN a playlist
    public class UserTracksResponse : PagedResponse<PlaylistTrackItem> { }

    public class PlaylistTrackItem
    {
        [JsonProperty("added_at")]
        public DateTimeOffset? AddedAt { get; set; }

        [JsonProperty("track")]
        public TrackInfo Track { get; set; }
    }

    public class PagedResponse<T>
    {
        [JsonProperty("items")]
        public List<T> Items { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }

    // --- 3. CORE SHARED MODELS ---

    public class TrackInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("duration_ms")]
        public long DurationMs { get; set; }

        [JsonProperty("artists")]
        public List<Artist> Artists { get; set; }

        [JsonProperty("album")]
        public SpotifyAlbum Album { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls ExternalUrls { get; set; }
    }

    public class SpotifyAlbum
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }
    }

    public class Artist
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("external_urls")]
        public ExternalUrls ExternalUrls { get; set; }
    }

    public class SpotifyImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }
    }

    public class ExternalUrls
    {
        [JsonProperty("spotify")]
        public string Spotify { get; set; }
    }
}
