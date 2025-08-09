using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IPasswordResetTokenRepository : IAsyncRepository<ResetPasswordToken, int>, IRepository<ResetPasswordToken, int>
{
}