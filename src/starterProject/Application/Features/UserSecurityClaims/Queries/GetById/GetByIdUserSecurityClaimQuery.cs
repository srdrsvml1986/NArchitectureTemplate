using Application.Features.UserSecurityClaims.Constants;
using Application.Features.UserSecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Features.UserSecurityClaims.Queries.GetById;

public class GetByIdUserSecurityClaimQuery : IRequest<GetByIdUserSecurityClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Constants.UserSecurityClaims.Read];

    public class GetByIdUserClaimQueryHandler
        : IRequestHandler<GetByIdUserSecurityClaimQuery, GetByIdUserSecurityClaimResponse>
    {
        private readonly IUserSecurityClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserSecurityClaimBusinessRules _userClaimBusinessRules;

        public GetByIdUserClaimQueryHandler(
            IUserSecurityClaimRepository userClaimRepository,
            IMapper mapper,
            UserSecurityClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<GetByIdUserSecurityClaimResponse> Handle(
            GetByIdUserSecurityClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            UserSecurityClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: b => b.Id.Equals(request.Id),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserSecurityClaimShouldExistWhenSelected(userClaim);

            GetByIdUserSecurityClaimResponse userClaimDto = _mapper.Map<GetByIdUserSecurityClaimResponse>(
                userClaim
            );
            return userClaimDto;
        }
    }
}
