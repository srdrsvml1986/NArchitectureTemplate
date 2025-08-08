using Application.Features.UserRoles.Constants;
using Application.Services.UserRoles;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.UserRoles.Constants.UserRolesOperationClaims;

namespace Application.Features.UserRoles.Queries.GetList;

public class GetListUserRoleQuery : IRequest<GetListResponse<GetListUserRoleListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListUserRoles({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetUserRoles";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListUserRoleQueryHandler : IRequestHandler<GetListUserRoleQuery, GetListResponse<GetListUserRoleListItemDto>>
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IMapper _mapper;

        public GetListUserRoleQueryHandler(IUserRoleService userRoleService, IMapper mapper)
        {
            _userRoleService = userRoleService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserRoleListItemDto>> Handle(GetListUserRoleQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserRole> userRoles = await _userRoleService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListUserRoleListItemDto> response;
            if (userRoles.Items == null || !userRoles.Items.Any())
            {
                response = new GetListResponse<GetListUserRoleListItemDto>
                {
                    Items = new List<GetListUserRoleListItemDto>(),
                    Index = userRoles.Index,
                    Size = userRoles.Size,
                    Count = userRoles.Count,
                    Pages = userRoles.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListUserRoleListItemDto>>(userRoles);
            }

            return response;
        }
    }
}