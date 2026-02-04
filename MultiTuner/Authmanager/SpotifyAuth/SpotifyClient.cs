using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class SpotifyClient
{
    private readonly SpotifyAuthManager auth;
    private readonly ISpotifyAuthStorage storage;

    private TokenResponse current;

    public SpotifyClient(string clientId)
    {
        storage = new FileAuthStorage();
        auth = new SpotifyAuthManager(clientId);
    }

    // PUBLIC API
    public async Task<string> GetAccessTokenAsync()
    {
        var token = await GetTokenAsync();
        return token.access_token;
    }

    private bool NeedsRefresh()
    {
        if (current == null)
            return true;

        return current.expires_at <= DateTime.UtcNow.AddMinutes(5);
    }

    private async Task<TokenResponse> GetTokenAsync()
    {
        // 1. Check if we really need to refresh
        if (!NeedsRefresh())
            return current;

        var savedRefresh = storage.LoadRefreshToken();

        // 2. Try to Refresh using the file token
        if (!string.IsNullOrEmpty(savedRefresh))
        {
            try
            {
                // Call the manager
                var refreshed = await auth.RefreshAccessTokenAsync(savedRefresh);

                // --- CRITICAL FIX START ---

                // A. Update Expiry
                refreshed.expires_at = DateTime.UtcNow.AddSeconds(refreshed.expires_in);

                // B. Handle Token Rotation (The "Revoked" Fix)
                // If Spotify gave us a new refresh token, SAVE IT TO DISK immediately.
                if (!string.IsNullOrEmpty(refreshed.refresh_token))
                {
                    storage.SaveRefreshToken(refreshed.refresh_token);
                }
                else
                {
                    // If Spotify didn't return a new one, keep using the old one in memory
                    refreshed.refresh_token = savedRefresh;
                }

                // --- CRITICAL FIX END ---

                current = refreshed;
                return current;
            }
            catch (Exception ex)
            {
                // Logging the specific error helps debugging
                Debug.WriteLine($"Refresh failed (forcing login): {ex.Message}");
                // We swallow the error here to fall through to "FULL LOGIN" below
            }
        }

        // 3. Fallback: Full Browser Login
        var login = await auth.AuthorizeUserAsync();
        login.expires_at = DateTime.UtcNow.AddSeconds(login.expires_in);

        // Save the initial refresh token
        if (!string.IsNullOrEmpty(login.refresh_token))
            storage.SaveRefreshToken(login.refresh_token);

        current = login;
        return current;
    }

}
