﻿using Application.Features.UserSessions.Rules;
using Application.Services.UserSessions;
using AutoMapper;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Commands.RevokeUserSession;
public class RevokeUserSessionCommand : IRequest<RevokeMySessionResponse>, ISecuredRequest
{
    public Guid SessionId { get; set; }
    public string? IpAddress { get; set; }

    public string[] Roles => [Admin, Write];

    public class RevokeUserSessionCommandHandler : IRequestHandler<RevokeUserSessionCommand, RevokeMySessionResponse>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly UserSessionBusinessRules _userSessionBusinessRules;
        private readonly IMapper _mapper;

        public RevokeUserSessionCommandHandler(
            IUserSessionService userSessionService,
            UserSessionBusinessRules userSessionBusinessRules,
            IMapper mapper)
        {
            _userSessionService = userSessionService;
            _userSessionBusinessRules = userSessionBusinessRules;
            _mapper = mapper;
        }

        public async Task<RevokeMySessionResponse> Handle(RevokeUserSessionCommand request, CancellationToken cancellationToken)
        {
            UserSession? userSession = await _userSessionService.GetAsync(
                predicate: us => us.Id == request.SessionId,
                cancellationToken: cancellationToken);

            await _userSessionBusinessRules.UserSessionShouldExistWhenSelected(userSession);

            userSession.IsRevoked = true;
            await _userSessionService.UpdateAsync(userSession);

            RevokeMySessionResponse response = _mapper.Map<RevokeMySessionResponse>(userSession);
            return response;
        }
    }
}
