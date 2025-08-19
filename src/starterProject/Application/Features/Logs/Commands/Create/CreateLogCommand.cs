using Application.Features.Logs.Constants;
using Application.Features.Logs.Rules;
using Application.Services.Logs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Logs.Constants.LogsOperationClaims;

namespace Application.Features.Logs.Commands.Create;

public class CreateLogCommand : IRequest<CreatedLogResponse>, ISecuredRequest
{

    public string[] Roles => [Admin, Write, LogsOperationClaims.Create];

    public class CreateLogCommandHandler : IRequestHandler<CreateLogCommand, CreatedLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly LogBusinessRules _logBusinessRules;

        public CreateLogCommandHandler(IMapper mapper, ILogService logService,
                                         LogBusinessRules logBusinessRules)
        {
            _mapper = mapper;
            _logService = logService;
            _logBusinessRules = logBusinessRules;
        }

        public async Task<CreatedLogResponse> Handle(CreateLogCommand request, CancellationToken cancellationToken)
        {
            Log log = _mapper.Map<Log>(request);

            await _logService.AddAsync(log);

            CreatedLogResponse response = _mapper.Map<CreatedLogResponse>(log);
            return response;
        }
    }
}