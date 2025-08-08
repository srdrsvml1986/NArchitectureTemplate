using Application.Features.Groups.Constants;
using Application.Features.Groups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Groups.Constants.GroupsOperationClaims;
using Domain.DTos;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Groups.Commands.UpdateClaimsInGroup;

public class UpdateClaimsInGroupCommand : IRequest<UpdateClaimsInGroupResponse>, ISecuredRequest
{
    public int GroupId { get; set; }
    public IList<int> ClaimIds { get; set; }

    public string[] Roles => [Admin, Write, GroupsOperationClaims.Update];

    public class UpdateClaimsInGroupCommandHandler : IRequestHandler<UpdateClaimsInGroupCommand, UpdateClaimsInGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly IOperationClaimRepository _claimRepository;
        private readonly GroupBusinessRules _groupBusinessRules;

        public UpdateClaimsInGroupCommandHandler(
            IMapper mapper,
            IGroupRepository groupRepository,
            IGroupOperationClaimRepository groupClaimRepository,
            IOperationClaimRepository claimRepository,
            GroupBusinessRules groupBusinessRules)
        {
            _mapper = mapper;
            _groupRepository = groupRepository;
            _groupClaimRepository = groupClaimRepository;
            _claimRepository = claimRepository;
            _groupBusinessRules = groupBusinessRules;
        }

        public async Task<UpdateClaimsInGroupResponse> Handle(UpdateClaimsInGroupCommand request, CancellationToken cancellationToken)
        {
            await _groupBusinessRules.GroupIdShouldExistWhenSelected(request.GroupId, cancellationToken);

            // Validate group exists
            await _groupBusinessRules.GroupIdShouldExistWhenSelected(request.GroupId, cancellationToken);

            // Validate all claim IDs exist
            await _groupBusinessRules.ClaimsShouldExistWhenSelected(request.ClaimIds, cancellationToken);

            // Get existing group claims
            IPaginate<GroupOperationClaim> existingGroupClaims = await _groupClaimRepository.GetListAsync(
                predicate: x => x.GroupId == request.GroupId,
                cancellationToken: cancellationToken
            );

            // Calculate changes
            var existingClaimIds = existingGroupClaims.Items.Select(x => x.OperationClaimId).ToList();
            var claimsToAdd = request.ClaimIds.Except(existingClaimIds)
                .Select(claimId => new GroupOperationClaim
                {
                    GroupId = request.GroupId,
                    OperationClaimId = claimId
                })
                .ToList();

            var claimsToRemove = existingGroupClaims.Items
                .Where(x => !request.ClaimIds.Contains(x.OperationClaimId))
                .ToList();

            // Apply changes
            if (claimsToAdd.Any())
                await _groupClaimRepository.AddRangeAsync(claimsToAdd, cancellationToken);

            if (claimsToRemove.Any())
                await _groupClaimRepository.DeleteRangeAsync(claimsToRemove);

            // Get updated claims
            IPaginate<OperationClaim> updatedClaims = await _claimRepository.GetListAsync(
                predicate: x => request.ClaimIds.Contains(x.Id),
                cancellationToken: cancellationToken
            );

            // Map to DTOs
            var claimDtos = _mapper.Map<IList<OperationClaimDto>>(updatedClaims.Items);

            return new UpdateClaimsInGroupResponse { Claims = claimDtos };
        }
    }
}
