using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Commands.AddUserClaims;

public class AddUserClaimsCommand : IRequest<AddUserClaimsResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public IList<int> ClaimIds { get; set; }

    public string[] Roles => new[] { Admin, Write, UsersOperationClaims.Create };

    public class AddUserClaimsCommandHandler : IRequestHandler<AddUserClaimsCommand, AddUserClaimsResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IOperationClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public AddUserClaimsCommandHandler(
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

        public async Task<AddUserClaimsResponse> Handle(AddUserClaimsCommand request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var existingUserClaims = await _userClaimRepository.GetListAsync(
                predicate: uc => uc.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var existingClaimIds = existingUserClaims.Items.Select(uc => uc.OperationClaimId).ToList();

            // Sadece yeni olan claim'leri ekle
            var newClaimsToAdd = request.ClaimIds
                .Except(existingClaimIds)
                .Select(claimId => new UserOperationClaim { UserId = request.UserId, OperationClaimId = claimId })
                .ToList();

            if (newClaimsToAdd.Any())
                await _userClaimRepository.AddRangeAsync(newClaimsToAdd,true, cancellationToken);

            var addedClaims = _claimRepository.Query().Where(c => newClaimsToAdd.Select(nc => nc.OperationClaimId).Contains(c.Id));

            return new AddUserClaimsResponse { Claims = addedClaims };
        }
    }
}
