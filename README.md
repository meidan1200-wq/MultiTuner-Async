# Playlist Transfer Application

## Overview

This application allows users to transfer playlists between music platforms.

**Supported platforms:**
- Spotify
- YouTube

Playlists can be moved in both directions. Support for additional platforms is planned.

---

## API Model

Because Spotify and YouTube require strict approval processes, the app uses a **per-user API model**.

Users can either:
- Run the app with **their own API credentials**, or
- Request access to **my API credentials** and use the app without setup.

---

## Usage

### Option 1: Use My API (Simple)

1. Download and run the `.exe` file.
2. Request access to my Spotify and/or YouTube API apps.
3. Authenticate and use the app.

**Note:**
Unverified or unauthorized app warnings may appear. This is expected and caused by platform policies.

---

### Option 2: Use Your Own API (Advanced)

#### Spotify

1. Create a Spotify Developer App.
2. Get your **Client ID**.
3. Replace it in `SpotifyAPI.cs`.
4. Build and run.

#### YouTube

1. Create a Google Cloud project and enable the YouTube Data API.
2. Create OAuth desktop credentials.
3. Encrypt the credentials JSON using `AesCrypto`.
4. Replace the embedded file `Youtube.Credentials.enc`.

The app requires encrypted credentials and will not accept raw JSON.

---

## Limitations

- Only Spotify and YouTube are supported.
- API rate limits apply.
- Platform verification warnings may appear.

---

## License

This project is free to use as source code and as a compiled application.

You may:
- Use, modify, and distribute the app

Conditions:
- Credit must be given to the original author.
- You may not claim authorship of the project.
- Public promotion or large-scale distribution requires attribution or prior approval.

Misrepresentation or unauthorized rebranding is not permitted.

