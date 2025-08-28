# NArchitectureTemplate API Uygulaması Kullanım Kılavuzu

## 1. Giriş

Bu kılavuz, NArchitectureTemplate projesinin API uygulamasının kurulumu, yapılandırması ve kullanımı hakkında detaylı bilgi sağlamaktadır. Proje, temiz mimari prensiplerine uygun olarak geliştirilmiş olup, kimlik doğrulama (Authentication), yetkilendirme (Authorization), kullanıcı yönetimi ve sır yönetimi (Secrets Management) gibi temel özellikleri içermektedir.

Amacı, .NET Core tabanlı API projeleri için sağlam, ölçeklenebilir ve bakımı kolay bir başlangıç noktası sunmaktır.




## 2. Proje Yapısı

NArchitectureTemplate projesi, temiz mimari prensiplerine uygun olarak aşağıdaki ana katmanlardan oluşmaktadır:

- **Domain:** İş varlıklarını (entities), değer nesnelerini (value objects) ve iş kurallarını (business rules) içerir. Projenin çekirdeğini oluşturur ve diğer katmanlardan bağımsızdır.
- **Application:** Uygulama katmanı, iş akışlarını ve kullanım senaryolarını (use cases) tanımlar. Komutlar (commands), sorgular (queries), işleyiciler (handlers) ve uygulama servisleri bu katmanda yer alır. Domain katmanına bağımlıdır.
- **Persistence:** Veri erişim katmanı olup, veritabanı işlemleri (CRUD operasyonları), repository uygulamaları ve ORM (Object-Relational Mapping) konfigürasyonlarını içerir. Domain katmanına bağımlıdır.
- **Infrastructure:** Harici servis entegrasyonları (e-posta, SMS, loglama, cache vb.) ve altyapısal bileşenleri barındırır. Application ve Persistence katmanlarına bağımlıdır.
- **WebAPI:** API endpoint'lerini, HTTP isteklerini işleyen controller'ları ve uygulamanın dış dünyaya açılan arayüzünü içerir. Diğer tüm katmanlara bağımlıdır ve bağımlılık enjeksiyonu (dependency injection) burada yapılandırılır.

Proje dizin yapısı aşağıdaki gibidir:

```
NArchitectureTemplate/
├── src/
│   ├── core/ (Temel mimari bileşenleri)
│   └── starterProject/ (Ana uygulama projesi)
│       ├── Application/
│       ├── Domain/
│       ├── Infrastructure/
│       ├── Persistence/
│       └── WebAPI/
│           ├── Configrations/
│           ├── Controllers/
│           ├── Extensions/
│           ├── Middleware/
│           ├── Properties/
│           ├── appsettings.json
│           ├── appsettings.Development.json
│           ├── appsettings.Production.json
│           ├── appsettings.Staging.json
│           └── Program.cs
├── tests/
├── docs/
├── LICENSE
├── NArchitectureTemplate.sln
└── README.md
```

Her bir proje, kendi sorumluluk alanına göre ayrılmıştır. Özellikle `WebAPI` projesi, API endpoint'lerini ve uygulamanın dış arayüzünü barındırır. `appsettings.json` dosyaları, uygulamanın farklı ortamlar için yapılandırma ayarlarını içerir.




## 3. MASTER_KEY Kurulumu ve Kullanımı

NArchitectureTemplate projesi, hassas bilgilerin (veritabanı bağlantı dizeleri, API anahtarları, şifreler vb.) güvenli bir şekilde yönetilmesi için özel bir "Secrets Management" yapısı kullanır. Bu yapının temelini **MASTER_KEY** oluşturur. MASTER_KEY, sırların şifrelenmesi ve şifresinin çözülmesi için kullanılan ana anahtardır. Bu anahtarın güvenliği, uygulamanızdaki tüm hassas verilerin güvenliği için kritik öneme sahiptir.

### 3.1. MASTER_KEY Nedir ve Neden Önemlidir?

MASTER_KEY, uygulamanızın sırlarını (API anahtarları, veritabanı şifreleri vb.) şifrelemek ve şifresini çözmek için kullanılan bir anahtardır. Bu anahtarın güvenli bir şekilde saklanması ve yönetilmesi, uygulamanızın genel güvenlik duruşu için hayati önem taşır. Eğer MASTER_KEY ele geçirilirse, şifrelenmiş tüm sırlar açığa çıkabilir.

**Önemli Notlar:**

*   **Üretim Ortamı:** Üretim ortamında `MASTER_KEY` ortam değişkeninin güvenli bir şekilde ayarlandığından emin olun. Bu anahtarın sızdırılması, tüm sırların güvenliğini tehlikeye atabilir.
*   **Geliştirme Ortamı:** Geliştirme ortamında (ASPNETCORE_ENVIRONMENT=Development veya Staging) varsayılan bir anahtar kullanılır. Ancak gerçek hassas verileri bu ortamda kullanmaktan kaçının veya kendi güvenli anahtarlarınızı yapılandırın.

### 3.2. MASTER_KEY Nasıl Oluşturulur?

MASTER_KEY, en az 32 karakter uzunluğunda, rastgele ve karmaşık bir dize olmalıdır. Güvenli bir anahtar oluşturmak için aşağıdaki yöntemleri kullanabilirsiniz:

**Örnek Güçlü MASTER_KEY:**

```
my-super-secret-master-key-for-narchitecture-template-2025-random-string-xyz123ABC
```

**Python ile Rastgele Anahtar Oluşturma (Önerilen):**

```python
import os
import base64

# 32 bayt (256 bit) rastgele anahtar oluştur
master_key = base64.urlsafe_b64encode(os.urandom(32)).decode("utf-8")
print(master_key)
```

Bu Python kodu, 32 bayt uzunluğunda rastgele bir anahtar oluşturur ve Base64 ile kodlayarak güvenli bir dizeye dönüştürür. Bu dizeyi MASTER_KEY olarak kullanabilirsiniz.

### 3.3. MASTER_KEY Ortam Değişkeni Olarak Nasıl Ayarlanır?

MASTER_KEY, uygulamanın çalıştığı ortamda bir ortam değişkeni olarak ayarlanmalıdır. İşletim sisteminize göre farklı yöntemler bulunmaktadır:

#### Windows

**Geçici (Sadece mevcut komut istemcisi oturumu için):**

```bash
set MASTER_KEY=my-super-secret-master-key-for-narchitecture-template-2025-random-string-xyz123ABC
```

**Kalıcı (Sistem genelinde):**

1.  "Denetim Masası"nı açın.
2.  "Sistem ve Güvenlik" > "Sistem"e gidin.
3.  Sol menüden "Gelişmiş sistem ayarları"nı tıklayın.
4.  Açılan pencerede "Ortam Değişkenleri..." düğmesine tıklayın.
5.  "Sistem değişkenleri" bölümünde "Yeni..." düğmesine tıklayın.
6.  Değişken adı olarak `MASTER_KEY`, değişken değeri olarak oluşturduğunuz anahtarı girin ve "Tamam"a tıklayın.

#### Linux / macOS

**Geçici (Sadece mevcut terminal oturumu için):**

```bash
export MASTER_KEY=my-super-secret-master-key-for-narchitecture-template-2025-random-string-xyz123ABC
```

**Kalıcı (Sistem genelinde - .bashrc, .zshrc veya .profile dosyasına ekleyerek):**

1.  Tercih ettiğiniz bir metin düzenleyici ile `~/.bashrc`, `~/.zshrc` veya `~/.profile` dosyasını açın (kullandığınız kabuğa göre değişir).
2.  Dosyanın sonuna aşağıdaki satırı ekleyin:
    ```bash
    export MASTER_KEY="my-super-secret-master-key-for-narchitecture-template-2025-random-string-xyz123ABC"
    ```
3.  Dosyayı kaydedin ve kapatın.
4.  Değişikliklerin etkili olması için terminali yeniden başlatın veya aşağıdaki komutu çalıştırın:
    ```bash
    source ~/.bashrc  # veya ~/.zshrc, ~/.profile
    ```

#### Docker / Docker Compose

Docker kullanıyorsanız, `Dockerfile` veya `docker-compose.yml` dosyanızda ortam değişkenini ayarlayabilirsiniz.

**Dockerfile:**

```dockerfile
ENV MASTER_KEY="my-super-secret-master-key-for-narchitecture-template-2025-random-string-xyz123ABC"
```

**docker-compose.yml:**

```yaml
services:
  webapi:
    environment:
      - MASTER_KEY=my-super-secret-master-key-for-narchitecture-template-2025-random-string-xyz123ABC
```

Bu adımları takip ederek MASTER_KEY'i güvenli bir şekilde yapılandırabilir ve uygulamanızın sır yönetimini sağlayabilirsiniz.




## 5. Servislerin Detaylı İncelenmesi ve Kullanım Örnekleri

NArchitectureTemplate projesi, çeşitli işlevleri yerine getirmek için farklı servisler kullanır. Bu bölümde, projenin önemli servisleri detaylı olarak açıklanacak ve kullanım örnekleri sunulacaktır.

### 5.1. EmergencyAndSecretServices

`EmergencyAndSecretServices` modülü, uygulamanın güvenlik, acil durum yönetimi ve sır yönetimi gibi kritik altyapısal ihtiyaçlarını karşılamak üzere tasarlanmıştır. Bu modül altında çeşitli servisler bulunmaktadır:

#### 5.1.1. AuditService

`AuditService`, hassas işlemlere (özellikle sır erişimlerine) yönelik denetim kayıtlarını tutar. Bu sayede, kimin ne zaman hangi sırra eriştiği veya hangi güvenlik eylemini gerçekleştirdiği izlenebilir.

**Konum:** `Application/Services/EmergencyAndSecretServices/AuditService.cs`

**Fonksiyonellik:**
*   `LogAccess(string key, string action, string user, string ipAddress, string details)`: Belirtilen anahtar, eylem, kullanıcı, IP adresi ve detaylarla bir erişim kaydı oluşturur ve `C:\App\Logs\Security\secrets_audit.log` dosyasına yazar.
*   `GetRecentAuditLogs(int maxEntries)`: En son denetim kayıtlarını okur.

**Kullanım Örneği (Dahili):**

`AuditService`, genellikle `LocalSecretsManager` gibi hassas verilere erişen veya güvenlik denetimi gerektiren diğer servisler tarafından kullanılır. Örneğin, bir sırra erişildiğinde veya bir güvenlik ayarı değiştirildiğinde bu servis aracılığıyla loglama yapılır.

```csharp
// AuditService'in enjekte edildiği bir yerde (örneğin LocalSecretsManager içinde)
_auditService.LogAccess("TokenOptions:SecurityKey", "Read", "System", "127.0.0.1", "JWT Security Key okundu");
```




#### 5.1.2. BackupService

`BackupService`, uygulamanın kritik verilerini (özellikle sırları) düzenli aralıklarla yedeklemek için tasarlanmış bir arka plan servisidir. Bu, olası veri kayıplarına karşı bir koruma katmanı sağlar.

**Konum:** `Application/Services/EmergencyAndSecretServices/BackupService.cs`

**Fonksiyonellik:**
*   Belirli aralıklarla (varsayılan olarak 6 saatte bir) `DisasterRecoveryService` kullanarak yedekleme yapar.
*   Eski yedekleri (varsayılan olarak 30 günden eski) temizler.

**Kullanım Örneği (Dahili):**

`BackupService`, bir `BackgroundService` olarak implemente edilmiştir ve uygulama başlatıldığında otomatik olarak çalışır. Yedekleme aralığı ve temizleme politikası gibi ayarlar `appsettings.json` veya ilgili konfigürasyon dosyalarından yönetilebilir.

```csharp
// Program.cs içinde servis kaydı (örnek)
builder.Services.AddHostedService<BackupService>();

// appsettings.json içinde ilgili ayarlar (örnek)
"Backup": {
  "Directory": "c:\\app\\backups",
  "EncryptionKey": "", // LocalSecretsManager tarafından doldurulacak
  "RetentionDays": 30
}
```




#### 5.1.3. DisasterRecoveryService

`DisasterRecoveryService`, uygulamanın kritik verilerinin (özellikle sırların) yedeklenmesi ve geri yüklenmesi işlemlerini yönetir. Bu servis, veri kaybı durumlarında hızlı bir şekilde kurtarma sağlamak için şifreli yedeklemeler oluşturur ve yönetir.

**Konum:** `Application/Services/EmergencyAndSecretServices/DisasterRecoveryService.cs`

**Fonksiyonellik:**
*   `CreateBackupAsync(string secretsFilePath)`: Belirtilen dosyanın şifreli bir yedeğini oluşturur ve yedekleme dizinine kaydeder. Yedekleme dosyaları `secrets_backup_yyyyMMdd_HHmmss.dat` formatında isimlendirilir.
*   `RestoreFromBackupAsync(string backupFilePath, string targetFilePath)`: Belirtilen yedekleme dosyasından verileri şifresini çözerek hedef dosyaya geri yükler. Geri yükleme sırasında mevcut hedef dosya yedeklenir.
*   `CleanOldBackups(int keepLast)`: Belirtilen sayıdan daha eski yedekleme dosyalarını temizler.
*   Yedekleme ve geri yükleme işlemleri için `_backupEncryptionKey` kullanılarak AES şifrelemesi yapılır. Bu anahtar `LocalSecretsManager`'dan alınır.
*   Dosya izinlerini işletim sistemine göre ayarlar (Windows ve Linux/macOS için farklı).

**Kullanım Örneği (Dahili):**

`DisasterRecoveryService`, `BackupService` tarafından düzenli yedeklemeler için kullanılır. Ayrıca, manuel olarak veya bir acil durum senaryosunda veri geri yükleme için de kullanılabilir.

```csharp
// Yedekleme oluşturma (örnek)
// DisasterRecoveryService disasterRecoveryService = ...;
// await disasterRecoveryService.CreateBackupAsync("path/to/your/secrets.json");

// Yedekten geri yükleme (örnek)
// await disasterRecoveryService.RestoreFromBackupAsync("path/to/backup_file.dat", "path/to/target_file.json");

// Eski yedekleri temizleme (örnek)
// disasterRecoveryService.CleanOldBackups(7); // Son 7 yedeği tut
```

**Konfigürasyon:**

`appsettings.json` dosyasında yedekleme dizini ve şifreleme anahtarı ile ilgili ayarlar bulunur:

```json
"Backup": {
  "Directory": "c:\\app",
  "EncryptionKey": "", // Bu alan LocalSecretsManager ile doldurulacak
  "RetentionDays": 30
}
```

`Backup:EncryptionKey` değeri, `MASTER_KEY` tarafından şifrelenmiş olarak `LocalSecretsManager`'da saklanır.




#### 5.1.4. EmergencyAccessService

`EmergencyAccessService`, acil durumlarda sisteme güvenli erişim sağlamak için tasarlanmıştır. Özellikle kritik sistem sırlarına veya işlevlerine erişim gerektiğinde kullanılır. Bu servis, tek kullanımlık veya kısa süreli geçerli token'lar üreterek güvenli erişimi mümkün kılar.

**Konum:** `Application/Services/EmergencyAndSecretServices/EmergencyAccessService.cs`

**Fonksiyonellik:**
*   `ValidateEmergencyToken(string token)`: Verilen acil durum token'ının geçerliliğini (doğru token ve geçerlilik süresi) kontrol eder.
*   `GetSecretsForEmergency(string token, string requesterIp)`: Geçerli bir acil durum token'ı ile kritik sırları (örneğin veritabanı bağlantı dizesi, JWT güvenlik anahtarı) döndürür. Bu erişim `AuditService` tarafından loglanır.
*   `RotateEmergencyToken()`: Yeni bir güvenli acil durum token'ı oluşturur ve bu token'ı belirlenen alıcılara (e-posta veya SMS ile) dağıtır. Token'ın geçerlilik süresi 4 saattir.
*   `DistributeEmergencyToken(string newToken, DateTime validUntil)`: Oluşturulan acil durum token'ını e-posta veya SMS yoluyla `Emergency:NotificationRecipients` konfigürasyonunda belirtilen alıcılara gönderir.
*   `ValidateAndExtendTokenAsync(string token, string requesterInfo)`: Mevcut bir acil durum token'ını doğrular ve geçerliyse süresini uzatır.

**Kullanım Örneği:**

Acil durum erişimi genellikle `WebAPI/Middleware/EmergencyMonitoringMiddleware.cs` gibi özel middleware'ler aracılığıyla tetiklenir. Bu middleware, belirli bir HTTP başlığı (örneğin `X-Emergency-Access`) kontrol ederek acil durum modunu etkinleştirebilir ve `EmergencyAccessService`'i kullanarak kritik bilgilere erişim sağlayabilir.

**Konfigürasyon (`appsettings.json`):**

```json
"Emergency": {
  "NotificationRecipients": "admin@example.com, +905xxxxxxxxx", // E-posta ve/veya telefon numaraları
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUser": "your_email@example.com",
  "SmtpPass": "your_email_password", // LocalSecretsManager tarafından doldurulacak
  "SmsApiUrl": "https://api.sms-provider.com/send", // SMS sağlayıcınızın API URL'si
  "SmsApiKey": "your_sms_api_key" // LocalSecretsManager tarafından doldurulacak
}
```

**Acil Durum Tokenı Oluşturma ve Dağıtma:**

Bu işlem genellikle manuel olarak veya belirli bir tetikleyici ile yapılır. Örneğin, bir sistem yöneticisi bir sorun anında `RotateEmergencyToken()` metodunu çağırarak yeni bir token oluşturabilir ve bu token otomatik olarak tanımlanmış alıcılara gönderilir.

**Örnek Senaryo:**

Bir veritabanı bağlantı sorunu yaşandığında, geliştirici `EmergencyAccessService` tarafından oluşturulan token'ı kullanarak kritik veritabanı bağlantı dizesine erişebilir ve sorunu gidermek için kullanabilir. Bu erişim denetim loglarına kaydedilir.




#### 5.1.5. EncryptionService

`EncryptionService`, metinleri AES algoritması kullanarak şifrelemek ve şifrelenmiş metinleri çözmek için kullanılır. Bu servis, özellikle hassas verilerin depolanması veya iletilmesi sırasında gizliliğini sağlamak için temel bir güvenlik bileşeni olarak görev yapar.

**Konum:** `Application/Services/EmergencyAndSecretServices/EncryptionService.cs`

**Fonksiyonellik:**
*   `Encrypt(string plainText)`: Verilen düz metni AES algoritması ile şifreler ve Base64 kodlu bir dize olarak döndürür.
*   `Decrypt(string cipherText)`: Verilen şifreli metni (Base64 kodlu) AES algoritması ile çözer ve düz metin olarak döndürür.
*   Şifreleme anahtarı (`_encryptionKey`), yapıcı metotta sağlanan `baseKey` (genellikle MASTER_KEY) kullanılarak SHA256 algoritması ile türetilir.

**Kullanım Örneği (Dahili):**

`EncryptionService`, `LocalSecretsManager` gibi sırları güvenli bir şekilde saklayan servisler tarafından dahili olarak kullanılır. Doğrudan uygulama katmanında çağrılması yerine, genellikle daha yüksek seviyeli servisler aracılığıyla dolaylı olarak kullanılır.

```csharp
// EncryptionService'in enjekte edildiği bir yerde (örneğin LocalSecretsManager içinde)
// IEncryptionService _encryptionService;

string sensitiveData = "Bu çok gizli bir bilgidir.";
string encryptedData = _encryptionService.Encrypt(sensitiveData);
Console.WriteLine($"Şifreli Veri: {encryptedData}");

string decryptedData = _encryptionService.Decrypt(encryptedData);
Console.WriteLine($"Şifresi Çözülmüş Veri: {decryptedData}");
```




#### 5.1.6. HealthCheckService

`HealthCheckService`, uygulamanın kritik bileşenlerinin (özellikle sır yöneticisinin) sağlığını düzenli olarak kontrol eden bir arka plan servisidir. Herhangi bir sağlık sorunu tespit edildiğinde, `EmergencyNotificationService` aracılığıyla bildirim gönderir.

**Konum:** `Application/Services/EmergencyAndSecretServices/HealthCheckService.cs`

**Fonksiyonellik:**
*   Belirli aralıklarla (varsayılan olarak 5 dakikada bir) çalışır.
*   `ILocalSecretsManager` üzerinde bir test anahtarı (`HealthCheck_TestKey`) oluşturur, değer atar, okur ve siler. Bu işlem, sır yöneticisinin doğru çalışıp çalışmadığını doğrular.
*   Eğer sır yöneticisi testi başarısız olursa, `EmergencyNotificationService` kullanarak bir güvenlik ihlali bildirimi gönderir.

**Kullanım Örneği (Dahili):**

`HealthCheckService`, bir `BackgroundService` olarak implemente edilmiştir ve uygulama başlatıldığında otomatik olarak çalışır. Bu servis, uygulamanın arka planda sürekli olarak kritik sistem bileşenlerinin sağlığını izlemesini sağlar.

```csharp
// Program.cs içinde servis kaydı (örnek)
builder.Services.AddHostedService<HealthCheckService>();

// appsettings.json içinde ilgili ayarlar (Emergency:NotificationRecipients üzerinden bildirim gönderir)
"Emergency": {
  "NotificationRecipients": "admin@example.com",
  // ... diğer SMTP/SMS ayarları
}
```

Bu servis, özellikle üretim ortamlarında uygulamanın temel güvenlik mekanizmalarının sürekli olarak izlenmesi için önemlidir.




#### 5.1.7. KeyRotationService

`KeyRotationService`, uygulamanın ana şifreleme anahtarını (MASTER_KEY) düzenli aralıklarla otomatik olarak döndürmek (yenilemek) için tasarlanmış bir arka plan servisidir. Anahtarların düzenli olarak değiştirilmesi, bir anahtarın ele geçirilmesi durumunda oluşabilecek zararı minimize ederek güvenliği artırır.

**Konum:** `Application/Services/EmergencyAndSecretServices/KeyRotationService.cs`

**Fonksiyonellik:**
*   Belirli aralıklarla (varsayılan olarak 90 günde bir) çalışır.
*   Yeni, güvenli bir MASTER_KEY oluşturur.
*   `ILocalSecretsManager` aracılığıyla saklanan tüm sırları yeni anahtar ile yeniden şifreler.
*   Ortam değişkeni olarak ayarlanmış olan `MASTER_KEY`'i günceller.

**Kullanım Örneği (Dahili):**

`KeyRotationService`, bir `BackgroundService` olarak implemente edilmiştir ve uygulama başlatıldığında otomatik olarak çalışır. Bu servis, uygulamanın arka planda sürekli olarak anahtar güvenliğini sağlamasını hedefler.

```csharp
// Program.cs içinde servis kaydı (örnek)
builder.Services.AddHostedService<KeyRotationService>();

// appsettings.json içinde ilgili ayarlar (varsa, genellikle kod içinde tanımlıdır)
// Örneğin, rotasyon aralığı gibi ayarlar burada belirtilebilir.
```

**Önemli Notlar:**
*   Anahtar rotasyonu sırasında tüm sırların yeniden şifrelenmesi gerektiğinden, bu işlem sırasında uygulamanın kısa süreliğine durdurulması veya dikkatli bir şekilde yönetilmesi gerekebilir. Mevcut implementasyonda `RotateAllSecrets` metodu, sırları tek tek günceller.
*   `MASTER_KEY`'in ortam değişkeni olarak güncellenmesi, uygulamanın yeniden başlatılmasını gerektirebilir veya ortam değişkeni yönetim sistemine bağlı olarak farklı davranışlar sergileyebilir.




### 5.2. NotificationServices

`NotificationService`, uygulama içindeki çeşitli olaylar hakkında kullanıcılara bildirim göndermek için tasarlanmıştır. Özellikle şüpheli oturumlar gibi güvenlik uyarıları için e-posta bildirimleri gönderir.

**Konum:** `Application/Services/NotificationServices/NotificationService.cs`

**Fonksiyonellik:**
*   `NotifySuspiciousSessionAsync(UserSession<Guid, Guid> session)`: Şüpheli bir kullanıcı oturumu tespit edildiğinde ilgili kullanıcıya detaylı bir e-posta bildirimi gönderir. Bu e-posta, oturumun IP adresi, konumu ve zamanı gibi bilgileri içerir ve kullanıcıya şifre değiştirme veya güvenlik ayarlarını kontrol etme gibi öneriler sunar.
*   E-posta gönderimi için `IMailService`'i kullanır.
*   E-posta şablonu HTML formatındadır ve şüpheli oturum detaylarını dinamik olarak doldurur.

**Kullanım Örneği (Dahili):**

`NotificationService`, genellikle `AuthService` veya `UserSessionService` gibi güvenlik ve oturum yönetimi ile ilgili servisler tarafından çağrılır. Örneğin, bir kullanıcının farklı bir IP adresinden veya cihazdan giriş yapması durumunda bu servis tetiklenebilir.

```csharp
// AuthService veya UserSessionService içinde (örnek)
// INotificationService _notificationService;
// UserSession<Guid, Guid> suspiciousSession = ...;
// await _notificationService.NotifySuspiciousSessionAsync(suspiciousSession);
```

**Konfigürasyon:**

`NotificationService`, `FrontedConfig` ayarlarını kullanarak e-posta içindeki bağlantıların (örneğin şifre değiştirme sayfası) doğru URL'lere yönlendirilmesini sağlar. Bu ayarlar `appsettings.json` dosyasında bulunur:

```json
"FrontedConfig": {
  "Development": "https://localhost:4200",
  "Production": "https://your-site.tr"
}
```

Bu servis, uygulamanın güvenlik olaylarına proaktif bir şekilde yanıt vermesini ve kullanıcıları potansiyel tehditler hakkında bilgilendirmesini sağlar.




### 5.3. LoggerService

NArchitectureTemplate projesi, uygulama loglarını yönetmek için esnek ve genişletilebilir bir loglama altyapısı sunar. Bu altyapı, farklı loglama hedeflerine (konsol, veritabanı, dosya vb.) log göndermeyi sağlayan bir kompozit desen (Composite Pattern) kullanır.

#### 5.3.1. CompositeLogger

`CompositeLogger`, birden fazla loglayıcıyı bir araya getirerek tek bir arayüz üzerinden loglama yapmayı sağlar. Bu sayede, loglama istekleri farklı loglama mekanizmalarına (örneğin hem veritabanına hem de dosyaya) aynı anda yönlendirilebilir.

**Konum:** `Application/Services/LoggerService/CompositeLogger.cs`

**Fonksiyonellik:**
*   `ILogger` arayüzünü implemente eder ve `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical` seviyelerinde loglama metodları sunar.
*   İçerisinde birden fazla `ILogger` implementasyonunu barındırır ve gelen loglama isteklerini bu implementasyonlara dağıtır.
*   `LoggingConfig` ayarlarını kullanarak her bir log seviyesi için loglama yapılıp yapılmayacağını kontrol eder.

**Kullanım Örneği (Dahili):**

`CompositeLogger`, uygulamanın genel loglama mekanizması olarak kullanılır. `Program.cs` dosyasında veya bağımlılık enjeksiyonu (DI) yapılandırmasında `ILogger` arayüzü istendiğinde `CompositeLogger` sağlanır.

```csharp
// Program.cs içinde ILogger'ın nasıl kullanıldığına dair örnek
// using (var scope = app.Services.CreateScope())
// {
//     try
//     {
//         var logger = scope.ServiceProvider.GetRequiredService<NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.ILogger>();
//         logger.Information($"Uygulama başlatıldı: {DateTime.UtcNow}, Environment: {builder.Environment.EnvironmentName}");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"Loglama hatası: {ex.Message}");
//     }
// }

// Uygulama içinde herhangi bir yerde loglama yapmak için:
// private readonly NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.ILogger _logger;
// _logger.Information("Kullanıcı {UserName} sisteme giriş yaptı.", "JohnDoe");
// _logger.Error(exception, "Bir hata oluştu: {ErrorMessage}", exception.Message);
```

**Konfigürasyon (`appsettings.json`):**

`CompositeLogger`, `LoggingConfig` bölümündeki ayarlara göre çalışır:

```json
"LoggingConfig": {
  "DefaultLogLevel": "Information", // Varsayılan log seviyesi
  "Override": { // Belirli kategoriler için log seviyesi geçersiz kılma
    "Microsoft.AspNetCore": "Warning"
  },
  "Target": "All" // Loglama hedefi: All, File, Database
}
```

`Target` ayarı, logların hangi hedeflere (dosya, veritabanı veya her ikisi) gönderileceğini belirler. `DefaultLogLevel` ve `Override` ayarları, hangi log seviyesindeki mesajların kaydedileceğini kontrol eder.




#### 5.3.2. ConsoleFallbackLogger

`ConsoleFallbackLogger`, uygulamanın loglarını doğrudan konsola yazan basit bir loglayıcıdır. Genellikle geliştirme ortamlarında veya diğer loglama mekanizmalarının devre dışı kaldığı durumlarda bir yedek (fallback) olarak kullanılır.

**Konum:** `Application/Services/LoggerService/ConsoleFallbackLogger.cs`

**Fonksiyonellik:**
*   `ILogger` arayüzünü implemente eder.
*   Tüm log seviyelerindeki (Trace, Debug, Information, Warning, Error, Critical) mesajları konsola yazar.
*   Hata durumlarında istisna bilgilerini de loglar.

**Kullanım Örneği (Dahili):**

`ConsoleFallbackLogger`, `ApplicationServiceRegistration.cs` içinde `CompositeLogger`'a eklenen loglayıcılardan biridir. Bu sayede, diğer loglama hedefleri (veritabanı, dosya) yapılandırılmamış olsa bile loglar konsolda görünür olacaktır.

```csharp
// ApplicationServiceRegistration.cs içinde (örnek)
// if (loggingConfig.Target == "All" || loggingConfig.Target == "Console")
// {
//     loggers.Add(new ConsoleFallbackLogger());
// }
```

Bu loglayıcı, özellikle uygulamanın başlangıç aşamalarında veya hata ayıklama sırasında hızlı geri bildirim sağlamak için kullanışlıdır.




#### 5.3.3. DatabaseLogger

`DatabaseLogger`, uygulamanın loglarını bir veritabanına kaydetmek için kullanılır. Bu, logların merkezi bir yerde depolanmasını ve daha sonra sorgulanabilir olmasını sağlar.

**Konum:** `Application/Services/LoggerService/DatabaseLogger.cs`

**Fonksiyonellik:**
*   `ILogger` arayüzünü implemente eder.
*   Gelen log mesajlarını `ILogService` aracılığıyla veritabanına kaydeder.
*   Log seviyelerine göre (Trace, Debug, Information, Warning, Error, Critical) loglama yapar.

**Kullanım Örneği (Dahili):**

`DatabaseLogger`, `ApplicationServiceRegistration.cs` içinde `CompositeLogger`'a eklenen loglayıcılardan biridir. `appsettings.json` dosyasındaki `LoggingConfig:Target` ayarı `Database` veya `All` olarak ayarlandığında aktif hale gelir.

```csharp
// ApplicationServiceRegistration.cs içinde (örnek)
// if (loggingConfig.Target == "All" || loggingConfig.Target == "Database")
// {
//     var dbLogger = sp.GetRequiredService<DatabaseLogger>();
//     loggers.Add(dbLogger);
// }
```

Bu loglayıcı, özellikle üretim ortamlarında logların kalıcı olarak saklanması ve analiz edilmesi için önemlidir.




### 5.4. Google Auth

NArchitectureTemplate projesi, kullanıcıların Google hesapları aracılığıyla uygulamaya giriş yapmalarını sağlayan Google Authentication entegrasyonuna sahiptir. Bu özellik, kullanıcı deneyimini iyileştirir ve kayıt/giriş süreçlerini basitleştirir.

**Konum:** `Core/Core.Security/OAuth/Services/GoogleAuthService.cs` ve `WebAPI/Controllers/AuthController.cs`

**Fonksiyonellik:**
*   **Yetkilendirme URL'si Oluşturma:** Kullanıcının Google'a yönlendirileceği yetkilendirme URL'sini oluşturur.
*   **Kimlik Doğrulama:** Google'dan gelen yetkilendirme kodunu kullanarak Access Token ve kullanıcı bilgilerini (e-posta, ad vb.) alır.
*   **Kullanıcı Kaydı/Güncelleme:** Alınan Google kullanıcı bilgilerini kullanarak uygulamadaki kullanıcıyı kaydeder veya günceller.
*   **JWT Token Üretimi:** Başarılı kimlik doğrulamasının ardından uygulama için JWT (JSON Web Token) ve Refresh Token üretir.

**Kullanım Örneği:**

1.  **Google Geliştirici Konsolu Yapılandırması:**
    *   Google API Console'da yeni bir proje oluşturun veya mevcut bir projeyi kullanın.
    *   "Credentials" bölümünden "OAuth client ID" oluşturun. Uygulama türü olarak "Web application" seçin.
    *   "Authorized JavaScript origins" ve "Authorized redirect URIs" alanlarına uygulamanızın URL'lerini ekleyin. Özellikle `Redirect URI` olarak `https://localhost:5001/signin-google` (geliştirme için) veya üretim URL'nizi (`https://your-site.tr/signin-google`) eklemeyi unutmayın.
    *   Oluşturulan `Client ID` ve `Client Secret` değerlerini not alın.

2.  **`appsettings.json` Konfigürasyonu:**
    `WebAPI` projesindeki `appsettings.json` dosyasına aşağıdaki ayarları ekleyin veya güncelleyin:

    ```json
    "Authentication": {
      "Google": {
        "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
        "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET",
        "RedirectUri": "https://localhost:5001/signin-google" // veya üretim URL'niz
      }
    }
    ```
    **Not:** `ClientId` ve `ClientSecret` değerleri `Secrets Management` yapısı tarafından da yönetilebilir. `SecretsExtensions.cs` dosyasında varsayılan değerler tanımlanmıştır. Üretim ortamında bu değerleri ortam değişkenleri veya güvenli bir sır yöneticisi aracılığıyla sağlamanız önerilir.

3.  **API Endpointleri:**
    *   **Giriş Başlatma:** Kullanıcı, `/api/Auth/google/login` endpoint'ine bir GET isteği göndererek Google giriş sürecini başlatır. Bu, kullanıcıyı Google'ın kimlik doğrulama sayfasına yönlendirir.
    *   **Geri Çağırma (Callback):** Google kimlik doğrulamasından sonra, kullanıcı `appsettings.json`'da belirtilen `RedirectUri`'ye yönlendirilir. Bu URL, `/api/Auth/google/callback` endpoint'ine karşılık gelir. Google, yetkilendirme kodunu bu endpoint'e gönderir ve uygulama bu kodu kullanarak Access Token ve kullanıcı bilgilerini alır.

    ```csharp
    // AuthController.cs içinde
    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        string authUrl = _googleAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        var result = await _googleAuthService.AuthenticateAsync(code);
        // ... (kullanıcı kaydı/güncelleme ve token üretimi)
        return Ok(tokenResult);
    }
    ```

Bu entegrasyon, kullanıcıların mevcut Google hesaplarıyla hızlı ve güvenli bir şekilde uygulamanıza erişmesini sağlar.




### 5.5. Facebook Auth

NArchitectureTemplate projesi, kullanıcıların Facebook hesapları aracılığıyla uygulamaya giriş yapmalarını sağlayan Facebook Authentication entegrasyonuna sahiptir. Google Auth ile benzer şekilde, bu özellik de kullanıcı deneyimini iyileştirir ve kayıt/giriş süreçlerini basitleştirir.

**Konum:** `Core/Core.Security/OAuth/Services/FacebookAuthService.cs` ve `WebAPI/Controllers/AuthController.cs`

**Fonksiyonellik:**
*   **Yetkilendirme URL'si Oluşturma:** Kullanıcının Facebook'a yönlendirileceği yetkilendirme URL'sini oluşturur.
*   **Kimlik Doğrulama:** Facebook'tan gelen yetkilendirme kodunu kullanarak Access Token ve kullanıcı bilgilerini (e-posta, ad vb.) alır.
*   **Kullanıcı Kaydı/Güncelleme:** Alınan Facebook kullanıcı bilgilerini kullanarak uygulamadaki kullanıcıyı kaydeder veya günceller.
*   **JWT Token Üretimi:** Başarılı kimlik doğrulamasının ardından uygulama için JWT (JSON Web Token) ve Refresh Token üretir.

**Kullanım Örneği:**

1.  **Facebook Geliştirici Konsolu Yapılandırması:**
    *   Facebook for Developers sayfasında yeni bir uygulama oluşturun veya mevcut bir uygulamayı kullanın.
    *   "Facebook Login" ürününü uygulamanıza ekleyin.
    *   "Settings" > "Basic" bölümünden `App ID` ve `App Secret` değerlerini not alın.
    *   "Facebook Login" > "Settings" bölümünde "Valid OAuth Redirect URIs" alanına uygulamanızın yönlendirme URL'lerini ekleyin. Özellikle `https://localhost:5001/signin-facebook` (geliştirme için) veya üretim URL'nizi (`https://your-site.tr/signin-facebook`) eklemeyi unutmayın.

2.  **`appsettings.json` Konfigürasyonu:**
    `WebAPI` projesindeki `appsettings.json` dosyasına aşağıdaki ayarları ekleyin veya güncelleyin:

    ```json
    "Authentication": {
      "Facebook": {
        "AppId": "YOUR_FACEBOOK_APP_ID",
        "AppSecret": "YOUR_FACEBOOK_APP_SECRET",
        "RedirectUri": "https://localhost:5001/signin-facebook" // veya üretim URL'niz
      }
    }
    ```
    **Not:** `AppId` ve `AppSecret` değerleri `Secrets Management` yapısı tarafından da yönetilebilir. `SecretsExtensions.cs` dosyasında varsayılan değerler tanımlanmıştır. Üretim ortamında bu değerleri ortam değişkenleri veya güvenli bir sır yöneticisi aracılığıyla sağlamanız önerilir.

3.  **API Endpointleri:**
    *   **Giriş Başlatma:** Kullanıcı, `/api/Auth/facebook/login` endpoint'ine bir GET isteği göndererek Facebook giriş sürecini başlatır. Bu, kullanıcıyı Facebook'un kimlik doğrulama sayfasına yönlendirir.
    *   **Geri Çağırma (Callback):** Facebook kimlik doğrulamasından sonra, kullanıcı `appsettings.json`'da belirtilen `RedirectUri`'ye yönlendirilir. Bu URL, `/api/Auth/facebook/callback` endpoint'ine karşılık gelir. Facebook, yetkilendirme kodunu bu endpoint'e gönderir ve uygulama bu kodu kullanarak Access Token ve kullanıcı bilgilerini alır.

    ```csharp
    // AuthController.cs içinde
    [HttpGet("facebook/login")]
    public IActionResult FacebookLogin()
    {
        string authUrl = _facebookAuthService.GetAuthorizationUrl();
        return Redirect(authUrl);
    }

    [HttpGet("facebook/callback")]
    public async Task<IActionResult> FacebookCallback([FromQuery] string code)
    {
        var result = await _facebookAuthService.AuthenticateAsync(code);
        // ... (kullanıcı kaydı/güncelleme ve token üretimi)
        return Ok(tokenResult);
    }
    ```

Bu entegrasyon, kullanıcıların mevcut Facebook hesaplarıyla hızlı ve güvenli bir şekilde uygulamanıza erişmesini sağlar.




### 5.6. TranslateService

`TranslateService`, metin çeviri işlemleri için tasarlanmış bir arayüzdür. Proje yapısında `ITranslateService` arayüzü tanımlanmış olmasına rağmen, bu servisin somut bir implementasyonu (`TranslateService.cs` gibi bir dosya) `starterProject` içerisinde doğrudan bulunmamaktadır. Bu durum, çeviri hizmetinin harici bir API (örneğin Google Translate API, DeepL API) aracılığıyla sağlanması veya geliştiricinin kendi özel çeviri mantığını implemente etmesi için bir yer tutucu (placeholder) olduğunu göstermektedir.

**Konum:** `Application/Services/TranslateService/ITranslateService.cs`

**Fonksiyonellik (Beklenen):**
*   `Translate(string text, string sourceLanguage, string targetLanguage)`: Belirtilen metni kaynak dilden hedef dile çevirir.
*   `DetectLanguage(string text)`: Verilen metnin dilini otomatik olarak algılar.

**Kullanım Örneği (Potansiyel):**

Bu servis, kullanıcı arayüzünde çok dilli destek sağlamak, kullanıcı tarafından girilen metinleri çevirmek veya farklı dillerdeki içerikleri işlemek gibi senaryolarda kullanılabilir.

```csharp
// ITranslateService arayüzünün tanımı (örnek)
public interface ITranslateService
{
    Task<string> Translate(string text, string sourceLanguage, string targetLanguage);
    Task<string> DetectLanguage(string text);
}

// Kullanım (örnek, somut implementasyon gerektirir)
// private readonly ITranslateService _translateService;
// string translatedText = await _translateService.Translate("Hello World", "en", "tr");
// Console.WriteLine(translatedText); // Merhaba Dünya
```

**Implementasyon Notları:**

Bu servisi kullanabilmek için aşağıdaki adımlardan birini izlemeniz gerekmektedir:

1.  **Harici Çeviri API Entegrasyonu:** Google Cloud Translation API, Microsoft Translator Text API veya DeepL API gibi bir üçüncü taraf çeviri hizmeti sağlayıcısı ile entegrasyon yapın. Bu entegrasyonu `Infrastructure` katmanında yeni bir sınıf olarak implemente edebilir ve `ITranslateService` arayüzünü uygulayabilirsiniz.
2.  **Özel Çeviri Mantığı:** Eğer basit çeviri ihtiyaçlarınız varsa veya kendi çeviri veritabanınızı kullanıyorsanız, `ITranslateService` arayüzünü implemente eden kendi özel çeviri mantığınızı yazabilirsiniz.

Bu servis, uygulamanızın çok dilli yeteneklerini genişletmek için önemli bir esneklik sunar.




### 5.7. Email Service (IMailService)

NArchitectureTemplate projesi, uygulama içerisinden e-posta gönderme işlevselliği için `IMailService` arayüzünü ve `MailKitMailService` implementasyonunu kullanır. Bu servis, şifre sıfırlama, şüpheli oturum bildirimleri ve diğer uygulama içi e-posta iletişimleri için merkezi bir mekanizma sağlar.

**Konum:** `Core/Core.Mailing/IMailService.cs` (arayüz) ve `Core/Core.Mailing.MailKit/MailKitMailService.cs` (implementasyon)

**Fonksiyonellik:**
*   `SendEmailAsync(Mail mail)`: Belirtilen `Mail` nesnesini kullanarak asenkron olarak e-posta gönderir. `Mail` nesnesi alıcı listesi, konu, HTML gövdesi ve ekler gibi bilgileri içerir.
*   SMTP sunucu ayarları (`MailSettings`) kullanılarak e-posta gönderimi gerçekleştirilir.

**Kullanım Örneği:**

`IMailService`, uygulamanın çeşitli katmanlarında (örneğin `Application` katmanındaki komut işleyicilerinde veya servislerde) bağımlılık enjeksiyonu aracılığıyla kullanılır.

```csharp
// IMailService enjekte edilmiş bir sınıf içinde (örneğin ForgotPasswordCommandHandler)
// private readonly IMailService _mailService;

// E-posta gönderme örneği
Mail mail = new Mail
{
    ToList = new List<MailboxAddress> { new MailboxAddress("Alıcı Adı", "alici@example.com") },
    Subject = "Şifre Sıfırlama Talebi",
    HtmlBody = "<p>Şifrenizi sıfırlamak için <a href=\"https://your-app.com/reset-password?token=abc\">buraya tıklayın</a>.</p>",
    // FromList, ReplyToList, BccList, CcList, Attachments gibi diğer özellikler de ayarlanabilir.
};
await _mailService.SendEmailAsync(mail);
```

**Konfigürasyon (`appsettings.json`):**

E-posta servisinin çalışabilmesi için `appsettings.json` dosyasında `MailSettings` bölümünün doğru şekilde yapılandırılması gerekmektedir:

```json
"MailSettings": {
  "SenderEmail": "noreply@yourdomain.com",
  "SenderFullName": "Your Application Name",
  "Password": "", // LocalSecretsManager tarafından doldurulacak
  "Server": "smtp.yourdomain.com",
  "Port": 587,
  "EnableSsl": true,
  "UseDefaultCredentials": false,
  "IsBodyHtml": true,
  "DkimPrivateKey": "", // LocalSecretsManager tarafından doldurulacak (isteğe bağlı)
  "DomainName": "yourdomain.com", // LocalSecretsManager tarafından doldurulacak (isteğe bağlı)
  "UserName": "noreply@yourdomain.com" // LocalSecretsManager tarafından doldurulacak
}
```

**Önemli Notlar:**
*   `Password`, `DkimPrivateKey`, `DomainName` ve `UserName` gibi hassas bilgiler `Secrets Management` yapısı aracılığıyla yönetilmelidir. `SecretsExtensions.cs` dosyasında varsayılan geliştirme değerleri tanımlanmıştır.
*   E-posta gönderme sorunlarını önlemek için SMTP sunucu bilgileri (Server, Port, EnableSsl) doğru bir şekilde ayarlanmalıdır.

Bu servis, uygulamanızın kullanıcılarla etkileşim kurması ve önemli bildirimleri göndermesi için temel bir iletişim kanalı sağlar.




## 6. API Endpointleri

NArchitectureTemplate projesi, çeşitli işlevler için RESTful API endpointleri sunmaktadır. Bu bölümde, `WebAPI/Controllers` dizini altında bulunan ana controller'lar ve içerdikleri endpointler detaylı olarak açıklanmıştır.

### 6.1. AuthController

`AuthController`, kullanıcı kimlik doğrulama ve yetkilendirme işlemlerini yönetir. JWT tabanlı kimlik doğrulama, sosyal giriş (Google ve Facebook), iki faktörlü kimlik doğrulama (2FA) ve Refresh Token yönetimi gibi özellikleri destekler.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/Auth/Login` | `POST` | Kullanıcı girişi yapar ve JWT token ile Refresh Token döndürür. 2FA aktifse `authenticatorCode` gereklidir. | `{"email": "string", "password": "string", "authenticatorCode": "string" (isteğe bağlı)}` | `{"accessToken": {"token": "string", "expiration": "datetime"}, "refreshToken": "string"}` |
| `/api/Auth/Register` | `POST` | Yeni kullanıcı kaydı yapar ve JWT token ile Refresh Token döndürür. | `{"email": "string", "password": "string"}` | `{"accessToken": {"token": "string", "expiration": "datetime"}, "refreshToken": "string"}` |
| `/api/Auth/RefreshToken` | `POST` | Mevcut Refresh Token ile yeni bir Access Token ve Refresh Token alır. | `{"refreshToken": "string" (isteğe bağlı, çerezden de alınabilir)}` | `{"accessToken": {"token": "string", "expiration": "datetime"}, "refreshToken": "string"}` |
| `/api/Auth/RevokeToken` | `PUT` | Belirtilen Refresh Token'ı iptal eder. | `"string" (Refresh Token, isteğe bağlı, çerezden de alınabilir)` | `{"message": "Token successfully revoked."}` |
| `/api/Auth/logout` | `GET` | Kullanıcının oturumunu kapatır ve ana sayfaya yönlendirir. Refresh Token'ı iptal eder. | Yok | Yönlendirme (Redirect) |
| `/api/Auth/EnableEmailAuthenticator` | `GET` | Email tabanlı 2FA'yı aktifleştirir. | Yok | `200 OK` |
| `/api/Auth/EnableOtpAuthenticator` | `GET` | OTP tabanlı 2FA'yı aktifleştirir ve OTP kodunu döndürür. | Yok | `{"secretKey": "string", "qrCodeImageUrl": "string"}` |
| `/api/Auth/DisableEmailAuthenticator` | `GET` | Email tabanlı 2FA'yı devre dışı bırakır. | Yok | `200 OK` |
| `/api/Auth/DisableOtpAuthenticator` | `GET` | OTP tabanlı 2FA'yı devre dışı bırakır. | Yok | `200 OK` |
| `/api/Auth/VerifyEmailAuthenticator` | `GET` | Email tabanlı 2FA doğrulamasını gerçekleştirir. | Query Parametreleri: `ActivationCode=string`, `UserId=guid` | `200 OK` |
| `/api/Auth/VerifyOtpAuthenticator` | `POST` | OTP tabanlı 2FA doğrulamasını gerçekleştirir. | `"string" (OTP kodu)` | `200 OK` |
| `/api/Auth/google/login` | `GET` | Google ile giriş yapma işlemini başlatır. | Yok | Yönlendirme (Redirect to Google Auth URL) |
| `/api/Auth/google/callback` | `GET` | Google giriş işlemini tamamlar ve JWT token döndürür. | Query Parametreleri: `code=string` | `{"accessToken": {"token": "string", "expiration": "datetime"}, "refreshToken": "string"}` |
| `/api/Auth/facebook/login` | `GET` | Facebook ile giriş yapma işlemini başlatır. | Yok | Yönlendirme (Redirect to Facebook Auth URL) |
| `/api/Auth/facebook/callback` | `GET` | Facebook giriş işlemini tamamlar ve JWT token döndürür. | Query Parametreleri: `code=string` | `{"accessToken": {"token": "string", "expiration": "datetime"}, "refreshToken": "string"}` |




### 6.2. UsersController

`UsersController`, kullanıcı yönetimi ile ilgili işlemleri (oluşturma, güncelleme, silme, listeleme, şifre sıfırlama, yetki ve grup atama vb.) yönetir.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/Users/{Id}` | `GET` | Belirli bir kullanıcıyı ID ile getirir. | Yok | `{"id": "guid", "email": "string", ...}` |
| `/api/Users` | `GET` | Tüm kullanıcıların listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "guid", "email": "string", ...}], "count": int, ...}` |
| `/api/Users` | `POST` | Yeni bir kullanıcı oluşturur. | `{"email": "string", "password": "string", ...}` | `{"id": "guid", "email": "string", ...}` |
| `/api/Users` | `PUT` | Mevcut bir kullanıcının bilgilerini günceller. | `{"id": "guid", "email": "string", "password": "string" (isteğe bağlı), ...}` | `{"id": "guid", "email": "string", ...}` |
| `/api/Users/UpdateFromAuth` | `PUT` | Kimlik doğrulaması yapılmış kullanıcının kendi bilgilerini günceller. | `{"email": "string", ...}` | `{"id": "guid", "email": "string", ...}` |
| `/api/Users` | `DELETE` | Belirli bir kullanıcıyı siler. | `{"id": "guid"}` | `{"id": "guid", "email": "string", ...}` |
| `/api/Users/me` | `GET` | Kimlik doğrulaması yapılmış kullanıcının kendi bilgilerini getirir. | Yok | `{"id": "guid", "email": "string", ...}` |
| `/api/Users/{userId}/status` | `PUT` | Belirli bir kullanıcının durumunu günceller. | `{"status": "int"}` | `{"id": "guid", "status": "int", ...}` |
| `/api/Users/UpdatePasswordFromAuth` | `PUT` | Kimlik doğrulaması yapılmış kullanıcının şifresini günceller. | `{"oldPassword": "string", "newPassword": "string"}` | `{"id": "guid", "email": "string", ...}` |
| `/api/Users/{userId}/photo-url` | `PUT` | Belirli bir kullanıcının fotoğraf URL\\'sini günceller. | `{"photoUrl": "string"}` | `{"id": "guid", "photoUrl": "string", ...}` |
| `/api/Users/ForgotPassword` | `POST` | Kullanıcının şifresini unuttuğunda şifre sıfırlama bağlantısı göndermek için kullanılan API metodu. | `{"email": "string"}` | `{"message": "Password reset email sent."}` |
| `/api/Users/ResetPassword` | `POST` | Şifre sıfırlama token\\'ı ile yeni şifre belirler. | `{"email": "string", "token": "string", "newPassword": "string"}` | `{"message": "Password reset successfully."}` |
| `/api/Users/{userId}/claims` | `GET` | Belirli bir kullanıcının claim\\'lerini (yetkilerini) getirir. | Yok | `[{"id": "int", "name": "string"}, ...]` |
| `/api/Users/{userId}/claims` | `POST` | Belirli bir kullanıcıya claim\\'ler ekler. | `[int, int, ...]` (Claim ID\\'leri listesi) | `{"userId": "guid", "claimIds": [int, ...], ...}` |
| `/api/Users/{userId}/claims` | `PUT` | Belirli bir kullanıcının claim\\'lerini günceller (mevcutları silip yenilerini ekler). | `[int, int, ...]` (Claim ID\\'leri listesi) | `{"userId": "guid", "claimIds": [int, ...], ...}` |
| `/api/Users/{userId}/groups` | `GET` | Belirli bir kullanıcının gruplarını getirir. | Yok | `[{"id": "int", "name": "string"}, ...]` |
| `/api/Users/{userId}/groups` | `POST` | Belirli bir kullanıcıya gruplar ekler. | `[int, int, ...]` (Grup ID\\'leri listesi) | `{"userId": "guid", "groupIds": [int, ...], ...}` |
| `/api/Users/{userId}/groups` | `PUT` | Belirli bir kullanıcının gruplarını günceller (mevcutları silip yenilerini ekler). | `[int, int, ...]` (Grup ID\\'leri listesi) | `{"userId": "guid", "groupIds": [int, ...], ...}` |




### 6.3. DeviceTokensController

`DeviceTokensController`, mobil cihaz bildirimleri için kullanılan cihaz token'larının yönetimi ile ilgili işlemleri (oluşturma, güncelleme, silme, listeleme, ID ile getirme) yönetir.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/DeviceTokens` | `POST` | Yeni bir cihaz token'ı ekler. | `{"userId": "guid", "token": "string", "deviceType": "string"}` | `{"id": "guid", "userId": "guid", "token": "string", "deviceType": "string"}` |
| `/api/DeviceTokens` | `PUT` | Mevcut bir cihaz token'ını günceller. | `{"id": "guid", "userId": "guid", "token": "string", "deviceType": "string"}` | `{"id": "guid", "userId": "guid", "token": "string", "deviceType": "string"}` |
| `/api/DeviceTokens/{id}` | `DELETE` | Belirli bir cihaz token'ını siler. | Yok | `{"id": "guid"}` |
| `/api/DeviceTokens/{id}` | `GET` | Belirli bir cihaz token'ını ID ile getirir. | Yok | `{"id": "guid", "userId": "guid", "token": "string", "deviceType": "string"}` |
| `/api/DeviceTokens` | `GET` | Tüm cihaz token'larının listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "guid", "userId": "guid", "token": "string", "deviceType": "string"}], "count": int, ...}` |




### 6.4. ExceptionLogsController

`ExceptionLogsController`, uygulama içerisinde oluşan istisna (exception) loglarının yönetimi ile ilgili işlemleri (oluşturma, güncelleme, silme, listeleme, ID ile getirme) yönetir.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/ExceptionLogs` | `POST` | Yeni bir istisna logu ekler. | `{"message": "string", "stackTrace": "string", "exceptionType": "string", ...}` | `{"id": "guid", "message": "string", ...}` |
| `/api/ExceptionLogs` | `PUT` | Mevcut bir istisna logunu günceller. | `{"id": "guid", "message": "string", "stackTrace": "string", "exceptionType": "string", ...}` | `{"id": "guid", "message": "string", ...}` |
| `/api/ExceptionLogs/{id}` | `DELETE` | Belirli bir istisna logunu siler. | Yok | `{"id": "guid"}` |
| `/api/ExceptionLogs/{id}` | `GET` | Belirli bir istisna logunu ID ile getirir. | Yok | `{"id": "guid", "message": "string", ...}` |
| `/api/ExceptionLogs` | `GET` | Tüm istisna loglarının listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "guid", "message": "string", ...}], "count": int, ...}` |




### 6.5. GroupOperationClaimsController

`GroupOperationClaimsController`, gruplara operasyon yetkileri (operation claims) atama ve yönetme işlemleri ile ilgilidir. Bu controller, grupların hangi işlemleri yapabileceğini belirleyen yetkilendirme kurallarını yönetir.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/GroupOperationClaims` | `POST` | Bir gruba yeni operasyon yetkileri ekler. | `{"groupId": "int", "operationClaimIds": [int, int, ...]}` | `{"id": "guid", "groupId": "int", "operationClaimIds": [int, ...], ...}` |
| `/api/GroupOperationClaims` | `PUT` | Bir grubun operasyon yetkilerini günceller. | `{"id": "guid", "groupId": "int", "operationClaimIds": [int, int, ...]}` | `{"id": "guid", "groupId": "int", "operationClaimIds": [int, ...], ...}` |
| `/api/GroupOperationClaims/{id}` | `DELETE` | Belirli bir grup operasyon yetkisini siler. | Yok | `{"id": "guid"}` |
| `/api/GroupOperationClaims/{id}` | `GET` | Belirli bir grup operasyon yetkisini ID ile getirir. | Yok | `{"id": "guid", "groupId": "int", "operationClaimId": "int"}` |
| `/api/GroupOperationClaims` | `GET` | Tüm grup operasyon yetkilerinin listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "guid", "groupId": "int", "operationClaimId": "int"}], "count": int, ...}` |




### 6.6. GroupRolesController

`GroupRolesController`, gruplara rol atama ve yönetme işlemleri ile ilgilidir. Bu controller, grupların sahip olduğu rolleri belirler.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/GroupRoles` | `POST` | Bir gruba yeni rol ekler. | `{"groupId": "int", "roleId": "int"}` | `{"id": "int", "groupId": "int", "roleId": "int"}` |
| `/api/GroupRoles` | `PUT` | Mevcut bir grup rolünü günceller. | `{"id": "int", "groupId": "int", "roleId": "int"}` | `{"id": "int", "groupId": "int", "roleId": "int"}` |
| `/api/GroupRoles/{id}` | `DELETE` | Belirli bir grup rolünü siler. | Yok | `{"id": "int"}` |
| `/api/GroupRoles/{id}` | `GET` | Belirli bir grup rolünü ID ile getirir. | Yok | `{"id": "int", "groupId": "int", "roleId": "int"}` |
| `/api/GroupRoles` | `GET` | Tüm grup rollerinin listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "int", "groupId": "int", "roleId": "int"}], "count": int, ...}` |




### 6.7. GroupsController

`GroupsController`, kullanıcı gruplarının yönetimi ile ilgili işlemleri (oluşturma, güncelleme, silme, listeleme, ID ile getirme) yönetir.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/Groups` | `POST` | Yeni bir grup oluşturur. | `{"name": "string", "description": "string"}` | `{"id": "int", "name": "string", "description": "string"}` |
| `/api/Groups` | `PUT` | Mevcut bir grubu günceller. | `{"id": "int", "name": "string", "description": "string"}` | `{"id": "int", "name": "string", "description": "string"}` |
| `/api/Groups/{id}` | `DELETE` | Belirli bir grubu siler. | Yok | `{"id": "int"}` |
| `/api/Groups/{id}` | `GET` | Belirli bir grubu ID ile getirir. | Yok | `{"id": "int", "name": "string", "description": "string"}` |
| `/api/Groups` | `GET` | Tüm grupların listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "int", "name": "string", "description": "string"}], "count": int, ...}` |




### 6.8. LogsController

`LogsController`, uygulama loglarının yönetimi ile ilgili işlemleri (oluşturma, güncelleme, silme, listeleme, ID ile getirme) yönetir.

| Endpoint | Metot | Açıklama | İstek Gövdesi (Request Body) | Yanıt (Response) |
|---|---|---|---|---|
| `/api/Logs` | `POST` | Yeni bir log kaydı ekler. | `{"message": "string", "level": "string", "timestamp": "datetime", ...}` | `{"id": "guid", "message": "string", ...}` |
| `/api/Logs` | `PUT` | Mevcut bir log kaydını günceller. | `{"id": "guid", "message": "string", "level": "string", "timestamp": "datetime", ...}` | `{"id": "guid", "message": "string", ...}` |
| `/api/Logs/{id}` | `DELETE` | Belirli bir log kaydını siler. | Yok | `{"id": "guid"}` |
| `/api/Logs/{id}` | `GET` | Belirli bir log kaydını ID ile getirir. | Yok | `{"id": "guid", "message": "string", ...}` |
| `/api/Logs` | `GET` | Tüm log kayıtlarının listesini sayfalama ile getirir. | Query Parametreleri: `PageIndex=int`, `PageSize=int` | `{"items": [{"id": "guid", "message": "string", ...}], "count": int, ...}` |



