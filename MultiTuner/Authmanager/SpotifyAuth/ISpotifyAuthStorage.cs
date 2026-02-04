


public interface ISpotifyAuthStorage
{
    void SaveRefreshToken(string token);
    string LoadRefreshToken();
}
