using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserOperationClaims.Constants.UserOperationClaims;

namespace Application.Features.UserOperationClaims.Commands.Update;

public class UpdateUserClaimCommand : IRequest<UpdateUserOperationClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int SecurityClaimId { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserOperationClaims.Update };

    public class UpdateUserClaimCommandHandler
        : IRequestHandler<UpdateUserClaimCommand, UpdateUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userClaimBusinessRules;

        public UpdateUserClaimCommandHandler(
            IUserOperationClaimRepository userClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<UpdateUserOperationClaimResponse> Handle(
            UpdateUserClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            UserOperationClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserClaimShouldExistWhenSelected(userClaim);
            await _userClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenUpdated(
                request.Id,
                request.UserId,
                request.SecurityClaimId
            );
            UserOperationClaim mappedUseClaim = _mapper.Map(request, destination: userClaim!);

            UserOperationClaim updatedUserClaim = await _userClaimRepository.UpdateAsync(
                mappedUseClaim
            );

            UpdateUserOperationClaimResponse updatedUserClaimDto = _mapper.Map<UpdateUserOperationClaimResponse>(
                updatedUserClaim
            );
            return updatedUserClaimDto;
        }
    }
}
