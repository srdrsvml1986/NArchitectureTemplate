using Application.Features.UserClaims.Constants;
using Application.Features.UserClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Features.UserClaims.Queries.GetById;

public class GetByIdUserClaimQuery : IRequest<GetByIdUserClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [Constants.UserClaims.Read];

    public class GetByIdUserClaimQueryHandler
        : IRequestHandler<GetByIdUserClaimQuery, GetByIdUserClaimResponse>
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserClaimBusinessRules _userClaimBusinessRules;

        public GetByIdUserClaimQueryHandler(
            IUserClaimRepository userClaimRepository,
            IMapper mapper,
            UserClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<GetByIdUserClaimResponse> Handle(
            GetByIdUserClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            UserClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: b => b.Id.Equals(request.Id),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserClaimShouldExistWhenSelected(userClaim);

            GetByIdUserClaimResponse userClaimDto = _mapper.Map<GetByIdUserClaimResponse>(
                userClaim
            );
            return userClaimDto;
        }
    }
}
