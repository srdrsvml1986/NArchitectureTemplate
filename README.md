# NArchitecture Core Project

Bu proje, Clean Architecture ve Union Architecture prensiplerine dayalı modern bir .NET çözümüdür. .NET 9.0 hedef framework'ü üzerinde geliştirilmiştir.

## Özellikler

- **CQRS & MediatR**: Command Query Responsibility Segregation pattern'i ve MediatR kütüphanesi ile gelişmiş request/response yönetimi
- **Exception Handling**: Merkezi hata yönetimi ve custom problem details implementasyonları
- **Localization**: Çoklu dil desteği ve yerelleştirme altyapısı
- **Logging**: Serilog tabanlı gelişmiş loglama yetenekleri
- **Mail Service**: MailKit üzerinden e-posta gönderimi desteği
- **Security**: JWT authentication, authorization ve diğer güvenlik özellikleri
- **Swagger Integration**: API dokümantasyonu için Swagger desteği
- **Testing**: Unit ve integration testleri için gerekli araçlar

## Proje Yapısı


```
src/
├── core/
│   ├── Core.Application/
│   ├── Core.Mailing/
│   ├── Core.Security/
│   ├── Core.Test/
│   └── ...
└── acenteApi/
    └── WebAPI/

```

## Teknolojiler & Framework'ler

- .NET 9.0
- MediatR 12.4.1
- FluentValidation 11.11.0
- AutoMapper 14.0.0
- MailKit 4.10.0
- Serilog
- Entity Framework Core 9.0.2
- Swagger/OpenAPI

## Başlangıç

1. Projeyi klonlayın

```shell
git clone [repo-url]

```

2. Bağımlılıkları yükleyin

```shell
dotnet restore

```

3. Projeyi derleyin

```shell
dotnet build

```

bu bir template projesidir. şablon olarak kullanılabilir.
```

