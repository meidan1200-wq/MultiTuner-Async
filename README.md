<div align="center">

<img width="auto" height="579" alt="Image" src="https://github.com/user-attachments/assets/f279de93-cc63-4071-bdda-fab9459f9435" />

<br />
<br />

<a href="https://spotify.com">
  <img src="https://img.shields.io/badge/Spotify-1ED760?style=for-the-badge&logo=spotify&logoColor=white" alt="Spotify" />
</a>
<a href="https://youtube.com">
  <img src="https://img.shields.io/badge/YouTube-FF0000?style=for-the-badge&logo=youtube&logoColor=white" alt="YouTube" />
</a>

<br />
<br />

<img width="2208" height="1810" alt="Music Transfer App Interface" src="https://github.com/user-attachments/assets/fb58841a-e7df-46f8-b6ec-3d92bf807cad" style="border-radius: 12px; border: 1px solid #333; box-shadow: 0px 10px 40px rgba(0,0,0,0.7);" />

<br />
<br />

<p align="left">
  <strong>Playlist Transfer Application</strong> allows users to transfer playlists between music platforms with a sleek, dark-mode interface. Currently supporting Spotify and YouTube with bi-directional syncing.
</p>

</div>

---

## ⚡ Features

* **Bi-Directional Sync:** Move playlists from Spotify to YouTube and vice versa.
* **User-Friendly Interface:** Visual album art and tracklists as seen in the interface.
* **Flexible API Model:** Choose between pre-configured credentials or personal API keys.

---

## 🛡️ API Model

Because Spotify and YouTube maintain strict approval processes for third-party applications, this project utilizes a **per-user API model**.

> [!IMPORTANT]
> You may encounter "Unverified App" warnings during login. This is expected behavior due to platform policies regarding open-source verification.

**Operational Modes:**
1.  **Quick Start:** Request access to the author's credentials to skip manual setup.
2.  **Developer Mode:** Run the application using your own independent API credentials.

---

## 🚀 Usage

### Option 1: Quick Start (Use My API)
*Recommended for most users.*

1.  Download and run the `.exe` file.
2.  Request access to the hosted Spotify and/or YouTube API apps.
3.  Authenticate within the app to begin transferring.

### Option 2: Advanced (Bring Your Own API)
*Recommended for developers.*

#### 🟢 For Spotify
1.  Create a **Spotify Developer App**.
2.  Obtain your **Client ID**.
3.  Replace the ID in `SpotifyAPI.cs`.
4.  Build and run the project.

#### 🔴 For YouTube
1.  Create a **Google Cloud Project** and enable the **YouTube Data API**.
2.  Create **OAuth desktop credentials**.
3.  Encrypt the credentials JSON using `AesCrypto`.
4.  Replace the embedded file `Youtube.Credentials.enc`.

*Note: The app requires encrypted credentials for security and will not accept raw JSON files.*

---

## ⚠️ Limitations

* **Supported Platforms:** Currently limited to Spotify and YouTube.
* **Rate Limiting:** Standard API rate limits apply based on the platform provider.
* **Platform Warnings:** Verification warnings may appear during the authentication process.

---

## ⚖️ License

This project is free to use as source code and as a compiled application.

**Conditions of Use:**
* ✅ You may use, modify, and distribute the app.
* 🏷️ **Credit must be given** to the original author.
* ❌ You may not claim authorship or use the project for unauthorized rebranding.
* 📢 Public promotion or large-scale distribution requires prior approval.

---

<div align="center">
  <sub>Developed for seamless music migration</sub>
</div>
