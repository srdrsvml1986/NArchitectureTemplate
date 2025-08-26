using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class UserNotificationSettingConfiguration : IEntityTypeConfiguration<UserNotificationSetting>
{
    public void Configure(EntityTypeBuilder<UserNotificationSetting> builder)
    {
        builder.ToTable("UserNotificationSettings").HasKey(uns => uns.Id);

        builder.Property(uns => uns.Id).HasColumnName("Id").IsRequired();
        builder.Property(uns => uns.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(uns => uns.NotificationType).HasColumnName("NotificationType").IsRequired();
        builder.Property(uns => uns.NotificationChannel).HasColumnName("NotificationChannel").IsRequired();
        builder.Property(uns => uns.IsEnabled).HasColumnName("IsEnabled").IsRequired();
        builder.Property(uns => uns.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(uns => uns.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(uns => uns.DeletedDate).HasColumnName("DeletedDate");

        // Enum conversions
        builder.Property(e => e.NotificationType)
            .HasConversion<string>();

        builder.Property(e => e.NotificationChannel)
            .HasConversion<string>();

        // Foreign key relationship
        builder.HasOne(uns => uns.User)
            .WithMany(u => u.UserNotificationSetting)
            .HasForeignKey(uns => uns.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint
        builder
            .HasIndex(u => new { u.UserId, u.NotificationType, u.NotificationChannel })
            .IsUnique();

        builder.HasData(GetSeedData());

        builder.HasQueryFilter(uns => !uns.DeletedDate.HasValue);
    }

    private static List<UserNotificationSetting> GetSeedData()
    {
        var settings = new List<UserNotificationSetting>();

        var notificationTypes = Enum.GetValues(typeof(NotificationType)).Cast<NotificationType>();
        var notificationChannels = Enum.GetValues(typeof(NotificationChannel)).Cast<NotificationChannel>();

        int id = 1;
        foreach (var type in notificationTypes)
        {
            foreach (var channel in notificationChannels)
            {
                settings.Add(new UserNotificationSetting
                {
                    Id = Guid.NewGuid(),
                    UserId = UserConfiguration.AdminId,
                    NotificationType = type,
                    NotificationChannel = channel,
                    IsEnabled = GetDefaultSetting(type, channel),
                    CreatedDate = DateTime.UtcNow
                });
            }
        }

        return settings;
    }

    private static bool GetDefaultSetting(NotificationType type, NotificationChannel channel)
    {
        // Varsayýlan ayarlar - proje gereksinimlerinize göre özelleþtirin
        return (type, channel) switch
        {
            (NotificationType.SMS, _) => false, // SMS varsayýlan kapalý
            (_, NotificationChannel.Promotion) => false, // Promosyonlar varsayýlan kapalý
            (NotificationType.InApp, NotificationChannel.SecurityAlert) => true, // Güvenlik uyarýlarý InApp açýk
            (NotificationType.PushNotification, NotificationChannel.SecurityAlert) => true, // Güvenlik uyarýlarý Push açýk
            (NotificationType.Email, NotificationChannel.SecurityAlert) => true, // Güvenlik uyarýlarý Email açýk
            _ => true // Diðer tüm kombinasyonlar varsayýlan açýk
        };
    }
}