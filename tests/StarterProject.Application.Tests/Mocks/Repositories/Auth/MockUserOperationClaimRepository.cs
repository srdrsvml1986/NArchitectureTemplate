using Application.Services.Repositories;
using Moq;
using StarterProject.Application.Tests.Mocks.FakeDatas;

namespace StarterProject.Application.Tests.Mocks.Repositories.Auth;

public class MockUserClaimRepository
{
    private readonly OperationClaimFakeData _operationClaimFakeData;

    public MockUserClaimRepository(OperationClaimFakeData operationClaimFakeData)
    {
        _operationClaimFakeData = operationClaimFakeData;
    }

    public IUserOperationClaimRepository GetMockUserClaimRepository()
    {
        List<Domain.Entities.OperationClaim> operationClaims = _operationClaimFakeData.Data;
        var mockRepo = new Mock<IUserOperationClaimRepository>();

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
