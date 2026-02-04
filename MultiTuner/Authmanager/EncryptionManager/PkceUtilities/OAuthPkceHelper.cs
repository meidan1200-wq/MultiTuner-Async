using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MultiTuner.Authmanager.EncryptionManager.PkceUtilities
{
    public class OAuthPkceHelper
    {

        public static string GenerateCodeVerifier()
        {
            byte[] bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Base64UrlEncode(bytes);
        }

        public static string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }





    }
}
