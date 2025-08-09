using Application.Services.Repositories;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class PasswordResetTokenRepository : EfRepositoryBase<ResetPasswordToken, int, BaseDbContext>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(BaseDbContext context) : base(context)
    {
    }
}