using Application.Features.UserSessions.Constants;
using Application.Features.UserSessions.Rules;
using Application.Services.UserSessions;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Commands.Update;

public class UpdateUserSessionCommand : IRequest<UpdatedUserSessionResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string IpAddress { get; set; }
    public required string UserAgent { get; set; }
    public required DateTime LoginTime { get; set; }
    public required bool IsRevoked { get; set; }
    public required bool IsSuspicious { get; set; }
    public string? LocationInfo { get; set; }

    public string[] Roles => [Admin, Write, UserSessionsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserSessions"];

    public class UpdateUserSessionCommandHandler : IRequestHandler<UpdateUserSessionCommand, UpdatedUserSessionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserSessionService _userSessionService;
        private readonly UserSessionBusinessRules _userSessionBusinessRules;

        public UpdateUserSessionCommandHandler(IMapper mapper, IUserSessionService userSessionService,
                                         UserSessionBusinessRules userSessionBusinessRules)
        {
            _mapper = mapper;
            _userSessionService = userSessionService;
            _userSessionBusinessRules = userSessionBusinessRules;
        }

        public async Task<UpdatedUserSessionResponse> Handle(UpdateUserSessionCommand request, CancellationToken cancellationToken)
        {
            UserSession? userSession = await _userSessionService.GetAsync(predicate: us => us.Id == request.Id, cancellationToken: cancellationToken);
            await _userSessionBusinessRules.UserSessionShouldExistWhenSelected(userSession);
            userSession = _mapper.Map(request, userSession);

            await _userSessionService.UpdateAsync(userSession!);

            UpdatedUserSessionResponse response = _mapper.Map<UpdatedUserSessionResponse>(userSession);
            return response;
        }
    }
}