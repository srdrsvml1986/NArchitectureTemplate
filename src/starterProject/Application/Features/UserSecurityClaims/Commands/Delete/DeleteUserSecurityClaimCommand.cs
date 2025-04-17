using Application.Features.UserSecurityClaims.Constants;
using Application.Features.UserSecurityClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserSecurityClaims.Constants.UserSecurityClaims;

namespace Application.Features.UserSecurityClaims.Commands.Delete;

public class DeleteUserSecurityClaimCommand : IRequest<DeletedUserSecurityClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserSecurityClaims.Delete };

    public class DeleteUserClaimCommandHandler
        : IRequestHandler<DeleteUserSecurityClaimCommand, DeletedUserSecurityClaimResponse>
    {
        private readonly IUserSecurityClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserSecurityClaimBusinessRules _userClaimBusinessRules;

        public DeleteUserClaimCommandHandler(
            IUserSecurityClaimRepository userClaimRepository,
            IMapper mapper,
            UserSecurityClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<DeletedUserSecurityClaimResponse> Handle(
            DeleteUserSecurityClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            UserSecurityClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserSecurityClaimShouldExistWhenSelected(userClaim);

            await _userClaimRepository.DeleteAsync(userClaim!);

            DeletedUserSecurityClaimResponse response = _mapper.Map<DeletedUserSecurityClaimResponse>(userClaim);
            return response;
        }
    }
}
