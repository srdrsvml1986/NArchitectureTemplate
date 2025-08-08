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

namespace Application.Features.GroupOperationClaims.Commands.Delete;

public class DeleteGroupOperationClaimCommand : IRequest<DeletedGroupOperationClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, Constants.GroupOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupClaims"];

    public class DeleteGroupClaimCommandHandler : IRequestHandler<DeleteGroupOperationClaimCommand, DeletedGroupOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly GroupOperationClaimBusinessRules _groupClaimBusinessRules;

        public DeleteGroupClaimCommandHandler(IMapper mapper, IGroupOperationClaimRepository groupClaimRepository,
                                         GroupOperationClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<DeletedGroupOperationClaimResponse> Handle(DeleteGroupOperationClaimCommand request, CancellationToken cancellationToken)
        {
            GroupOperationClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate: gc => gc.Id == request.Id, cancellationToken: cancellationToken);
            await _groupClaimBusinessRules.GroupClaimShouldExistWhenSelected(groupClaim);

            await _groupClaimRepository.DeleteAsync(groupClaim!);

            DeletedGroupOperationClaimResponse response = _mapper.Map<DeletedGroupOperationClaimResponse>(groupClaim);
            return response;
        }
    }
}