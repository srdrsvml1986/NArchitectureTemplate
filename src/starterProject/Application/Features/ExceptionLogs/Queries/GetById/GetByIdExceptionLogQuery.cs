using Application.Features.ExceptionLogs.Constants;
using Application.Features.ExceptionLogs.Rules;
using Application.Services.ExceptionLogs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.ExceptionLogs.Constants.ExceptionLogsOperationClaims;

namespace Application.Features.ExceptionLogs.Queries.GetById;

public class GetByIdExceptionLogQuery : IRequest<GetByIdExceptionLogResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdExceptionLogQueryHandler : IRequestHandler<GetByIdExceptionLogQuery, GetByIdExceptionLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly IExceptionLogService _exceptionLogService;
        private readonly ExceptionLogBusinessRules _exceptionLogBusinessRules;

        public GetByIdExceptionLogQueryHandler(IMapper mapper, IExceptionLogService exceptionLogService, ExceptionLogBusinessRules exceptionLogBusinessRules)
        {
            _mapper = mapper;
            _exceptionLogService = exceptionLogService;
            _exceptionLogBusinessRules = exceptionLogBusinessRules;
        }

        public async Task<GetByIdExceptionLogResponse> Handle(GetByIdExceptionLogQuery request, CancellationToken cancellationToken)
        {
            ExceptionLog? exceptionLog = await _exceptionLogService.GetAsync(predicate: el => el.Id == request.Id, cancellationToken: cancellationToken);
            await _exceptionLogBusinessRules.ExceptionLogShouldExistWhenSelected(exceptionLog);

            GetByIdExceptionLogResponse response = _mapper.Map<GetByIdExceptionLogResponse>(exceptionLog);
            return response;
        }
    }
}