using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

/// <summary>
/// bu sınıf, metinleri AES algoritması ile şifrelemek ve şifrelenmiş metinleri çözmek için kullanılır.
/// bu sınıf, şifreleme anahtarını SHA256 algoritması ile türetilmiş 32 baytlık bir dizi olarak saklar.
/// SecureLocalSecrets için kullanılabilir.
/// </summary>
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _encryptionKey;

    public EncryptionService(string baseKey)
    {
        using var sha = SHA256.Create();
        _encryptionKey = sha.ComputeHash(Encoding.UTF8.GetBytes(baseKey))[..32];
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        var iv = aes.IV;
        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(iv, 0, iv.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        var iv = new byte[16];
        Array.Copy(buffer, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}