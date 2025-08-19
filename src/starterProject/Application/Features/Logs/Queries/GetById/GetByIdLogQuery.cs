using Application.Features.Logs.Constants;
using Application.Features.Logs.Rules;
using Application.Services.Logs;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Logs.Constants.LogsOperationClaims;

namespace Application.Features.Logs.Queries.GetById;

public class GetByIdLogQuery : IRequest<GetByIdLogResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdLogQueryHandler : IRequestHandler<GetByIdLogQuery, GetByIdLogResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly LogBusinessRules _logBusinessRules;

        public GetByIdLogQueryHandler(IMapper mapper, ILogService logService, LogBusinessRules logBusinessRules)
        {
            _mapper = mapper;
            _logService = logService;
            _logBusinessRules = logBusinessRules;
        }

        public async Task<GetByIdLogResponse> Handle(GetByIdLogQuery request, CancellationToken cancellationToken)
        {
            Log? log = await _logService.GetAsync(predicate: l => l.Id == request.Id, cancellationToken: cancellationToken);
            await _logBusinessRules.LogShouldExistWhenSelected(log);

            GetByIdLogResponse response = _mapper.Map<GetByIdLogResponse>(log);
            return response;
        }
    }
}