using Application.Features.UserClaims.Constants;
using Application.Features.UserClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserClaims.Constants.UserClaims;

namespace Application.Features.UserClaims.Commands.Create;

public class CreateUserClaimCommand : IRequest<CreatedUserClaimResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public int ClaimId { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserClaims.Create };

    public class CreateUserClaimCommandHandler
        : IRequestHandler<CreateUserClaimCommand, CreatedUserClaimResponse>
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserClaimBusinessRules _userClaimBusinessRules;

        public CreateUserClaimCommandHandler(
            IUserClaimRepository userClaimRepository,
            IMapper mapper,
            UserClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<CreatedUserClaimResponse> Handle(
            CreateUserClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _userClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
                request.UserId,
                request.ClaimId
            );
            UserClaim mappedUserClaim = _mapper.Map<UserClaim>(request);

            UserClaim createdUserClaim = await _userClaimRepository.AddAsync(mappedUserClaim);

            CreatedUserClaimResponse createdUserClaimDto = _mapper.Map<CreatedUserClaimResponse>(
                createdUserClaim
            );
            return createdUserClaimDto;
        }
    }
}
