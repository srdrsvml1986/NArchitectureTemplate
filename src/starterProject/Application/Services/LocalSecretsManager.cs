using System.Text.Json;

namespace Application.Services;
public class LocalSecretsManager : ILocalSecretsManager
{
    private const string SecretsFile = "secrets.dat";
    private readonly EncryptionService _encryptor;
    private Dictionary<string, string> _secrets;

    public LocalSecretsManager(string masterKey)
    {
        _encryptor = new EncryptionService(masterKey);
        _secrets = File.Exists(SecretsFile) ?
            LoadSecrets() : new Dictionary<string, string>();
    }

    public void SetSecret(string key, string value)
    {
        _secrets[key] = value;
        SaveSecrets();
    }

    public string GetSecret(string key)
    {
        return _secrets.TryGetValue(key, out var value) ? value : null;
    }

    public void UpdateSecret(string key, string newValue)
    {
        if (!_secrets.ContainsKey(key))
            throw new KeyNotFoundException($"Secret key '{key}' not found");

        _secrets[key] = newValue;
        SaveSecrets();
    }

    public void DeleteSecret(string key)
    {
        if (!_secrets.Remove(key))
            throw new KeyNotFoundException($"Secret key '{key}' not found");

        SaveSecrets();
    }

    public Dictionary<string, string> GetAllSecrets()
    {
        return new Dictionary<string, string>(_secrets);
    }

    public bool SecretExists(string key)
    {
        return _secrets.ContainsKey(key);
    }

    private Dictionary<string, string> LoadSecrets()
    {
        var encrypted = File.ReadAllText(SecretsFile);
        var json = _encryptor.Decrypt(encrypted);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }

    private void SaveSecrets()
    {
        var json = JsonSerializer.Serialize(_secrets);
        var encrypted = _encryptor.Encrypt(json);
        File.WriteAllText(SecretsFile, encrypted);
    }
}