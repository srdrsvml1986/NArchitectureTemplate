using Application.Features.UserSessions.Constants;
using Application.Features.UserSessions.Rules;
using Application.Services.UserSessions;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Queries.GetById;

public class GetByIdUserSessionQuery : IRequest<GetByIdUserSessionResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdUserSessionQueryHandler : IRequestHandler<GetByIdUserSessionQuery, GetByIdUserSessionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUserSessionService _userSessionService;
        private readonly UserSessionBusinessRules _userSessionBusinessRules;

        public GetByIdUserSessionQueryHandler(IMapper mapper, IUserSessionService userSessionService, UserSessionBusinessRules userSessionBusinessRules)
        {
            _mapper = mapper;
            _userSessionService = userSessionService;
            _userSessionBusinessRules = userSessionBusinessRules;
        }

        public async Task<GetByIdUserSessionResponse> Handle(GetByIdUserSessionQuery request, CancellationToken cancellationToken)
        {
            UserSession? userSession = await _userSessionService.GetAsync(predicate: us => us.Id == request.Id, cancellationToken: cancellationToken);
            await _userSessionBusinessRules.UserSessionShouldExistWhenSelected(userSession);

            GetByIdUserSessionResponse response = _mapper.Map<GetByIdUserSessionResponse>(userSession);
            return response;
        }
    }
}