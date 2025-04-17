using Application.Features.UserSecurityClaims.Constants;
using Application.Features.UserSecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSecurityClaims.Constants.UserSecurityClaims;

namespace Application.Features.UserSecurityClaims.Commands.Update;

public class UpdateUserSecurityClaimCommand : IRequest<UpdatedUserSecurityClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int OperationClaimId { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserSecurityClaims.Update };

    public class UpdateUserClaimCommandHandler
        : IRequestHandler<UpdateUserSecurityClaimCommand, UpdatedUserSecurityClaimResponse>
    {
        private readonly IUserSecurityClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserSecurityClaimBusinessRules _userClaimBusinessRules;

        public UpdateUserClaimCommandHandler(
            IUserSecurityClaimRepository userClaimRepository,
            IMapper mapper,
            UserSecurityClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<UpdatedUserSecurityClaimResponse> Handle(
            UpdateUserSecurityClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            UserSecurityClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserSecurityClaimShouldExistWhenSelected(userClaim);
            await _userClaimBusinessRules.UserShouldNotHasClaimAlreadyWhenUpdated(
                request.Id,
                request.UserId,
                request.OperationClaimId
            );
            UserSecurityClaim mappedUseClaim = _mapper.Map(request, destination: userClaim!);

            UserSecurityClaim updatedUserClaim = await _userClaimRepository.UpdateAsync(
                mappedUseClaim
            );

            UpdatedUserSecurityClaimResponse updatedUserClaimDto = _mapper.Map<UpdatedUserSecurityClaimResponse>(
                updatedUserClaim
            );
            return updatedUserClaimDto;
        }
    }
}
