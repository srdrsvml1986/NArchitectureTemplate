using Application.Features.Logs.Constants;
using Application.Features.Logs.Rules;
using Application.Services.Logs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Logs.Constants.LogsOperationClaims;

namespace Application.Features.Logs.Commands.Update;

public class UpdateLogCommand : IRequest<UpdatedLogResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Write, LogsOperationClaims.Update];

    public class UpdateLogCommandHandler : IRequestHandler<UpdateLogCommand, UpdatedLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly LogBusinessRules _logBusinessRules;

        public UpdateLogCommandHandler(IMapper mapper, ILogService logService,
                                         LogBusinessRules logBusinessRules)
        {
            _mapper = mapper;
            _logService = logService;
            _logBusinessRules = logBusinessRules;
        }

        public async Task<UpdatedLogResponse> Handle(UpdateLogCommand request, CancellationToken cancellationToken)
        {
            Log? log = await _logService.GetAsync(predicate: l => l.Id == request.Id, cancellationToken: cancellationToken);
            await _logBusinessRules.LogShouldExistWhenSelected(log);
            log = _mapper.Map(request, log);

            await _logService.UpdateAsync(log!);

            UpdatedLogResponse response = _mapper.Map<UpdatedLogResponse>(log);
            return response;
        }
    }
}