using Application.Features.ExceptionLogs.Constants;
using Application.Features.ExceptionLogs.Rules;
using Application.Services.ExceptionLogs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.ExceptionLogs.Constants.ExceptionLogsOperationClaims;

namespace Application.Features.ExceptionLogs.Commands.Create;

public class CreateExceptionLogCommand : IRequest<CreatedExceptionLogResponse>, ISecuredRequest
{

    public string[] Roles => [Admin, Write, ExceptionLogsOperationClaims.Create];

    public class CreateExceptionLogCommandHandler : IRequestHandler<CreateExceptionLogCommand, CreatedExceptionLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly IExceptionLogService _exceptionLogService;
        private readonly ExceptionLogBusinessRules _exceptionLogBusinessRules;

        public CreateExceptionLogCommandHandler(IMapper mapper, IExceptionLogService exceptionLogService,
                                         ExceptionLogBusinessRules exceptionLogBusinessRules)
        {
            _mapper = mapper;
            _exceptionLogService = exceptionLogService;
            _exceptionLogBusinessRules = exceptionLogBusinessRules;
        }

        public async Task<CreatedExceptionLogResponse> Handle(CreateExceptionLogCommand request, CancellationToken cancellationToken)
        {
            ExceptionLog exceptionLog = _mapper.Map<ExceptionLog>(request);

            await _exceptionLogService.AddAsync(exceptionLog);

            CreatedExceptionLogResponse response = _mapper.Map<CreatedExceptionLogResponse>(exceptionLog);
            return response;
        }
    }
}