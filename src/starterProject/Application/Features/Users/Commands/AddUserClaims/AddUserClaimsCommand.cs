using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
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
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public AddUserClaimsCommandHandler(
            IUserRepository userRepository,
            IUserClaimRepository userClaimRepository,
            IClaimRepository claimRepository,
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

            var existingClaimIds = existingUserClaims.Items.Select(uc => uc.ClaimId).ToList();

            // Sadece yeni olan claim'leri ekle
            var newClaimsToAdd = request.ClaimIds
                .Except(existingClaimIds)
                .Select(claimId => new UserClaim { UserId = request.UserId, ClaimId = claimId })
                .ToList();

            if (newClaimsToAdd.Any())
                await _userClaimRepository.AddRangeAsync(newClaimsToAdd, cancellationToken);

            var addedClaims = _claimRepository.Query().Where(c => newClaimsToAdd.Select(nc => nc.ClaimId).Contains(c.Id));

            return new AddUserClaimsResponse { Claims = addedClaims };
        }
    }
}
