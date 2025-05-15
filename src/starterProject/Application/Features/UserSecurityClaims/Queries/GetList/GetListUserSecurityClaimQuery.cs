using Application.Features.UserSecurityClaims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.UserSecurityClaims.Queries.GetList;

public class GetListUserSecurityClaimQuery : IRequest<GetListResponse<GetListUserSecurityClaimListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Constants.UserSecurityClaims.Read];

    public GetListUserSecurityClaimQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListUserSecurityClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListUserClaimQueryHandler
        : IRequestHandler<GetListUserSecurityClaimQuery, GetListResponse<GetListUserSecurityClaimListItemDto>>
    {
        private readonly IUserSecurityClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;

        public GetListUserClaimQueryHandler(IUserSecurityClaimRepository userClaimRepository, IMapper mapper)
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserSecurityClaimListItemDto>> Handle(
            GetListUserSecurityClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<UserSecurityClaim> userClaims = await _userClaimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );

            GetListResponse<GetListUserSecurityClaimListItemDto> mappedUserClaimListModel = _mapper.Map<
                GetListResponse<GetListUserSecurityClaimListItemDto>
            >(userClaims);
            return mappedUserClaimListModel;
        }
    }
}
