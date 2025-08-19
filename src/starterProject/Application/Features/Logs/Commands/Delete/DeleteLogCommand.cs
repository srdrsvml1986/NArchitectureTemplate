using Application.Features.Logs.Constants;
using Application.Features.Logs.Constants;
using Application.Features.Logs.Rules;
using Application.Services.Logs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Logs.Constants.LogsOperationClaims;

namespace Application.Features.Logs.Commands.Delete;

public class DeleteLogCommand : IRequest<DeletedLogResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, LogsOperationClaims.Delete];

    public class DeleteLogCommandHandler : IRequestHandler<DeleteLogCommand, DeletedLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly LogBusinessRules _logBusinessRules;

        public DeleteLogCommandHandler(IMapper mapper, ILogService logService,
                                         LogBusinessRules logBusinessRules)
        {
            _mapper = mapper;
            _logService = logService;
            _logBusinessRules = logBusinessRules;
        }

        public async Task<DeletedLogResponse> Handle(DeleteLogCommand request, CancellationToken cancellationToken)
        {
            Log? log = await _logService.GetAsync(predicate: l => l.Id == request.Id, cancellationToken: cancellationToken);
            await _logBusinessRules.LogShouldExistWhenSelected(log);

            await _logService.DeleteAsync(log!);

            DeletedLogResponse response = _mapper.Map<DeletedLogResponse>(log);
            return response;
        }
    }
}