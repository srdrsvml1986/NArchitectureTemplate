using Application.Features.UserRoles.Constants;
using Application.Features.UserRoles.Rules;
using Application.Services.UserRoles;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.UserRoles.Constants.UserRolesOperationClaims;

namespace Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleQuery : IRequest<GetByIdUserRoleResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdUserRoleQueryHandler : IRequestHandler<GetByIdUserRoleQuery, GetByIdUserRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserRoleService _userRoleService;
        private readonly UserRoleBusinessRules _userRoleBusinessRules;

        public GetByIdUserRoleQueryHandler(IMapper mapper, IUserRoleService userRoleService, UserRoleBusinessRules userRoleBusinessRules)
        {
            _mapper = mapper;
            _userRoleService = userRoleService;
            _userRoleBusinessRules = userRoleBusinessRules;
        }

        public async Task<GetByIdUserRoleResponse> Handle(GetByIdUserRoleQuery request, CancellationToken cancellationToken)
        {
            UserRole? userRole = await _userRoleService.GetAsync(predicate: ur => ur.Id == request.Id, cancellationToken: cancellationToken);
            await _userRoleBusinessRules.UserRoleShouldExistWhenSelected(userRole);

            GetByIdUserRoleResponse response = _mapper.Map<GetByIdUserRoleResponse>(userRole);
            return response;
        }
    }
}