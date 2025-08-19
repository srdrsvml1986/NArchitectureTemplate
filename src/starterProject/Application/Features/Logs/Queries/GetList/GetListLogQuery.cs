using Application.Features.Logs.Constants;
using Application.Services.Logs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Logs.Constants.LogsOperationClaims;

namespace Application.Features.Logs.Queries.GetList;

public class GetListLogQuery : IRequest<GetListResponse<GetListLogListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListLogQueryHandler : IRequestHandler<GetListLogQuery, GetListResponse<GetListLogListItemDto>>
    {
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public GetListLogQueryHandler(ILogService logService, IMapper mapper)
        {
            _logService = logService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListLogListItemDto>> Handle(GetListLogQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Log> logs = await _logService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListLogListItemDto> response;
            if (logs.Items == null || !logs.Items.Any())
            {
                response = new GetListResponse<GetListLogListItemDto>
                {
                    Items = new List<GetListLogListItemDto>(),
                    Index = logs.Index,
                    Size = logs.Size,
                    Count = logs.Count,
                    Pages = logs.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListLogListItemDto>>(logs);
            }

            return response;
        }
    }
}