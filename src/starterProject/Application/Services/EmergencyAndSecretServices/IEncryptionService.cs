namespace Application.Services.EmergencyAndSecretServices;

public interface IEncryptionService
{
    string Decrypt(string cipherText);
    string Encrypt(string plainText);
}