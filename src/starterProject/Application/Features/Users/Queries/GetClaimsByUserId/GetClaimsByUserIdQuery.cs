using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.GetClaimsByUserId;

public class GetClaimsByUserIdQuery : IRequest<GetClaimsByUserIdResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [UsersOperationClaims.Read];

    public class GetClaimsByUserIdQueryHandler : IRequestHandler<GetClaimsByUserIdQuery, GetClaimsByUserIdResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IOperationClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public GetClaimsByUserIdQueryHandler(
            IUserRepository userRepository,
            IUserOperationClaimRepository userClaimRepository,
            IOperationClaimRepository claimRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules)
        {
            _userRepository = userRepository;
            _userClaimRepository = userClaimRepository;
            _claimRepository = claimRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
        }

        public async Task<GetClaimsByUserIdResponse> Handle(GetClaimsByUserIdQuery request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var userClaims = await _userClaimRepository.GetListAsync(
                predicate: uc => uc.UserId == request.UserId,
                cancellationToken: cancellationToken
            );

            var claimIds = userClaims.Items.Select(uc => uc.OperationClaimId).ToList();

            var claims = _claimRepository.Query()
                .Where(c => claimIds.Contains(c.Id));

            return new GetClaimsByUserIdResponse { Claims = claims };
        }
    }
}
