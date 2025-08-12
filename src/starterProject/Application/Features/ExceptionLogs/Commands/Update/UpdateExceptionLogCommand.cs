using Application.Features.ExceptionLogs.Constants;
using Application.Features.ExceptionLogs.Rules;
using Application.Services.ExceptionLogs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.ExceptionLogs.Constants.ExceptionLogsOperationClaims;

namespace Application.Features.ExceptionLogs.Commands.Update;

public class UpdateExceptionLogCommand : IRequest<UpdatedExceptionLogResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, ExceptionLogsOperationClaims.Update];

    public class UpdateExceptionLogCommandHandler : IRequestHandler<UpdateExceptionLogCommand, UpdatedExceptionLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly IExceptionLogService _exceptionLogService;
        private readonly ExceptionLogBusinessRules _exceptionLogBusinessRules;

        public UpdateExceptionLogCommandHandler(IMapper mapper, IExceptionLogService exceptionLogService,
                                         ExceptionLogBusinessRules exceptionLogBusinessRules)
        {
            _mapper = mapper;
            _exceptionLogService = exceptionLogService;
            _exceptionLogBusinessRules = exceptionLogBusinessRules;
        }

        public async Task<UpdatedExceptionLogResponse> Handle(UpdateExceptionLogCommand request, CancellationToken cancellationToken)
        {
            ExceptionLog? exceptionLog = await _exceptionLogService.GetAsync(predicate: el => el.Id == request.Id, cancellationToken: cancellationToken);
            await _exceptionLogBusinessRules.ExceptionLogShouldExistWhenSelected(exceptionLog);
            exceptionLog = _mapper.Map(request, exceptionLog);

            await _exceptionLogService.UpdateAsync(exceptionLog!);

            UpdatedExceptionLogResponse response = _mapper.Map<UpdatedExceptionLogResponse>(exceptionLog);
            return response;
        }
    }
}