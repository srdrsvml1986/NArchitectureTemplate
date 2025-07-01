using Application.Features.UserClaims.Constants;
using Application.Features.UserClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.UserClaims.Constants.UserClaims;

namespace Application.Features.UserClaims.Commands.Delete;

public class DeleteUserClaimCommand : IRequest<DeletedUserClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserClaims.Delete };

    public class DeleteUserClaimCommandHandler
        : IRequestHandler<DeleteUserClaimCommand, DeletedUserClaimResponse>
    {
        private readonly IUserClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserClaimBusinessRules _userClaimBusinessRules;

        public DeleteUserClaimCommandHandler(
            IUserClaimRepository userClaimRepository,
            IMapper mapper,
            UserClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<DeletedUserClaimResponse> Handle(
            DeleteUserClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            UserClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserClaimShouldExistWhenSelected(userClaim);

            await _userClaimRepository.DeleteAsync(userClaim!);

            DeletedUserClaimResponse response = _mapper.Map<DeletedUserClaimResponse>(userClaim);
            return response;
        }
    }
}
