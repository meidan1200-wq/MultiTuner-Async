using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace MultiTuner.Authmanager.EncryptionManager.AesCrypto
{
    public static class EmbeddedCredentialLoader
    {
        public static Stream LoadEncryptedResource(string resourceName, byte[] key, byte[] iv)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var encryptedStream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new Exception("Embedded resource not found");

            using var ms = new MemoryStream();
            encryptedStream.CopyTo(ms);

            var decrypted = AesCrypto.Decrypt(ms.ToArray(), key, iv);

            return new MemoryStream(decrypted); // caller owns stream
        }
    }
}
