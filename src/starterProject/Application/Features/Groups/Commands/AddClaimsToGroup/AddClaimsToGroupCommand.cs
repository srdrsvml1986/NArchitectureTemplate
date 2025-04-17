using Application.Features.Groups.Constants;
using Application.Features.Groups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Groups.Constants.GroupsOperationClaims;

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
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly ISecurityClaimRepository _claimRepository;
        private readonly GroupBusinessRules _groupBusinessRules;

        public AddClaimsToGroupCommandHandler(
            IMapper mapper,
            IGroupRepository groupRepository,
            IGroupClaimRepository groupClaimRepository,
            ISecurityClaimRepository claimRepository,
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
            await _groupBusinessRules.GroupIdShouldExistWhenSelected(request.GroupId, cancellationToken);

            List<GroupClaim> groupClaimsToAdd = request.ClaimIds
                .Select(claimId => new GroupClaim { GroupId = request.GroupId, ClaimId = claimId })
                .ToList();

            await _groupClaimRepository.AddRangeAsync(groupClaimsToAdd, cancellationToken);

            var claims = _claimRepository.Query().Where(x => request.ClaimIds.Contains(x.Id));

            return new AddClaimsToGroupResponse { Claims = claims };
        }
    }
}

