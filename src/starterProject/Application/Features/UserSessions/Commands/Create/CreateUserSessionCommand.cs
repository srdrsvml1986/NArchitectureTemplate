using Application.Features.UserSessions.Constants;
using Application.Features.UserSessions.Rules;
using Application.Services.UserSessions;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Pipelines.Caching;
using NArchitectureTemplate.Core.Application.Pipelines.Logging;
using NArchitectureTemplate.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Commands.Create;

public class CreateUserSessionCommand : IRequest<CreatedUserSessionResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required Guid UserId { get; set; }
    public required string IpAddress { get; set; }
    public required string UserAgent { get; set; }
    public required DateTime LoginTime { get; set; }
    public required bool IsRevoked { get; set; }
    public required bool IsSuspicious { get; set; }
    public string? LocationInfo { get; set; }

    public string[] Roles => [Admin, Write, UserSessionsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetUserSessions"];

    public class CreateUserSessionCommandHandler : IRequestHandler<CreateUserSessionCommand, CreatedUserSessionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserSessionService _userSessionService;
        private readonly UserSessionBusinessRules _userSessionBusinessRules;

        public CreateUserSessionCommandHandler(IMapper mapper, IUserSessionService userSessionService,
                                         UserSessionBusinessRules userSessionBusinessRules)
        {
            _mapper = mapper;
            _userSessionService = userSessionService;
            _userSessionBusinessRules = userSessionBusinessRules;
        }

        public async Task<CreatedUserSessionResponse> Handle(CreateUserSessionCommand request, CancellationToken cancellationToken)
        {
            UserSession userSession = _mapper.Map<UserSession>(request);

            await _userSessionService.AddAsync(userSession);

            CreatedUserSessionResponse response = _mapper.Map<CreatedUserSessionResponse>(userSession);
            return response;
        }
    }
}