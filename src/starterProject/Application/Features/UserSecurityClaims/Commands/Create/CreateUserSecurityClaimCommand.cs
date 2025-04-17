using Application.Features.UserSecurityClaims.Constants;
using Application.Features.UserSecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSecurityClaims.Constants.UserSecurityClaims;

namespace Application.Features.UserSecurityClaims.Commands.Create;

public class CreateUserSecurityClaimCommand : IRequest<CreatedUserSecurityClaimResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public int ClaimId { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserSecurityClaims.Create };

    public class CreateUserClaimCommandHandler
        : IRequestHandler<CreateUserSecurityClaimCommand, CreatedUserSecurityClaimResponse>
    {
        private readonly IUserSecurityClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserSecurityClaimBusinessRules _userClaimBusinessRules;

        public CreateUserClaimCommandHandler(
            IUserSecurityClaimRepository userClaimRepository,
            IMapper mapper,
            UserSecurityClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<CreatedUserSecurityClaimResponse> Handle(
            CreateUserSecurityClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _userClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
                request.UserId,
                request.ClaimId
            );
            UserSecurityClaim mappedUserClaim = _mapper.Map<UserSecurityClaim>(request);

            UserSecurityClaim createdUserClaim = await _userClaimRepository.AddAsync(mappedUserClaim);

            CreatedUserSecurityClaimResponse createdUserClaimDto = _mapper.Map<CreatedUserSecurityClaimResponse>(
                createdUserClaim
            );
            return createdUserClaimDto;
        }
    }
}
