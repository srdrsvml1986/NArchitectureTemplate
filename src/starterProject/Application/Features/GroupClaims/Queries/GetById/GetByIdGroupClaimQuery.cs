using Application.Features.GroupClaims.Constants;
using Application.Features.GroupClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.GroupClaims.Constants.GroupClaimsOperationClaims;

namespace Application.Features.GroupClaims.Queries.GetById;

public class GetByIdGroupClaimQuery : IRequest<GetByIdGroupClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdGroupClaimQueryHandler : IRequestHandler<GetByIdGroupClaimQuery, GetByIdGroupClaimResponse>
    {
        private readonly IMapper _mapper;
        private readonly IGroupClaimRepository _groupClaimRepository;
        private readonly GroupClaimBusinessRules _groupClaimBusinessRules;

        public GetByIdGroupClaimQueryHandler(IMapper mapper, IGroupClaimRepository groupClaimRepository, GroupClaimBusinessRules groupClaimBusinessRules)
        {
            _mapper = mapper;
            _groupClaimRepository = groupClaimRepository;
            _groupClaimBusinessRules = groupClaimBusinessRules;
        }

        public async Task<GetByIdGroupClaimResponse> Handle(GetByIdGroupClaimQuery request, CancellationToken cancellationToken)
        {
            GroupClaim? groupClaim = await _groupClaimRepository.GetAsync(predicate: gc => gc.Id == request.Id, cancellationToken: cancellationToken);
            await _groupClaimBusinessRules.GroupClaimShouldExistWhenSelected(groupClaim);

            GetByIdGroupClaimResponse response = _mapper.Map<GetByIdGroupClaimResponse>(groupClaim);
            return response;
        }
    }
}