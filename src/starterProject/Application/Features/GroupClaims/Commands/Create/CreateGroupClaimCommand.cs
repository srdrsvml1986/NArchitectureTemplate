using Application.Features.GroupClaims.Constants;
using Application.Features.GroupClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.GroupClaims.Constants.GroupClaimsOperationClaims;

namespace Application.Features.GroupClaims.Commands.Create;

public class CreateGroupClaimCommand : IRequest<CreatedGroupClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int ClaimId { get; set; }
    public required int GroupId { get; set; }

    public string[] Roles => [Admin, Write, GroupClaimsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupClaims"];

    public class CreateGroupClaimCommandHandler : IRequestHandler<CreateGroupClaimCommand, CreatedGroupClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly GroupClaimBusinessRules _groupClaimBusinessRules;

        public CreateGroupClaimCommandHandler(IMapper mapper, IGroupClaimRepository groupClaimRepository,
                                         GroupClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<CreatedGroupClaimResponse> Handle(CreateGroupClaimCommand request, CancellationToken cancellationToken)
        {
            GroupClaim groupClaim = _mapper.Map<GroupClaim>(request);

            await _groupClaimRepository.AddAsync(groupClaim);

            CreatedGroupClaimResponse response = _mapper.Map<CreatedGroupClaimResponse>(groupClaim);
            return response;
        }
    }
}