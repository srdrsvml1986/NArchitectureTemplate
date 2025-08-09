using Application.Features.GroupOperationClaims.Constants;
using Application.Features.GroupOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.GroupOperationClaims.Constants.GroupOperationClaims;

namespace Application.Features.GroupOperationClaims.Queries.GetById;

public class GetByIdGroupOperationClaimQuery : IRequest<GetByIdGroupOperationClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdGroupClaimQueryHandler : IRequestHandler<GetByIdGroupOperationClaimQuery, GetByIdGroupOperationClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupOperationClaimRepository _groupClaimRepository;
        private readonly GroupOperationClaimBusinessRules _groupClaimBusinessRules;

        public GetByIdGroupClaimQueryHandler(IMapper mapper, IGroupOperationClaimRepository groupClaimRepository, GroupOperationClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<GetByIdGroupOperationClaimResponse> Handle(GetByIdGroupOperationClaimQuery request, CancellationToken cancellationToken)
        {
            GroupOperationClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate: gc => gc.Id == request.Id, cancellationToken: cancellationToken);
            await _groupClaimBusinessRules.GroupClaimShouldExistWhenSelected(groupClaim);

            GetByIdGroupOperationClaimResponse response = _mapper.Map<GetByIdGroupOperationClaimResponse>(groupClaim);
            return response;
        }
    }
}