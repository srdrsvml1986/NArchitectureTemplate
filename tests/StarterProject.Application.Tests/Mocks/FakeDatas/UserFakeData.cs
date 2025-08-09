using Domain.Entities;
using NArchitectureTemplate.Core.Security.Hashing;
using NArchitectureTemplate.Core.Test.Application.FakeData;

namespace StarterProject.Application.Tests.Mocks.FakeDatas;

public class UserFakeData : BaseFakeData<User, Guid>
{
    public static Guid[] Ids = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];

    public override List<User> CreateFakeData()
    {
        HashingHelper.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);

        List<User> data =
        [
            new User
            {
                Id = Ids[0],
                Email = "example@serdarsevimli.tr",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.Now,
            },
            new User
            {
                Id = Ids[1],
                Email = "example2@serdarsevimli.tr",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.Now,
            },
            new User
            {
                Id = Ids[2],
                Email = "example3@serdarsevimli.tr",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedDate = DateTime.Now,
            }
        ];
        return data;
    }
}
