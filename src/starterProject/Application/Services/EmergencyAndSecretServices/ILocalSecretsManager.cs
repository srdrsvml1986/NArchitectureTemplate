namespace Application.Services.EmergencyAndSecretServices;

public interface ILocalSecretsManager
{
    void SetSecret(string key, string value);
    string GetSecret(string key);
    void UpdateSecret(string key, string newValue);
    void DeleteSecret(string key);
    Dictionary<string, string> GetAllSecrets();
    bool SecretExists(string key);
}