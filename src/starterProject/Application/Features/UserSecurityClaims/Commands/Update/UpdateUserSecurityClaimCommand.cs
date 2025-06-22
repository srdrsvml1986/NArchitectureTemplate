using Application.Features.UserClaims.Constants;
using Application.Features.UserClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserClaims.Constants.UserClaims;

namespace Application.Features.UserClaims.Commands.Update;

public class UpdateUserClaimCommand : IRequest<UpdatedUserClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int SecurityClaimId { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserClaims.Update };

    public class UpdateUserClaimCommandHandler
        : IRequestHandler<UpdateUserClaimCommand, UpdatedUserClaimResponse>
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserClaimBusinessRules _userClaimBusinessRules;

        public UpdateUserClaimCommandHandler(
            IUserClaimRepository userClaimRepository,
            IMapper mapper,
            UserClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<UpdatedUserClaimResponse> Handle(
            UpdateUserClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            UserClaim? userClaim = await _userClaimRepository.GetAsync(
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
            UserClaim mappedUseClaim = _mapper.Map(request, destination: userClaim!);

            UserClaim updatedUserClaim = await _userClaimRepository.UpdateAsync(
                mappedUseClaim
            );

            UpdatedUserClaimResponse updatedUserClaimDto = _mapper.Map<UpdatedUserClaimResponse>(
                updatedUserClaim
            );
            return updatedUserClaimDto;
        }
    }
}
