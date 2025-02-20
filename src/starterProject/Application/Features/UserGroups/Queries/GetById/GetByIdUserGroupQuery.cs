using Application.Features.UserGroups.Constants;
using Application.Features.UserGroups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.UserGroups.Constants.UserGroupsOperationClaims;

namespace Application.Features.UserGroups.Queries.GetById;

public class GetByIdUserGroupQuery : IRequest<GetByIdUserGroupResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdUserGroupQueryHandler : IRequestHandler<GetByIdUserGroupQuery, GetByIdUserGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly UserGroupBusinessRules _userGroupBusinessRules;

        public GetByIdUserGroupQueryHandler(IMapper mapper, IUserGroupRepository userGroupRepository, UserGroupBusinessRules userGroupBusinessRules)
        {
            _mapper = mapper;
            _userGroupRepository = userGroupRepository;
            _userGroupBusinessRules = userGroupBusinessRules;
        }

        public async Task<GetByIdUserGroupResponse> Handle(GetByIdUserGroupQuery request, CancellationToken cancellationToken)
        {
            UserGroup? userGroup = await _userGroupRepository.GetAsync(predicate: ug => ug.Id == request.Id, cancellationToken: cancellationToken);
            await _userGroupBusinessRules.UserGroupShouldExistWhenSelected(userGroup);

            GetByIdUserGroupResponse response = _mapper.Map<GetByIdUserGroupResponse>(userGroup);
            return response;
        }
    }
}