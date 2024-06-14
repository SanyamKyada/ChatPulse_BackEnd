using CP.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CP.Services.Implementations
{
    public class EncryptionService : IEncryptionService
    {
        private IConfiguration _config;

        public EncryptionService()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
        public string Decrypt(string encryptedData)
        {
            try
            {
                var encSettings = _config.GetSection("EncryptionSettings").Get<EncryptionSettings>();//.Get<EncryptionSettings>();
                var key = Encoding.UTF8.GetBytes(encSettings.AESKey);
                var fullCipher = Convert.FromBase64String(encryptedData);

                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.Mode = CipherMode.CBC; // Set cipher mode to CBC
                    aesAlg.Padding = PaddingMode.PKCS7; // Set padding mode to PKCS7
                    aesAlg.IV = fullCipher.Take(16).ToArray(); // Extract IV from the encrypted data

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        using (var msDecrypt = new MemoryStream(fullCipher.Skip(16).ToArray()))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    return srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }

    public class EncryptionSettings
    {
        public string AESKey { get; set; }
    }
}
