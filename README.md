<div align="center">

# 🎵 Playlist Transfer Application
### Seamlessly migrate your music between platforms.

<a href="https://spotify.com">
  <img src="https://img.shields.io/badge/Spotify-1ED760?style=for-the-badge&logo=spotify&logoColor=white" alt="Spotify" />
</a>
<a href="https://youtube.com">
  <img src="https://img.shields.io/badge/YouTube-FF0000?style=for-the-badge&logo=youtube&logoColor=white" alt="YouTube" />
</a>
<a href="#">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#" />
</a>

<br />
<br />

<img src="./Screenshot of the app.jpg" alt="Music Transfer App Interface" width="100%" style="border-radius: 10px; box-shadow: 0px 0px 20px rgba(0,0,0,0.5);" />

<br />

<p align="left">
  <strong>Playlist Transfer Application</strong> allows users to transfer playlists between music platforms with a sleek, dark-mode interface. Currently supporting Spotify and YouTube with bi-directional syncing.
</p>

</div>

---

## ⚡ Features

* **Bi-Directional Sync:** Move playlists from Spotify to YouTube and vice versa.
* **User-Friendly Interface:** Visual album art and tracklists (as seen in screenshot).
* **Flexible API Model:** Use pre-configured credentials or bring your own keys.

---

## ⚙️ API Model

Because Spotify and YouTube maintain strict approval processes for third-party applications, this project utilizes a **per-user API model**.

> **Note:** You may encounter "Unverified App" warnings during login. This is expected behavior due to platform policies regarding open-source verification.

You have two modes of operation:
1.  **Quick Start:** Request access to the author's credentials.
2.  **Developer Mode:** Run with your own API credentials.

---

## 🚀 Usage

### Option 1: Quick Start (Use My API)
*Recommended for most users.*

1.  Download and run the `.exe` file.
2.  Request access to the hosted Spotify/YouTube API apps.
3.  Authenticate within the app to begin transferring.

### Option 2: Advanced (Bring Your Own API)
*Recommended for developers.*

#### 🟢 For Spotify
1.  Create a **Spotify Developer App**.
2.  Obtain your `Client ID`.
3.  Replace the ID in `SpotifyAPI.cs`.
4.  Build and run.

#### 🔴 For YouTube
1.  Create a **Google Cloud Project** and enable the **YouTube Data API**.
2.  Create **OAuth desktop credentials**.
3.  Encrypt the credentials JSON using `AesCrypto`.
4.  Replace the embedded file `Youtube.Credentials.enc`.

> **Important:** The app requires encrypted credentials for security and will not accept raw JSON files.

---

## ⚠️ Limitations

* **Supported Platforms:** Currently limited to Spotify and YouTube.
* **Rate Limiting:** Standard API rate limits apply based on the platform.

---

## ⚖️ License

This project is free to use as source code and as a compiled application.

**Conditions:**
* ✅ You may use, modify, and distribute the app.
* 🏷️ **Credit must be given** to the original author.
* ❌ You may not claim authorship or use for unauthorized rebranding.
* 📢 Public promotion or large-scale distribution requires prior approval.
