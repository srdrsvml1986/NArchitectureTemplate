using Application.Services.Repositories;
using Moq;
using StarterProject.Application.Tests.Mocks.FakeDatas;

namespace StarterProject.Application.Tests.Mocks.Repositories.Auth;

public class MockUserSecurityClaimRepository
{
    private readonly OperationClaimFakeData _operationClaimFakeData;

    public MockUserSecurityClaimRepository(OperationClaimFakeData operationClaimFakeData)
    {
        _operationClaimFakeData = operationClaimFakeData;
    }

    public IUserSecurityClaimRepository GetMockUserSecurityClaimRepository()
    {
        List<Domain.Entities.SecurityClaim> operationClaims = _operationClaimFakeData.Data;
        var mockRepo = new Mock<IUserSecurityClaimRepository>();

        mockRepo
            .Setup(s => s.GetSecurityClaimsByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(
                (Guid userId) =>
                {
                    var claims = operationClaims.ToList();
                    return claims;
                }
            );

        return mockRepo.Object;
    }
}
