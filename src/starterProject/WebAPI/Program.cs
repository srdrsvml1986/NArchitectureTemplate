using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
using Persistence.Contexts;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Threading.RateLimiting;
using WebAPI.Configrations;
using WebAPI.Extensions;
using WebAPI.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add environment-specific config
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
// HTTP client'larý kaydet
builder.Services.AddHttpClient("VodafoneSms", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Sms:Vodafone:BaseUrl"] ?? "https://api.vodafone.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("TurkcellSms", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Sms:Turkcell:BaseUrl"] ?? "https://mesajussu.turkcell.com.tr/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSecretsManagement(builder.Configuration);
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
        ?? throw new InvalidOperationException("TokenOptions section cannot found in configuration."),
    loggingConfig: builder.Configuration.GetSection("LoggingConfig").Get<LoggingConfig>()
    ?? throw new InvalidOperationException("loggingConfig section cannot found in configuration.")
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

builder.Services.AddHsts(opts =>
{
    opts.Preload = true;
    opts.IncludeSubDomains = true;
    opts.MaxAge = TimeSpan.FromDays(365);
});

builder.Services.AddOAuthSecurity();
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.DocExpansion(DocExpansion.None);
        opt.SwaggerEndpoint("/swagger/v2/swagger.json", "Auth API v2");
    });
}
//// Middleware'ler
app.ConfigureCustomExceptionMiddleware(); //bu genel exeptions için Core'dan geliyor
app.UseCustomExceptionHandler(); // bu middleware EmergencyNotificationService kullandýðýndan Core'a eklenmedi
app.UseMiddleware<EmergencyMonitoringMiddleware>();
app.UseMiddleware<SecurityAuditMiddleware>();
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

app.UseHsts();

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
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapSecretManagerEndpoints();
    // Acil durum endpoint'leri
    app.MapEmergencyEndpoints();
}

//// Database migration iþlemleri
//// Bu kýsým, EF Core migration'larýný uygulamak için kullanýlýr.
//// Eðer veritabaný henüz oluþturulmamýþsa, önce veritabanýný oluþturur, ardýndan migration'larý uygular.
//// Bu iþlem, uygulama baþlatýlýrken otomatik olarak yapýlýr.
//// Bu, genellikle geliþtirme ortamýnda kullanýlýr.
//using (var scope = app.Services.CreateScope())
//{
//    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

//    if (env.IsDevelopment() || env.IsStaging())
//    {
//        var dbContext = scope.ServiceProvider.GetRequiredService<BaseDbContext>();

//        if (!dbContext.Database.GetMigrations().Any())
//        {
//            dbContext.Database.EnsureCreated();
//        }
//        else
//        {
//            dbContext.Database.Migrate();
//        }

//        dbContext.Database.Migrate();
//    }
//}

// Uygulama baþlarken örnek log
using (var scope = app.Services.CreateScope())
{
    try
    {
        var logger = scope.ServiceProvider.GetRequiredService<NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.ILogger>();
        logger.Information($"Uygulama baþlatýldý: {DateTime.UtcNow}, Environment: {builder.Environment.EnvironmentName}");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Loglama hatasý: {ex.Message}");
    }
}

app.Run();