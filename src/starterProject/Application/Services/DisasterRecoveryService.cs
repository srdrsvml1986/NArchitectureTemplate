using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Security.Cryptography;

namespace Application.Services;

public class DisasterRecoveryService
{
    private readonly string _backupDirectory;
    private readonly ILogger<DisasterRecoveryService> _logger;
    private readonly string _backupEncryptionKey;

    public DisasterRecoveryService(
        IConfiguration configuration,
        ILogger<DisasterRecoveryService> logger)
    {
        _backupDirectory = configuration["Backup:Directory"] ?? "C:\\App";
        _backupEncryptionKey = configuration["Backup:EncryptionKey"];
        _logger = logger;
        Directory.CreateDirectory(_backupDirectory);
        if (string.IsNullOrEmpty(_backupEncryptionKey))
        {
            _backupEncryptionKey = "fixed-production-backup-key-2024"; 
        }
    }

    public async Task<bool> CreateBackupAsync(string secretsFilePath)
    {
        string tempBackupPath = null;
        try
        {
            var backupFileName = $"secrets_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.dat";
            var backupPath = Path.Combine(_backupDirectory, backupFileName);

            // Geçici dosya oluştur
            tempBackupPath = Path.GetTempFileName();
            File.Copy(secretsFilePath, tempBackupPath, true);

            // Backup'ı şifrele
            await EncryptBackupAsync(tempBackupPath, backupPath);

            // Dosya izinlerini ayarla
            SetFilePermissions(backupPath);

            _logger.LogInformation($"Backup oluşturuldu: {backupPath}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup oluşturulamadı");
            return false;
        }
        finally
        {
            // Geçici dosyayı temizle
            if (tempBackupPath != null && File.Exists(tempBackupPath))
            {
                File.Delete(tempBackupPath);
            }
        }
    }

    public async Task<bool> RestoreFromBackupAsync(string backupFilePath, string targetFilePath)
    {
        string tempBackupPath = null;
        string tempRestorePath = null;

        try
        {
            if (!File.Exists(backupFilePath))
            {
                _logger.LogError("Backup dosyası bulunamadı: {BackupPath}", backupFilePath);
                return false;
            }

            // Geçici decrypt dosyası oluştur
            tempRestorePath = Path.GetTempFileName();

            // Backup'ı decrypt et
            await DecryptBackupAsync(backupFilePath, tempRestorePath);

            // Hedef dosyayı yedekle (varsa)
            if (File.Exists(targetFilePath))
            {
                tempBackupPath = targetFilePath + $".old_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
                File.Move(targetFilePath, tempBackupPath, true);
            }

            // Backup'ı geri yükle
            File.Copy(tempRestorePath, targetFilePath, true);

            // İzinleri yeniden ayarla
            SetFilePermissions(targetFilePath);

            _logger.LogInformation("Backup başarıyla geri yüklendi");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Backup geri yükleme hatası");

            // Geri yükleme başarısız olursa eski dosyayı geri getir
            if (tempBackupPath != null && File.Exists(tempBackupPath))
            {
                File.Move(tempBackupPath, targetFilePath, true);
            }

            return false;
        }
        finally
        {
            // Geçici dosyaları temizle
            if (tempRestorePath != null && File.Exists(tempRestorePath))
            {
                File.Delete(tempRestorePath);
            }
        }
    }

    private async Task EncryptBackupAsync(string sourcePath, string destinationPath)
    {
        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
        using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
        using (var aes = Aes.Create())
        {
            aes.Key = DeriveKeyFromPassword(_backupEncryptionKey);
            aes.IV = GenerateRandomBytes(16);

            // IV'yi dosyanın başına yaz
            await destinationStream.WriteAsync(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(
                destinationStream,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write))
            {
                await sourceStream.CopyToAsync(cryptoStream);
            }
        }
    }

    private async Task DecryptBackupAsync(string sourcePath, string destinationPath)
    {
        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
        using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
        using (var aes = Aes.Create())
        {
            // IV'yi dosyanın başından oku
            var iv = new byte[16];
            await sourceStream.ReadAsync(iv, 0, iv.Length);

            aes.Key = DeriveKeyFromPassword(_backupEncryptionKey);
            aes.IV = iv;

            using (var cryptoStream = new CryptoStream(
                sourceStream,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read))
            {
                await cryptoStream.CopyToAsync(destinationStream);
            }
        }
    }

    private byte[] DeriveKeyFromPassword(string password)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(
            password,
            Encoding.UTF8.GetBytes("FixedSaltForBackup"),
            10000,
            HashAlgorithmName.SHA256))
        {
            return deriveBytes.GetBytes(32); // 256-bit key
        }
    }

    private byte[] GenerateRandomBytes(int length)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return bytes;
        }
    }

    private void SetFilePermissions(string filePath)
    {
        //try
        //{
        //    if (!File.Exists(filePath)) return;

        //    var fileInfo = new FileInfo(filePath);
        //    var fileSecurity = fileInfo.GetAccessControl();

        //    // Tüm miras alınan izinleri kaldır
        //    fileSecurity.SetAccessRuleProtection(true, false);

        //    // Yöneticilere tam kontrol izni ver
        //    var adminRule = new System.Security.AccessControl.FileSystemAccessRule(
        //        "BUILTIN\\Administrators",
        //        System.Security.AccessControl.FileSystemRights.FullControl,
        //        System.Security.AccessControl.AccessControlType.Allow);
        //    fileSecurity.AddAccessRule(adminRule);

        //    // SYSTEM hesabına tam kontrol izni ver
        //    var systemRule = new System.Security.AccessControl.FileSystemAccessRule(
        //        "SYSTEM",
        //        System.Security.AccessControl.FileSystemRights.FullControl,
        //        System.Security.AccessControl.AccessControlType.Allow);
        //    fileSecurity.AddAccessRule(systemRule);

        //    // Uygulama kullanıcısına okuma/yazma izni ver
        //    var appUserRule = new System.Security.AccessControl.FileSystemAccessRule(
        //        "IIS_APPPOOL\\MyAppPool",
        //        System.Security.AccessControl.FileSystemRights.Read | System.Security.AccessControl.FileSystemRights.Write,
        //        System.Security.AccessControl.AccessControlType.Allow);
        //    fileSecurity.AddAccessRule(appUserRule);

        //    fileInfo.SetAccessControl(fileSecurity);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogWarning(ex, "Dosya izinleri ayarlanamadı: {FilePath}", filePath);
        //}
    }

    private string GenerateBackupEncryptionKey()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public string[] GetAvailableBackups()
    {
        return Directory.GetFiles(_backupDirectory, "secrets_backup_*.dat")
            .OrderByDescending(f => f)
            .ToArray();
    }

    public void CleanOldBackups(int keepLast = 7)
    {
        var backups = GetAvailableBackups();
        if (backups.Length > keepLast)
        {
            foreach (var oldBackup in backups.Skip(keepLast))
            {
                try
                {
                    File.Delete(oldBackup);
                    _logger.LogInformation("Eski backup silindi: {BackupPath}", oldBackup);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Backup silinemedi: {BackupPath}", oldBackup);
                }
            }
        }
    }
}