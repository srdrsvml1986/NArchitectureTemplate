using Application.Features.UserSessions.Queries.GetList;
using Application.Services.UserSessions;
using AutoMapper;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Features.UserSessions.Constants.UserSessionsOperationClaims;

namespace Application.Features.UserSessions.Queries.GetUserSessions;
public class GetUserSessionsQuery : IRequest<GetListResponse<GetListUserSessionListItemDto>>, ISecuredRequest
{
    public Guid UserId { get; set; }
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, GetListResponse<GetListUserSessionListItemDto>>
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IMapper _mapper;

        public GetUserSessionsQueryHandler(IUserSessionService userSessionService, IMapper mapper)
        {
            _userSessionService = userSessionService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserSessionListItemDto>> Handle(GetUserSessionsQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserSession>? userSessions = await _userSessionService.GetListAsync(
                predicate: us => us.UserId == request.UserId,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
            );
            if (userSessions == null)
            {
                return new GetListResponse<GetListUserSessionListItemDto>
                {
                    Items = new List<GetListUserSessionListItemDto>()                    
                };
            }
            GetListResponse<GetListUserSessionListItemDto> response = _mapper.Map<GetListResponse<GetListUserSessionListItemDto>>(userSessions);
            return response;
        }
    }
}
