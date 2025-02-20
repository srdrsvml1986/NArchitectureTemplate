using Application.Features.GroupClaims.Constants;
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

namespace Application.Features.GroupClaims.Commands.Delete;

public class DeleteGroupClaimCommand : IRequest<DeletedGroupClaimResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, GroupClaimsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetGroupClaims"];

    public class DeleteGroupClaimCommandHandler : IRequestHandler<DeleteGroupClaimCommand, DeletedGroupClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly GroupClaimBusinessRules _groupClaimBusinessRules;

        public DeleteGroupClaimCommandHandler(IMapper mapper, IGroupClaimRepository groupClaimRepository,
                                         GroupClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<DeletedGroupClaimResponse> Handle(DeleteGroupClaimCommand request, CancellationToken cancellationToken)
        {
            GroupClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate: gc => gc.Id == request.Id, cancellationToken: cancellationToken);
            await _groupClaimBusinessRules.GroupClaimShouldExistWhenSelected(groupClaim);

            await _groupClaimRepository.DeleteAsync(groupClaim!);

            DeletedGroupClaimResponse response = _mapper.Map<DeletedGroupClaimResponse>(groupClaim);
            return response;
        }
    }
}