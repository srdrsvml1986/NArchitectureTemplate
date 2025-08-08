using Application.Features.Groups.Constants;
using Application.Features.Groups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Groups.Constants.GroupsOperationClaims;
using NArchitecture.Core.Persistence.Paging;
using Domain.DTos;

namespace Application.Features.Groups.Queries.GetClaimsByGroupId;

public class GetClaimsByGroupIdGroupQuery : IRequest<GetClaimsByGroupIdGroupResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetClaimsByGroupIdGroupQueryHandler : IRequestHandler<GetClaimsByGroupIdGroupQuery, GetClaimsByGroupIdGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly IOperationClaimRepository _claimRepository;
        private readonly GroupBusinessRules _groupBusinessRules;

        public GetClaimsByGroupIdGroupQueryHandler(IMapper mapper, IGroupRepository groupRepository, GroupBusinessRules groupBusinessRules, IGroupOperationClaimRepository groupClaimRepository, IOperationClaimRepository claimRepository)
        {
            _mapper = mapper;
            _groupRepository = groupRepository;
            _groupBusinessRules = groupBusinessRules;
            _groupClaimRepository = groupClaimRepository;
            _claimRepository = claimRepository;
        }

        public async Task<GetClaimsByGroupIdGroupResponse> Handle(GetClaimsByGroupIdGroupQuery request, CancellationToken cancellationToken)
        {
            Group? group = await _groupRepository.GetAsync(
        predicate: g => g.Id == request.Id,
        cancellationToken: cancellationToken
    );
            await _groupBusinessRules.GroupShouldExistWhenSelected(group);

            // Grup claim'lerini al
            IPaginate<GroupOperationClaim> groupClaims = await _groupClaimRepository.GetListAsync(
                predicate: x => x.GroupId == request.Id,
                cancellationToken: cancellationToken
            );

            // Claim ID'leri çýkar
            List<int> claimIds = groupClaims.Items
                .Select(x => x.OperationClaimId)
                .ToList();

            // Claim nesnelerini al
            IList<OperationClaim> claims = (await _claimRepository.GetListAsync(
                predicate: x => claimIds.Contains(x.Id),
                cancellationToken: cancellationToken
            )).Items.ToList(); // Materialize the list

            // DTO'ya map et (döngüyü kýr)
            IList<OperationClaimDto> claimDtos = _mapper.Map<IList<OperationClaimDto>>(claims);

            return new GetClaimsByGroupIdGroupResponse { Claims = claimDtos };
        }
    }
}