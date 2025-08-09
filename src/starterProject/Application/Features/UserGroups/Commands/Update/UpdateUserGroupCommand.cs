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

namespace Application.Features.UserGroups.Commands.Update;

public class UpdateUserGroupCommand : IRequest<UpdatedUserGroupResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public required int GroupId { get; set; }

    public string[] Roles => [Admin, Write, UserGroupsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserGroups"];

    public class UpdateUserGroupCommandHandler : IRequestHandler<UpdateUserGroupCommand, UpdatedUserGroupResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly UserGroupBusinessRules _userGroupBusinessRules;

        public UpdateUserGroupCommandHandler(IMapper mapper, IUserGroupRepository userGroupRepository,
                                         UserGroupBusinessRules userGroupBusinessRules)
        {
            _mapper = mapper;
            _userGroupRepository = userGroupRepository;
            _userGroupBusinessRules = userGroupBusinessRules;
        }

        public async Task<UpdatedUserGroupResponse> Handle(UpdateUserGroupCommand request, CancellationToken cancellationToken)
        {
            UserGroup? userGroup = await _userGroupRepository.GetAsync(predicate: ug => ug.Id == request.Id, cancellationToken: cancellationToken);
            await _userGroupBusinessRules.UserGroupShouldExistWhenSelected(userGroup);
            userGroup = _mapper.Map(request, userGroup);

            await _userGroupRepository.UpdateAsync(userGroup!);

            UpdatedUserGroupResponse response = _mapper.Map<UpdatedUserGroupResponse>(userGroup);
            return response;
        }
    }
}