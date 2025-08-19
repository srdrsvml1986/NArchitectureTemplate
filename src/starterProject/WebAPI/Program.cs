using Application;
using Application.Services;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Extensions;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Configurations;
using NArchitectureTemplate.Core.ElasticSearch.Models;
using NArchitectureTemplate.Core.Localization.WebApi;
using NArchitectureTemplate.Core.Mailing;
using NArchitectureTemplate.Core.Persistence.WebApi;
using NArchitectureTemplate.Core.Security.Encryption;
using NArchitectureTemplate.Core.Security.JWT;
using NArchitectureTemplate.Core.Security.OAuth.Configurations;
using NArchitectureTemplate.Core.Security.OAuth.Extensions;
using NArchitectureTemplate.Core.Security.OAuth.Middleware;
using NArchitectureTemplate.Core.Security.OAuth.Services;
using NArchitectureTemplate.Core.Security.WebApi.Swagger.Extensions;
using Persistence;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Threading.RateLimiting;
using WebAPI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add environment-specific config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Master key'i ortam deðiþkenlerinden al
var masterKey = Environment.GetEnvironmentVariable("MASTER_KEY");
if (string.IsNullOrEmpty(masterKey))
    throw new Exception("MASTER_KEY ortam deðiþkeni tanýmlanmalý");

// Servis kayýtlarý
builder.Services.AddSingleton<IEncryptionService, EncryptionService>(enc => new EncryptionService(masterKey));
builder.Services.AddSingleton<ILocalSecretsManager>(provider =>
    new LocalSecretsManager(masterKey));

var secretsToLoad = new Dictionary<string, string>
{
    // Database
    ["DatabaseSettings:ConnectionString"] = "DatabaseSettings:ConnectionString",

    // Token
    ["TokenOptions:SecurityKey"] = "TokenOptions:SecurityKey",

    // Security
    ["Security:EncryptionKey"] = "Security:EncryptionKey",
    ["Backup:EncryptionKey"] = "Backup:EncryptionKey",

    // Cloudinary
    ["CloudinaryAccount:ApiSecret"] = "CloudinaryAccount:ApiSecret",

    // MailSettings
    ["MailSettings:Password"] = "MailSettings:Password",
    ["MailSettings:DkimPrivateKey"] = "MailSettings:DkimPrivateKey",
    ["MailSettings:DomainName"] = "MailSettings:DomainName",
    ["MailSettings:SenderEmail"] = "MailSettings:SenderEmail",
    ["MailSettings:Server"] = "MailSettings:Server",
    ["MailSettings:UserName"] = "MailSettings:UserName",

    // Google Authentication
    ["Authentication:Google:ClientId"] = "Authentication:Google:ClientId",
    ["Authentication:Google:ClientSecret"] = "Authentication:Google:ClientSecret",
    ["Authentication:Google:RedirectUri"] = "Authentication:Google:RedirectUri",

    // Facebook Authentication
    ["Authentication:Facebook:AppId"] = "Authentication:Facebook:AppId",
    ["Authentication:Facebook:AppSecret"] = "Authentication:Facebook:AppSecret",
    ["Authentication:Facebook:RedirectUri"] = "Authentication:Facebook:RedirectUri"
};

if (!string.IsNullOrEmpty(masterKey))
{
    // Secrets manager'ý baþlat
    var secretsManager = new LocalSecretsManager(masterKey);

    foreach (var (configPath, secretKey) in secretsToLoad)
    {
        var secretValue = secretsManager.GetSecret(secretKey);
        if (!string.IsNullOrEmpty(secretValue))
        {
            builder.Configuration[configPath] = secretValue;
            Console.WriteLine($"{secretKey} konfigürasyonu secrets manager'dan yüklendi");
        }
        else
        {
            // Ortam deðiþkeni fallback (örnek: Authentication__Google__ClientId)
            var envVarName = secretKey.Replace(":", "__");
            var envValue = Environment.GetEnvironmentVariable(envVarName);

            if (!string.IsNullOrEmpty(envValue))
            {
                builder.Configuration[configPath] = envValue;
                Console.WriteLine($"{secretKey} ortam deðiþkeninden yüklendi: {envVarName}");
            }
            else
            {
                // Kritik olmayanlar için uyarý, kritikler için hata
                if (secretKey.StartsWith("DatabaseSettings") ||
                    secretKey.StartsWith("TokenOptions"))
                {
                    throw new Exception($"Kritik konfigürasyon eksik: {secretKey}");
                }

                Console.WriteLine($"UYARI: {secretKey} konfigürasyonu bulunamadý!");
            }
        }
    }
}
else
{
    // Fallback: Ortam deðiþkenlerinden veya diðer kaynaklardan
    builder.Configuration.AddEnvironmentVariables();
}


builder.Services.AddControllers();
builder.Services.AddAntiforgery();
builder.Services.AddSingleton<RateLimiter>(_ =>
    new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = 100,
        Window = TimeSpan.FromMinutes(1),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0
    }));

builder.Services.Configure<GoogleAuthConfig>(
    builder.Configuration.GetSection("Authentication:Google"));
builder.Services.Configure<FacebookAuthConfig>(
    builder.Configuration.GetSection("Authentication:Facebook"));

EncryptionHelper.Initialize(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    });


builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IFacebookAuthService, FacebookAuthService>();
builder.Services.AddApplicationServices(
    mailSettings: builder.Configuration.GetSection("MailSettings").Get<MailSettings>()
        ?? throw new InvalidOperationException("MailSettings section cannot found in configuration."),
    fileLogConfiguration: builder
        .Configuration.GetSection("SeriLogConfigurations:FileLogConfiguration")
        .Get<FileLogConfiguration>()
        ?? throw new InvalidOperationException("FileLogConfiguration section cannot found in configuration."),
    elasticSearchConfig: builder.Configuration.GetSection("ElasticSearchConfig").Get<ElasticSearchConfig>()
        ?? throw new InvalidOperationException("ElasticSearchConfig section cannot found in configuration."),
    tokenOptions: builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>()
        ?? throw new InvalidOperationException("TokenOptions section cannot found in configuration.")
);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpContextAccessor();

const string tokenOptionsConfigurationSection = "TokenOptions";
TokenOptions tokenOptions =
    builder.Configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
    ?? throw new InvalidOperationException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // Token süresi bitiminden sonra 0 saniye bekle
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
    {
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    })
);
builder.Services.AddSwaggerGen(opt =>
{
    // API'nin genel bilgilerini tanýmlama
    opt.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "Auth API",
        Description = "Authentication ve Authorization iþlemleri için REST API",
        Contact = new OpenApiContact
        {
            Name = "API Geliþtirici Ekibi",
            Email = "bilgi@serdarsevimli.tr"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);

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


builder.Services.AddOAuthSecurity();
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.DocExpansion(DocExpansion.None);
    });
}


//if (app.Environment.IsProduction())
app.ConfigureCustomExceptionMiddleware(); //bu genel exeptions için Core'dan geliyor
app.UseCustomExceptionHandler(); // bu middleware EmergencyNotificationService kullandýðýndan Core'a eklenmedi
// Middleware'ler
app.UseMiddleware<EmergencyMonitoringMiddleware>();
app.UseMiddleware<SecurityAuditMiddleware>();

// Diðer middleware'ler...

// Acil durum endpoint'leri
app.MapEmergencyEndpoints();

app.UseOAuthSecurity();
app.UseMiddleware<OAuthRateLimitMiddleware>();


app.UseDbMigrationApplier();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//const string webApiConfigurationSection = "WebAPIConfiguration";
//WebApiConfiguration webApiConfiguration =
//    app.Configuration.GetSection(webApiConfigurationSection).Get<WebApiConfiguration>()
//    ?? throw new InvalidOperationException($"\"{webApiConfigurationSection}\" section cannot found in configuration.");
//app.UseCors(opt => opt.WithOrigins(webApiConfiguration.AllowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());


var corsPolicies = app.Services
    .GetRequiredService<IOptions<WebApiConfiguration>>()
    .Value
    .Origins;

app.UseCors(opt => opt.WithOrigins(corsPolicies).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

app.UseResponseLocalization();

builder.Services.AddHsts(opts =>
{
    opts.Preload = true;
    opts.IncludeSubDomains = true;
    opts.MaxAge = TimeSpan.FromDays(365);
});

app.UseHttpsRedirection();

// OAuth callback'ler için HTTPS zorunluluðu
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/auth")
        && !context.Request.IsHttps)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("HTTPS required");
        return;
    }
    await next();
});

// Secret yönetimi için endpoint (sadece development)
if (app.Environment.IsDevelopment())
{
    app.MapSecretManagerEndpoints();
}
 

app.Run();