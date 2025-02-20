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

namespace Application.Features.GroupClaims.Commands.Update;

public class UpdateGroupClaimCommand : IRequest<UpdatedGroupClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int ClaimId { get; set; }
    public required int GroupId { get; set; }

    public string[] Roles => [Admin, Write, GroupClaimsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupClaims"];

    public class UpdateGroupClaimCommandHandler : IRequestHandler<UpdateGroupClaimCommand, UpdatedGroupClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly GroupClaimBusinessRules _groupClaimBusinessRules;

        public UpdateGroupClaimCommandHandler(IMapper mapper, IGroupClaimRepository groupClaimRepository,
                                         GroupClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<UpdatedGroupClaimResponse> Handle(UpdateGroupClaimCommand request, CancellationToken cancellationToken)
        {
            GroupClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate: gc => gc.Id == request.Id, cancellationToken: cancellationToken);
            await _groupClaimBusinessRules.GroupClaimShouldExistWhenSelected(groupClaim);
            groupClaim = _mapper.Map(request, groupClaim);

            await _groupClaimRepository.UpdateAsync(groupClaim!);

            UpdatedGroupClaimResponse response = _mapper.Map<UpdatedGroupClaimResponse>(groupClaim);
            return response;
        }
    }
}