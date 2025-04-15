using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace NArchitecture.Core.Security.Encryption;

public static class EncryptionHelper
{
    private static string _key;

    public static void Initialize(IConfiguration configuration)
    {
        _key = configuration["Security:EncryptionKey"]
            ?? throw new InvalidOperationException("Encryption key not found in configuration");
    }

    public static string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] encrypted = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

            byte[] result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }
        catch (Exception ex)
        {
            throw new Exception("Encryption failed", ex);
        }
    }

    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return string.Empty;

        try
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32));

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception ex)
        {
            throw new Exception("Decryption failed", ex);
        }
    }
}
