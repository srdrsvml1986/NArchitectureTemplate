using Domain.Entities;
using NArchitecture.Core.Test.Application.FakeData;
using System;
using StarterProject.Application.Tests.Mocks.FakeDatas; // UserFakeData'a erişim için

namespace StarterProject.Application.Tests.Mocks.FakeDatas
{
    public class SessionFakeData : BaseFakeData<UserSession, Guid>
    {
        private static readonly Random _random = new Random();

        public override List<UserSession> CreateFakeData()
        {
            var userFakeData = new UserFakeData();
            var users = userFakeData.CreateFakeData();

            var locations = new[] { "Istanbul, TR", "Ankara, TR", "Izmir, TR", "Bursa, TR" };
            var userAgents = new[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15",
                "Mozilla/5.0 (Linux; Android 10; SM-G981B) AppleWebKit/537.36",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 14_4 like Mac OS X) AppleWebKit/605.1.15"
            };

            List<UserSession> data = new List<UserSession>();
            int sessionCount = 0;

            foreach (var user in users)
            {
                // Her kullanıcı için 3 oturum oluştur
                for (int i = 0; i < 3; i++)
                {
                    sessionCount++;
                    var isSuspicious = sessionCount % 4 == 0; // Her 4 oturumdan 1'i şüpheli
                    var isRevoked = sessionCount % 3 == 0;   // Her 3 oturumdan 1'i iptal edilmiş

                    data.Add(new UserSession
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        IpAddress = $"192.168.1.{_random.Next(1, 255)}",
                        UserAgent = userAgents[_random.Next(userAgents.Length)],
                        LoginTime = DateTime.UtcNow.AddDays(-_random.Next(1, 30)),
                        IsRevoked = isRevoked,
                        IsSuspicious = isSuspicious,
                        LocationInfo = locations[_random.Next(locations.Length)]
                    });
                }
            }

            return data;
        }
    }
}