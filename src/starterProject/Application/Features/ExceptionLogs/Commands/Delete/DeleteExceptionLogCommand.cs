using Application.Features.ExceptionLogs.Constants;
using Application.Features.ExceptionLogs.Constants;
using Application.Features.ExceptionLogs.Rules;
using Application.Services.ExceptionLogs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.ExceptionLogs.Constants.ExceptionLogsOperationClaims;

namespace Application.Features.ExceptionLogs.Commands.Delete;

public class DeleteExceptionLogCommand : IRequest<DeletedExceptionLogResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, ExceptionLogsOperationClaims.Delete];

    public class DeleteExceptionLogCommandHandler : IRequestHandler<DeleteExceptionLogCommand, DeletedExceptionLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly IExceptionLogService _exceptionLogService;
        private readonly ExceptionLogBusinessRules _exceptionLogBusinessRules;

        public DeleteExceptionLogCommandHandler(IMapper mapper, IExceptionLogService exceptionLogService,
                                         ExceptionLogBusinessRules exceptionLogBusinessRules)
        {
            _mapper = mapper;
            _exceptionLogService = exceptionLogService;
            _exceptionLogBusinessRules = exceptionLogBusinessRules;
        }

        public async Task<DeletedExceptionLogResponse> Handle(DeleteExceptionLogCommand request, CancellationToken cancellationToken)
        {
            ExceptionLog? exceptionLog = await _exceptionLogService.GetAsync(predicate: el => el.Id == request.Id, cancellationToken: cancellationToken);
            await _exceptionLogBusinessRules.ExceptionLogShouldExistWhenSelected(exceptionLog);

            await _exceptionLogService.DeleteAsync(exceptionLog!);

            DeletedExceptionLogResponse response = _mapper.Map<DeletedExceptionLogResponse>(exceptionLog);
            return response;
        }
    }
}