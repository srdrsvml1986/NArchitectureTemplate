using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NArchitectureTemplate.Core.Security.OAuth.Encryption;
public static class TokenEncryption
{
    public static string EncryptToken(string token, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] encrypted = encryptor.TransformFinalBlock(tokenBytes, 0, tokenBytes.Length);

        byte[] result = new byte[aes.IV.Length + encrypted.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

        return Convert.ToBase64String(result);
    }
}

