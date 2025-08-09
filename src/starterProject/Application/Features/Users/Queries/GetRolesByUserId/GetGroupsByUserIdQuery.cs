using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Responses;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Queries.GetRolesByUserId;

public class GetRolesByUserIdQuery : IRequest<GetRolesByUserIdResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Read];

    public class GetRolesByUserIdQueryHandler : IRequestHandler<GetRolesByUserIdQuery, GetRolesByUserIdResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public GetRolesByUserIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<GetRolesByUserIdResponse> Handle(GetRolesByUserIdQuery request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var userRoles = await _userRoleRepository.GetListAsync(
                predicate: ug => ug.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var groupIds = userRoles.Items.Select(ug => ug.RoleId).ToList();
            var roles = _roleRepository.Query().Where(g => groupIds.Contains(g.Id));

            return new GetRolesByUserIdResponse { Roles = roles };
        }
    }
}

public class GetRolesByUserIdResponse : IResponse
{
    public IQueryable<Role>? Roles { get; set; }
}
