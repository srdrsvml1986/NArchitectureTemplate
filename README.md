# NArchitectureTemplate

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
![Last Updated](https://img.shields.io/badge/Last%20Updated-2025--04--06-brightgreen.svg)
[![Maintainer](https://img.shields.io/badge/Maintainer-@srdrsvml1986-blue.svg)](https://github.com/srdrsvml1986)

Modern, ölçeklenebilir ve sürdürülebilir .NET uygulamaları geliştirmek için tasarlanmış kapsamlı bir mimari şablondur. Clean Architecture ve Union Architecture prensiplerine dayalı olan bu template, enterprise-level uygulamalar için gerekli tüm temel yapıları ve best practice'leri içermektedir.

## 📚 İçindekiler

- [Özellikler](#-özellikler)
- [Mimari Yapı](#-mimari-yapı)
- [Teknik Detaylar](#-teknik-detaylar)
- [Kurulum](#-kurulum)
- [Konfigürasyon](#-konfigürasyon)
- [Kullanım](#-kullanım)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Güvenlik](#-güvenlik)
- [Test](#-test)
- [Katkıda Bulunma](#-katkıda-bulunma)
- [Lisans](#-lisans)

## 🌟 Özellikler

### Mimari ve Tasarım
- **Clean Architecture**: Bağımlılıkları minimize eden, test edilebilir ve sürdürülebilir kod
- **CQRS Pattern**: Command ve Query sorumluluklarının ayrılması
- **Modüler Yapı**: Bağımsız ve yeniden kullanılabilir modüller
- **Domain-Driven Design**: İş mantığı odaklı geliştirme yaklaşımı
- **Microservice Ready**: Mikroservis mimarisine uygun tasarım

### Core Bileşenler ve Özellikleri

#### 🔐 Core.Security
- JWT (JSON Web Token) tabanlı kimlik doğrulama
- Role-based ve Policy-based yetkilendirme
- 2FA (Two-Factor Authentication) desteği
- Şifreleme ve hash işlemleri
- OAuth 2.0 ve OpenID Connect desteği

#### 📦 Core.Application
- CQRS implementasyonu
- MediatR entegrasyonu
- Request-Response pattern
- Validation pipeline (FluentValidation)
- Cross-cutting concerns (logging, caching, validation, vb.)
- Business rules engine

#### 🗃 Core.Persistence
- Repository pattern implementasyonu
- Unit of Work pattern
- Entity Framework Core entegrasyonu
- Generic repository implementations
- Async operasyon desteği
- Multi-database desteği

#### 📧 Core.Mailing
- MailKit entegrasyonu
- Template-based e-posta gönderimi
- HTML ve text formatında e-postalar
- Attachments desteği
- SMTP konfigürasyonu

#### 🔍 Core.ElasticSearch
- Elasticsearch client implementasyonu
- Full-text search yetenekleri
- Document indexing ve mapping
- Aggregation desteği
- Bulk operasyonları

#### 🌍 Core.Localization
- Çoklu dil desteği
- Resource-based lokalizasyon
- YAML tabanlı kaynak dosyaları
- Kültür-bazlı formatlama
- Dinamik dil değiştirme

## 🏗 Mimari Yapı

```
src/
├── core/                          # Çekirdek kütüphaneler
│   ├── Core.Application/         # Uygulama katmanı bileşenleri
│   │   ├── Pipelines/          # MediatR pipeline behaviors
│   │   ├── Rules/             # Business rules
│   │   └── Features/         # CQRS features
│   ├── Core.Domain/            # Domain katmanı
│   │   ├── Entities/         # Domain entities
│   │   ├── Events/          # Domain events
│   │   └── ValueObjects/   # Value objects
│   ├── Core.Security/         # Güvenlik implementasyonları
│   │   ├── JWT/            # JWT işlemleri
│   │   ├── Encryption/    # Şifreleme işlemleri
│   │   └── Hashing/      # Hash işlemleri
│   └── Core.Infrastructure/   # Altyapı servisleri
│       ├── Persistence/    # Veritabanı işlemleri
│       ├── Caching/       # Cache mekanizmaları
│       └── Messaging/    # Mesajlaşma sistemleri
└── starterProject/           # Örnek proje implementasyonu
    ├── WebAPI/             # Web API katmanı
    ├── Application/       # Uygulama katmanı
    └── Infrastructure/   # Altyapı katmanı
```

## 🔧 Teknik Detaylar

### Pipeline Behaviors
```csharp
services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
    configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
    configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
    configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
    configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
    configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
});
```

### JWT Konfigürasyonu
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });
```

## 🚀 Kurulum

### Ön Gereksinimler

- .NET 9.0 SDK
- SQL Server 2019+ (veya tercih edilen başka bir veritabanı)
- Redis (opsiyonel, caching için)
- Elasticsearch 7.x+ (opsiyonel, full-text search için)

### Adım Adım Kurulum

1. Projeyi klonlayın:
```bash
git clone https://github.com/srdrsvml1986/NArchitectureTemplate.git
```

2. Gerekli NuGet paketlerini yükleyin:
```bash
dotnet restore
```

3. Veritabanı migrationlarını uygulayın:
```bash
cd src/starterProject/WebAPI
dotnet ef database update
```

4. Projeyi derleyin:
```bash
dotnet build
```

5. API'yi başlatın:
```bash
dotnet run
```

## ⚙️ Konfigürasyon

### appsettings.json Örneği
```json
{
  "TokenOptions": {
    "Audience": "starterproject",
    "Issuer": "starterproject",
    "AccessTokenExpiration": 10,
    "SecurityKey": "your-super-secret-key-here",
    "RefreshTokenTTL": 2
  },
  "MailSettings": {
    "Server": "smtp.example.com",
    "Port": 587,
    "SenderFullName": "Starter Project",
    "SenderEmail": "info@example.com",
    "UserName": "your-username",
    "Password": "your-password"
  },
  "ElasticSearchConfig": {
    "ConnectionString": "http://localhost:9200",
    "UserName": "elastic",
    "Password": "elastic"
  },
  "CacheSettings": {
    "SlidingExpiration": 2
  }
}
```

## 📍 Kullanım

### CQRS Örneği

1. Query Tanımlama:
```csharp
public class GetUserQuery : IRequest<UserDto>, ISecuredRequest
{
    public Guid Id { get; set; }
    public string[] Roles => ["Admin", "User.Read"];
}
```

2. Handler Implementasyonu:
```csharp
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetAsync(u => u.Id == request.Id);
        return _mapper.Map<UserDto>(user);
    }
}
```

### Validation Örneği

```csharp
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");
    }
}
```

## 🔒 Güvenlik Özellikleri

- JWT tabanlı kimlik doğrulama
- Refresh token mekanizması
- Role-based authorization
- Policy-based authorization
- Password hashing (PBKDF2)
- API rate limiting
- Cross-Origin Resource Sharing (CORS) yapılandırması
- SSL/TLS desteği
- Request validation
- Anti-forgery token desteği

## 🧪 Test

### Unit Test Örneği

```csharp
[TestFixture]
public class CreateUserCommandTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private CreateUserCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task Handle_ValidUser_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email
        };

        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _userRepositoryMock.Setup(r => r.AddAsync(user)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsSuccess, Is.True);
    }
}
```

## 📚 API Dokümantasyonu

API dokümantasyonu Swagger/OpenAPI ile otomatik olarak oluşturulur ve `/swagger` endpoint'inden erişilebilir.

### Swagger Konfigürasyonu

```csharp
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition(
        name: "Bearer",
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        }
    );
    opt.OperationFilter<BearerSecurityRequirementOperationFilter>();
});
```

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylı bilgi için [LICENSE](LICENSE) dosyasını inceleyebilirsiniz.

## 🔗 Faydalı Linkler

- [Clean Architecture Documentation](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [JWT Authentication](https://jwt.io/introduction)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [FluentValidation Documentation](https://fluentvalidation.net/)
- [AutoMapper Documentation](https://docs.automapper.org/en/stable/)
- [MediatR Documentation](https://github.com/jbogard/MediatR/wiki)

## 🙏 Teşekkürler

Bu template'i kullanarak geri bildirimde bulunan ve katkıda bulunan herkese teşekkürler!

---

- GitHub: [GitHub Repository](https://github.com/srdrsvml1986/NArchitectureTemplate)
- Website: [serdarsevimli.tr](https://serdarsevimli.tr)
- E-mail: [bilgi@serdarsevimli.tr](mailto:bilgi@serdarsevimli.tr)