using System.Security.Cryptography;
using System.Text;

namespace PayLi.API.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptPin(string pin, string secretKey)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    // Generate key from secret key
                    var key = GenerateKeyFromString(secretKey);
                    aes.Key = key;
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var msEncrypt = new MemoryStream())
                    {
                        // Prepend IV to the encrypted data
                        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(pin);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to encrypt PIN", ex);
            }
        }

        public string DecryptPin(string encryptedPin, string secretKey)
        {
            try
            {
                var encryptedData = Convert.FromBase64String(encryptedPin);

                using (var aes = Aes.Create())
                {
                    var key = GenerateKeyFromString(secretKey);
                    aes.Key = key;

                    // Extract IV from the beginning of encrypted data
                    var iv = new byte[aes.BlockSize / 8];
                    Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var msDecrypt = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt PIN", ex);
            }
        }

        private byte[] GenerateKeyFromString(string secretKey)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
            }
        }
    }
}
