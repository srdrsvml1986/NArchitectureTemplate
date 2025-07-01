using Application.Features.Groups.Constants;
using Application.Features.Groups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Groups.Constants.GroupsOperationClaims;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Groups.Queries.GetClaimsByGroupId;

public class GetClaimsByGroupIdGroupQuery : IRequest<GetClaimsByGroupIdGroupResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetClaimsByGroupIdGroupQueryHandler : IRequestHandler<GetClaimsByGroupIdGroupQuery, GetClaimsByGroupIdGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly IClaimRepository _claimRepository;
        private readonly GroupBusinessRules _groupBusinessRules;

        public GetClaimsByGroupIdGroupQueryHandler(IMapper mapper, IGroupRepository groupRepository, GroupBusinessRules groupBusinessRules, IGroupClaimRepository groupClaimRepository, IClaimRepository claimRepository)
        {
            _mapper = mapper;
            _groupRepository = groupRepository;
            _groupBusinessRules = groupBusinessRules;
            _groupClaimRepository = groupClaimRepository;
            _claimRepository = claimRepository;
        }

        public async Task<GetClaimsByGroupIdGroupResponse> Handle(GetClaimsByGroupIdGroupQuery request, CancellationToken cancellationToken)
        {
            Group? group = await _groupRepository.GetAsync(predicate: g => g.Id == request.Id, cancellationToken: cancellationToken);
            await _groupBusinessRules.GroupShouldExistWhenSelected(group);

            var groupClaims = await _groupClaimRepository.GetListAsync(
     predicate: x => x.GroupId == request.Id,
     cancellationToken: cancellationToken
 );

            var claimIds = groupClaims.Items.Select(x => x.ClaimId).ToList();

            var claims = _claimRepository.Query().Where(x => claimIds.Contains(x.Id));

            GetClaimsByGroupIdGroupResponse response = _mapper.Map<GetClaimsByGroupIdGroupResponse>(
                new GetClaimsByGroupIdGroupResponse {Claims=claims });
            return response;
        }
    }
}