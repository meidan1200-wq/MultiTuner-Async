using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MultiTuner.Authmanager.EncryptionManager.PkceUtilities;

public class SpotifyAuthManager
{
    private readonly string ClientId;
    private readonly string redirectUri = "http://127.0.0.1:5000/callback";

    // PKCE state
    private string _codeVerifier;

    public SpotifyAuthManager(string clientId)
    {
        ClientId = clientId;
    }

    // ----------------------------------------------------------
    // 1. First-time login via browser + redirect (PKCE)
    // ----------------------------------------------------------
    public async Task<TokenResponse> AuthorizeUserAsync()
    {
        string state = Guid.NewGuid().ToString();

        _codeVerifier = OAuthPkceHelper.GenerateCodeVerifier();
        string codeChallenge = OAuthPkceHelper.GenerateCodeChallenge(_codeVerifier);

        string url =
            "https://accounts.spotify.com/authorize" +
            $"?client_id={ClientId}" +
            $"&response_type=code" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            $"&scope=playlist-read-private%20playlist-read-collaborative%20playlist-modify-public%20playlist-modify-private" +
            $"&state={state}" +
            $"&code_challenge_method=S256" +
            $"&code_challenge={codeChallenge}";

        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

        using var listener = new HttpListener();
        listener.Prefixes.Add($"{redirectUri}/");
        listener.Start();

        var ctx = await listener.GetContextAsync();

        string code = ctx.Request.QueryString["code"];
        string returnedState = ctx.Request.QueryString["state"];

        if (returnedState != state)
            throw new Exception("State mismatch");

        byte[] buffer = Encoding.UTF8.GetBytes(
            "<html><body><h2>Login Successful</h2>You may close this tab.</body></html>"
        );

        ctx.Response.ContentType = "text/html";
        ctx.Response.ContentLength64 = buffer.Length;
        await ctx.Response.OutputStream.WriteAsync(buffer);
        ctx.Response.Close();

        listener.Stop();

        return await ExchangeCodeForTokenAsync(code);
    }

    // ----------------------------------------------------------
    // 2. Token exchange (NO client secret)
    // ----------------------------------------------------------
    private async Task<TokenResponse> PostTokenAsync(Dictionary<string, string> form)
    {
        using var client = new HttpClient();

        var response = await client.PostAsync(
            "https://accounts.spotify.com/api/token",
            new FormUrlEncodedContent(form)
        );

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Spotify token error {response.StatusCode}: {body}");

        return JsonConvert.DeserializeObject<TokenResponse>(body);
    }

    public Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        return PostTokenAsync(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "client_id", ClientId },
            { "code_verifier", _codeVerifier }
        });
    }

    // ----------------------------------------------------------
    // 3. Refresh access token (still works without secret)
    // ----------------------------------------------------------
    public Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken)
    {
        return PostTokenAsync(new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", ClientId }
        });
    }
}
