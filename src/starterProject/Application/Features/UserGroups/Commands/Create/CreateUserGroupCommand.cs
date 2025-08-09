using Application.Features.UserGroups.Constants;
using Application.Features.UserGroups.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserGroups.Constants.UserGroupsOperationClaims;

namespace Application.Features.UserGroups.Commands.Create;

public class CreateUserGroupCommand : IRequest<CreatedUserGroupResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int UserId { get; set; }
    public required int GroupId { get; set; }

    public string[] Roles => [Admin, Write, UserGroupsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserGroups"];

    public class CreateUserGroupCommandHandler : IRequestHandler<CreateUserGroupCommand, CreatedUserGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly UserGroupBusinessRules _userGroupBusinessRules;

        public CreateUserGroupCommandHandler(IMapper mapper, IUserGroupRepository userGroupRepository,
                                         UserGroupBusinessRules userGroupBusinessRules)
        {
            _mapper = mapper;
            _userGroupRepository = userGroupRepository;
            _userGroupBusinessRules = userGroupBusinessRules;
        }

        public async Task<CreatedUserGroupResponse> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
        {
            UserGroup userGroup = _mapper.Map<UserGroup>(request);

            await _userGroupRepository.AddAsync(userGroup);

            CreatedUserGroupResponse response = _mapper.Map<CreatedUserGroupResponse>(userGroup);
            return response;
        }
    }
}