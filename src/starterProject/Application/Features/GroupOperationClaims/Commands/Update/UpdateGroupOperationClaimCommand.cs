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

namespace Application.Features.GroupOperationClaims.Commands.Update;

public class UpdateGroupOperationClaimCommand : IRequest<UpdatedGroupOperationClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int ClaimId { get; set; }
    public required int GroupId { get; set; }

    public string[] Roles => [Admin, Write, Constants.GroupOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupClaims"];

    public class UpdateGroupClaimCommandHandler : IRequestHandler<UpdateGroupOperationClaimCommand, UpdatedGroupOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly GroupOperationClaimBusinessRules _groupClaimBusinessRules;

        public UpdateGroupClaimCommandHandler(IMapper mapper, IGroupOperationClaimRepository groupClaimRepository,
                                         GroupOperationClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<UpdatedGroupOperationClaimResponse> Handle(UpdateGroupOperationClaimCommand request, CancellationToken cancellationToken)
        {
            GroupOperationClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate: gc => gc.Id == request.Id, cancellationToken: cancellationToken);
            await _groupClaimBusinessRules.GroupClaimShouldExistWhenSelected(groupClaim);
            groupClaim = _mapper.Map(request, groupClaim);

            await _groupClaimRepository.UpdateAsync(groupClaim!);

            UpdatedGroupOperationClaimResponse response = _mapper.Map<UpdatedGroupOperationClaimResponse>(groupClaim);
            return response;
        }
    }
}