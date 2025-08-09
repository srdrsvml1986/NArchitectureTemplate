using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.UserOperationClaims.Constants.UserOperationClaims;

namespace Application.Features.UserOperationClaims.Commands.Create;

public class CreateUserOperationClaimCommand : IRequest<CreateUserOperationClaimResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public int OperationClaimId { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserOperationClaims.Create };

    public class CreateUserOperationClaimCommandHandler
        : IRequestHandler<CreateUserOperationClaimCommand, CreateUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userClaimBusinessRules;

        public CreateUserOperationClaimCommandHandler(
            IUserOperationClaimRepository userClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<CreateUserOperationClaimResponse> Handle(
            CreateUserOperationClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            await _userClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenInsert(
                request.UserId,
                request.OperationClaimId
            );
            UserOperationClaim mappedUserClaim = _mapper.Map<UserOperationClaim>(request);

            UserOperationClaim createdUserClaim = await _userClaimRepository.AddAsync(mappedUserClaim);

            CreateUserOperationClaimResponse createdUserClaimDto = _mapper.Map<CreateUserOperationClaimResponse>(
                createdUserClaim
            );
            return createdUserClaimDto;
        }
    }
}
