# NArchitectureTemplate API

Bu proje, temiz mimari prensiplerine uygun olarak geliştirilmiş bir .NET Core API şablonudur. Güvenlik, loglama, bildirimler ve dış servis entegrasyonları gibi modern bir API uygulamasının temel ihtiyaçlarını karşılamak üzere tasarlanmıştır.




## Proje Yapısı

NArchitectureTemplate projesi, temiz mimari prensiplerine uygun olarak aşağıdaki ana katmanlardan oluşmaktadır:

- **Domain:** İş varlıklarını (entities), değer nesnelerini (value objects) ve iş kurallarını (business rules) içerir.
- **Application:** Uygulama katmanı, iş akışlarını ve kullanım senaryolarını (use cases) tanımlar.
- **Persistence:** Veri erişim katmanı olup, veritabanı işlemleri ve ORM konfigürasyonlarını içerir.
- **Infrastructure:** Harici servis entegrasyonları (e-posta, SMS, loglama, cache vb.) ve altyapısal bileşenleri barındırır.
- **WebAPI:** API endpointlerini, HTTP isteklerini işleyen controllerları ve uygulamanın dış dünyaya açılan arayüzünü içerir.

Detaylı proje yapısı için `user_manual.md` dosyasını inceleyebilirsiniz.




## MASTER_KEY Kurulumu

NArchitectureTemplate projesi, hassas bilgilerin (veritabanı bağlantı dizeleri, API anahtarları vb.) güvenli bir şekilde yönetilmesi için özel bir "Secrets Management" yapısı kullanır. Bu yapının temelini **MASTER_KEY** oluşturur. MASTER_KEY, sırların şifrelenmesi ve şifresinin çözülmesi için kullanılan ana anahtardır.

**MASTER_KEY'i Ortam Değişkeni Olarak Ayarlama:**

MASTER_KEY, uygulamanın çalıştığı ortamda bir ortam değişkeni olarak ayarlanmalıdır. Güvenli bir anahtar oluşturmak ve ortam değişkeni olarak ayarlamak için `user_manual.md` dosyasındaki "MASTER_KEY Kurulumu ve Kullanımı" bölümüne bakınız.




## Temel Servisler

Proje, çeşitli işlevleri yerine getirmek için aşağıdaki temel servisleri kullanır:

-   **EmergencyAndSecretServices:** Güvenlik, acil durum yönetimi ve sır yönetimi (yedekleme, şifreleme, sağlık kontrolü, anahtar rotasyonu) gibi kritik altyapısal ihtiyaçları karşılar.
-   **NotificationServices:** Uygulama içindeki çeşitli olaylar hakkında kullanıcılara bildirim göndermek için kullanılır (örneğin şüpheli oturum e-postaları).
-   **LoggerService:** Uygulama loglarını yönetmek için esnek ve genişletilebilir bir loglama altyapısı sunar (konsol, veritabanı, dosya loglama).
-   **Google Auth & Facebook Auth:** Kullanıcıların Google ve Facebook hesapları aracılığıyla uygulamaya giriş yapmalarını sağlar.
-   **TranslateService:** Metin çeviri işlemleri için bir arayüz sağlar (implementasyon gerektirir).
-   **Email Service (IMailService):** Uygulama içerisinden e-posta gönderme işlevselliği için kullanılır (şifre sıfırlama, bildirimler vb.).

Her bir servisin detaylı açıklamaları ve kullanım örnekleri için `user_manual.md` dosyasını inceleyebilirsiniz.




## API Endpointleri

Proje, çeşitli işlevler için RESTful API endpointleri sunmaktadır. Başlıca controller'lar ve içerdikleri endpointler şunlardır:

-   `AuthController`: Kullanıcı kimlik doğrulama ve yetkilendirme (Giriş, Kayıt, Token Yenileme, 2FA, Sosyal Giriş).
-   `UsersController`: Kullanıcı yönetimi (Oluşturma, Güncelleme, Silme, Listeleme, Şifre Sıfırlama, Yetki/Grup Atama).
-   `DeviceTokensController`: Mobil cihaz bildirim tokenlarının yönetimi.
-   `ExceptionLogsController`: Uygulama istisna loglarının yönetimi.
-   `GroupOperationClaimsController`: Gruplara operasyon yetkileri atama.
-   `GroupRolesController`: Gruplara rol atama.
-   `GroupsController`: Kullanıcı gruplarının yönetimi.
-   `LogsController`: Uygulama loglarının yönetimi.
-   `NotificationsController`: Anlık bildirim gönderme.
-   `OperationClaimsController`: Operasyon yetkilerinin yönetimi.
-   `RoleOperationClaimsController`: Rollere operasyon yetkileri atama.
-   `RolesController`: Kullanıcı rollerinin yönetimi.
-   `SmsCallbackController`: SMS teslimat raporlarını alma.
-   `UserGroupsController`: Kullanıcıların gruplara atanması.
-   `UserNotificationSettingsController`: Kullanıcı bildirim ayarlarının yönetimi.
-   `UserOperationClaimsController`: Kullanıcılara operasyon yetkileri atama.
-   `UserRolesController`: Kullanıcılara rol atama.
-   `UserSessionsController`: Kullanıcı oturumlarının yönetimi.

Her bir endpoint'in detaylı açıklaması, metodu, istek/yanıt yapıları için `user_manual.md` dosyasındaki "API Endpointleri" bölümünü inceleyebilirsiniz.



