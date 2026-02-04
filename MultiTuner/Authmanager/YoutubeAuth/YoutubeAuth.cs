using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MultiTuner.Authmanager.EncryptionManager.AesCrypto;
using System.Diagnostics;


public class YoutubeAuth
{
    private readonly string _resourceName; // Embedded encrypted resource
    private readonly string _userId;
    private readonly byte[] _key; // 32-byte AES key (Base64-decoded)
    private readonly byte[] _iv;  // 16-byte AES IV (Base64-decoded)

    public YoutubeAuth(string resourceName, string base64Key, string base64Iv, string userId = "default_user")
    {
        _resourceName = resourceName;
        _userId = userId;
        _key = Convert.FromBase64String(base64Key);
        _iv = Convert.FromBase64String(base64Iv);
    }

    public async Task<UserCredential> GetCredentialAsync()
    {
        // Load encrypted resource and decrypt in memory
        using var stream = EmbeddedCredentialLoader.LoadEncryptedResource(_resourceName, _key, _iv);

        // Pass decrypted stream to Google SDK
        var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets,
            new[] { YouTubeService.Scope.Youtube },
            _userId,
            CancellationToken.None,
            new FileDataStore("Youtube.Auth.Store")
        );

        Debug.WriteLine("Authentication successful!");
        return credential;
    }
}