using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Responses;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Queries.GetGroupsByUserId;

public class GetGroupsByUserIdQuery : IRequest<GetGroupsByUserIdResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Read];

    public class GetGroupsByUserIdQueryHandler : IRequestHandler<GetGroupsByUserIdQuery, GetGroupsByUserIdResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public GetGroupsByUserIdQueryHandler(
            IUserRepository userRepository,
            IUserGroupRepository userGroupRepository,
            IGroupRepository groupRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<GetGroupsByUserIdResponse> Handle(GetGroupsByUserIdQuery request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var userGroups = await _userGroupRepository.GetListAsync(
                predicate: ug => ug.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var groupIds = userGroups.Items.Select(ug => ug.GroupId).ToList();
            var groups = _groupRepository.Query().Where(g => groupIds.Contains(g.Id));

            return new GetGroupsByUserIdResponse { Groups = groups };
        }
    }
}

public class GetGroupsByUserIdResponse : IResponse
{
    public IQueryable<Group>? Groups { get; set; }
}
