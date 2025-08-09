using Application.Features.UserSessions.Constants;
using Application.Features.UserSessions.Rules;
using Application.Services.UserSessions;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Commands.Delete;

public class DeleteUserSessionCommand : IRequest<DeletedUserSessionResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, UserSessionsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserSessions"];

    public class DeleteUserSessionCommandHandler : IRequestHandler<DeleteUserSessionCommand, DeletedUserSessionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserSessionService _userSessionService;
        private readonly UserSessionBusinessRules _userSessionBusinessRules;

        public DeleteUserSessionCommandHandler(IMapper mapper, IUserSessionService userSessionService,
                                         UserSessionBusinessRules userSessionBusinessRules)
        {
            _mapper = mapper;
            _userSessionService = userSessionService;
            _userSessionBusinessRules = userSessionBusinessRules;
        }

        public async Task<DeletedUserSessionResponse> Handle(DeleteUserSessionCommand request, CancellationToken cancellationToken)
        {
            UserSession? userSession = await _userSessionService.GetAsync(predicate: us => us.Id == request.Id, cancellationToken: cancellationToken);
            await _userSessionBusinessRules.UserSessionShouldExistWhenSelected(userSession);

            await _userSessionService.DeleteAsync(userSession!);

            DeletedUserSessionResponse response = _mapper.Map<DeletedUserSessionResponse>(userSession);
            return response;
        }
    }
}