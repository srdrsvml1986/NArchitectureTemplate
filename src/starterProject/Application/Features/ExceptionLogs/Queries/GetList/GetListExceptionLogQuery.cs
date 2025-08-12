using Application.Features.ExceptionLogs.Constants;
using Application.Services.ExceptionLogs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using NArchitectureTemplate.Core.Persistence.Paging;
using MediatR;
using static Application.Features.ExceptionLogs.Constants.ExceptionLogsOperationClaims;

namespace Application.Features.ExceptionLogs.Queries.GetList;

public class GetListExceptionLogQuery : IRequest<GetListResponse<GetListExceptionLogListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetListExceptionLogQueryHandler : IRequestHandler<GetListExceptionLogQuery, GetListResponse<GetListExceptionLogListItemDto>>
    {
        private readonly IExceptionLogService _exceptionLogService;
        private readonly IMapper _mapper;

        public GetListExceptionLogQueryHandler(IExceptionLogService exceptionLogService, IMapper mapper)
        {
            _exceptionLogService = exceptionLogService;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListExceptionLogListItemDto>> Handle(GetListExceptionLogQuery request, CancellationToken cancellationToken)
        {
            IPaginate<ExceptionLog> exceptionLogs = await _exceptionLogService.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize, 
                cancellationToken: cancellationToken
            );

                        GetListResponse<GetListExceptionLogListItemDto> response;
            if (exceptionLogs.Items == null || !exceptionLogs.Items.Any())
            {
                response = new GetListResponse<GetListExceptionLogListItemDto>
                {
                    Items = new List<GetListExceptionLogListItemDto>(),
                    Index = exceptionLogs.Index,
                    Size = exceptionLogs.Size,
                    Count = exceptionLogs.Count,
                    Pages = exceptionLogs.Pages
                };
            }
            else
            {
                response = _mapper.Map<GetListResponse<GetListExceptionLogListItemDto>>(exceptionLogs);
            }

            return response;
        }
    }
}