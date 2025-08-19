namespace Application.Services;

public interface IEncryptionService
{
    string Decrypt(string cipherText);
    string Encrypt(string plainText);
}