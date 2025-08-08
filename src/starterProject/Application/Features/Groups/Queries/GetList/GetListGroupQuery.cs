using Application.Features.Groups.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Groups.Constants.GroupsOperationClaims;

namespace Application.Features.Groups.Queries.GetList;

public class GetListGroupQuery : IRequest<GetListResponse<GetListGroupListItemDto>>, ISecuredRequest, ICachableRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListGroups({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetGroups";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListGroupQueryHandler : IRequestHandler<GetListGroupQuery, GetListResponse<GetListGroupListItemDto>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GetListGroupQueryHandler(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListGroupListItemDto>> Handle(GetListGroupQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Group> groups = await _groupRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListGroupListItemDto> response;
            if (groups.Items == null || !groups.Items.Any())
            {
                response = new GetListResponse<GetListGroupListItemDto>
                {
                    Items = new List<GetListGroupListItemDto>(),
                    Index = groups.Index,
                    Size = groups.Size,
                    Count = groups.Count,
                    Pages = groups.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListGroupListItemDto>>(groups);
            }

            return response;
        }
    }
}