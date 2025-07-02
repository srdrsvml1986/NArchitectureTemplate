using Application.Features.GroupOperationClaims.Constants;
using Application.Features.GroupOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.GroupOperationClaims.Constants.GroupOperationClaims;

namespace Application.Features.GroupOperationClaims.Commands.Create;

public class CreateGroupOperationClaimCommand : IRequest<CreatedGroupOperationClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int ClaimId { get; set; }
    public required int GroupId { get; set; }

    public string[] Roles => [Admin, Write, Constants.GroupOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupClaims"];

    public class CreateGroupClaimCommandHandler : IRequestHandler<CreateGroupOperationClaimCommand, CreatedGroupOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly GroupOperationClaimBusinessRules _groupClaimBusinessRules;

        public CreateGroupClaimCommandHandler(IMapper mapper, IGroupOperationClaimRepository groupClaimRepository,
                                         GroupOperationClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<CreatedGroupOperationClaimResponse> Handle(CreateGroupOperationClaimCommand request, CancellationToken cancellationToken)
        {
            GroupOperationClaim groupClaim = _mapper.Map<GroupOperationClaim>(request);

            await _groupClaimRepository.AddAsync(groupClaim);

            CreatedGroupOperationClaimResponse response = _mapper.Map<CreatedGroupOperationClaimResponse>(groupClaim);
            return response;
        }
    }
}