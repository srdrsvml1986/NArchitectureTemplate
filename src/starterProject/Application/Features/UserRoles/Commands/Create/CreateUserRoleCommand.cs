using Application.Features.UserRoles.Constants;
using Application.Features.UserRoles.Rules;
using Application.Services.UserRoles;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserRoles.Constants.UserRolesOperationClaims;

namespace Application.Features.UserRoles.Commands.Create;

public class CreateUserRoleCommand : IRequest<CreatedUserRoleResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid UserId { get; set; }
    public required int RoleId { get; set; }

    public string[] Roles => [Admin, Write, UserRolesOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserRoles"];

    public class CreateUserRoleCommandHandler : IRequestHandler<CreateUserRoleCommand, CreatedUserRoleResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserRoleService _userRoleService;
        private readonly UserRoleBusinessRules _userRoleBusinessRules;

        public CreateUserRoleCommandHandler(IMapper mapper, IUserRoleService userRoleService,
                                         UserRoleBusinessRules userRoleBusinessRules)
        {
            _mapper = mapper;
            _userRoleService = userRoleService;
            _userRoleBusinessRules = userRoleBusinessRules;
        }

        public async Task<CreatedUserRoleResponse> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
        {
            UserRole userRole = _mapper.Map<UserRole>(request);

            await _userRoleService.AddAsync(userRole);

            CreatedUserRoleResponse response = _mapper.Map<CreatedUserRoleResponse>(userRole);
            return response;
        }
    }
}