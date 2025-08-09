using Application.Features.Groups.Constants;
using Application.Features.Groups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Groups.Constants.GroupsOperationClaims;
using Domain.DTos;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.Groups.Commands.AddClaimsToGroup;

public class AddClaimsToGroupCommand : IRequest<AddClaimsToGroupResponse>, ISecuredRequest
{
    public int GroupId { get; set; }
    public IList<int> ClaimIds { get; set; }

    public string[] Roles => [Admin, Write];

    public class AddClaimsToGroupCommandHandler : IRequestHandler<AddClaimsToGroupCommand, AddClaimsToGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly IOperationClaimRepository _claimRepository;
        private readonly GroupBusinessRules _groupBusinessRules;

        public AddClaimsToGroupCommandHandler(
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

        public async Task<AddClaimsToGroupResponse> Handle(AddClaimsToGroupCommand request, CancellationToken cancellationToken)
        {
            // Validate group exists
            await _groupBusinessRules.GroupIdShouldExistWhenSelected(request.GroupId, cancellationToken);

            // Validate all claim IDs exist
            await _groupBusinessRules.ClaimsShouldExistWhenSelected(request.ClaimIds, cancellationToken);

            // Check for duplicate claims
            IPaginate<GroupOperationClaim> existingClaims = await _groupClaimRepository.GetListAsync(
                predicate: x =>
                    x.GroupId == request.GroupId &&
                    request.ClaimIds.Contains(x.OperationClaimId),
                cancellationToken: cancellationToken
            );

            // Filter out existing claims
            var newClaimIds = request.ClaimIds
                .Except(existingClaims.Items.Select(x => x.OperationClaimId))
                .ToList();

            // Create new group-claim relationships
            List<GroupOperationClaim> groupClaimsToAdd = newClaimIds
                .Select(claimId => new GroupOperationClaim
                {
                    GroupId = request.GroupId,
                    OperationClaimId = claimId
                })
                .ToList();

            // Add new claims
            if (groupClaimsToAdd.Any())
            {
                await _groupClaimRepository.AddRangeAsync(groupClaimsToAdd,true, cancellationToken);
            }

            // Get added claims
            IPaginate<OperationClaim> claims = await _claimRepository.GetListAsync(
                predicate: x => newClaimIds.Contains(x.Id),
                cancellationToken: cancellationToken
            );

            // Map to DTOs
            var claimDtos = _mapper.Map<IList<OperationClaimDto>>(claims.Items);

            return new AddClaimsToGroupResponse { Claims = claimDtos };
        }
    }
}

