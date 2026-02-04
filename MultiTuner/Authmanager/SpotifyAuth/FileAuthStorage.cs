using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public class FileAuthStorage : ISpotifyAuthStorage
{
    private readonly string _filePath;

    public FileAuthStorage()
    {
        // 1. Get the path to C:\Users\Username\AppData\Local
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // 2. Combine it with your App Name
        string folderPath = Path.Combine(appData, "MyMediaApp");

        // 3. Create the folder if it doesn't exist yet
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 4. Set the final file path
        _filePath = Path.Combine(folderPath, "spotify_refresh_token.dat");
    }

    public void SaveRefreshToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var encrypted = ProtectedData.Protect(
            bytes,
            null,
            DataProtectionScope.CurrentUser
        );

        File.WriteAllBytes(_filePath, encrypted);
    }

    public string LoadRefreshToken()
    {
        if (!File.Exists(_filePath))
            return null;

        var encrypted = File.ReadAllBytes(_filePath);

        var decrypted = ProtectedData.Unprotect(
            encrypted,
            null,
            DataProtectionScope.CurrentUser
        );

        return Encoding.UTF8.GetString(decrypted);
    }
}
