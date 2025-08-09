using Application.Features.UserOperationClaims.Commands.Update;
using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.UpdateUserClaims;

public class UpdateUserClaimsCommand : IRequest<UpdateUserClaimsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public IList<int> ClaimIds { get; set; }

    public string[] Roles => new[] { Admin, Write, UsersOperationClaims.Update };

    public class UpdateUserClaimsCommandHandler : IRequestHandler<UpdateUserClaimsCommand, UpdateUserClaimsResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IOperationClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public UpdateUserClaimsCommandHandler(
            IUserRepository userRepository,
            IUserOperationClaimRepository userClaimRepository,
            IOperationClaimRepository claimRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userClaimRepository = userClaimRepository;
            _claimRepository = claimRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<UpdateUserClaimsResponse> Handle(UpdateUserClaimsCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var existingUserClaims = await _userClaimRepository.GetListAsync(
                predicate: uc => uc.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var existingClaimIds = existingUserClaims.Items.Select(uc => uc.OperationClaimId).ToList();

            // Yeni eklenecek claim'ler
            var claimsToAdd = request.ClaimIds
                .Except(existingClaimIds)
                .Select(claimId => new UserOperationClaim { UserId = request.UserId, OperationClaimId = claimId })
                .ToList();

            // Silinecek claim'ler
            var claimsToRemove = existingUserClaims.Items
                .Where(uc => !request.ClaimIds.Contains(uc.OperationClaimId))
                .ToList();

            if (claimsToAdd.Any())
                await _userClaimRepository.AddRangeAsync(claimsToAdd, true, cancellationToken);

            if (claimsToRemove.Any())
                await _userClaimRepository.DeleteRangeAsync(claimsToRemove);

            var updatedClaims = _claimRepository.Query().Where(c => request.ClaimIds.Contains(c.Id));

            return new UpdateUserClaimsResponse { Claims = updatedClaims };
        }
    }
}
