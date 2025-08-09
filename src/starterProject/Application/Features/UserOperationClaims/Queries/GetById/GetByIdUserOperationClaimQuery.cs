using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;

namespace Application.Features.UserOperationClaims.Queries.GetById;

public class GetByIdUserOperationClaimQuery : IRequest<GetByIdUserOperationClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Constants.UserOperationClaims.Read];

    public class GetByIdUserClaimQueryHandler
        : IRequestHandler<GetByIdUserOperationClaimQuery, GetByIdUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userClaimBusinessRules;

        public GetByIdUserClaimQueryHandler(
            IUserOperationClaimRepository userClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<GetByIdUserOperationClaimResponse> Handle(
            GetByIdUserOperationClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            UserOperationClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: b => b.Id.Equals(request.Id),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserClaimShouldExistWhenSelected(userClaim);

            GetByIdUserOperationClaimResponse userClaimDto = _mapper.Map<GetByIdUserOperationClaimResponse>(
                userClaim
            );
            return userClaimDto;
        }
    }
}
