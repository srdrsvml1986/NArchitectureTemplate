using Application.Features.UserClaims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.UserClaims.Queries.GetList;

public class GetListUserClaimQuery : IRequest<GetListResponse<GetListUserClaimListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Constants.UserClaims.Read];

    public GetListUserClaimQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListUserClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListUserClaimQueryHandler
        : IRequestHandler<GetListUserClaimQuery, GetListResponse<GetListUserClaimListItemDto>>
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;

        public GetListUserClaimQueryHandler(IUserClaimRepository userClaimRepository, IMapper mapper)
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserClaimListItemDto>> Handle(
            GetListUserClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            IPaginate<UserClaim> userClaims = await _userClaimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                enableTracking: false
            );

            GetListResponse<GetListUserClaimListItemDto> mappedUserClaimListModel = _mapper.Map<
                GetListResponse<GetListUserClaimListItemDto>
            >(userClaims);
            return mappedUserClaimListModel;
        }
    }
}
