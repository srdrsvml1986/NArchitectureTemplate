using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using static Application.Features.UserOperationClaims.Constants.UserOperationClaims;

namespace Application.Features.UserOperationClaims.Commands.Delete;

public class DeleteUserOperationClaimCommand : IRequest<DeletedUserOperationClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.UserOperationClaims.Delete };

    public class DeleteUserClaimCommandHandler
        : IRequestHandler<DeleteUserOperationClaimCommand, DeletedUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userClaimRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userClaimBusinessRules;

        public DeleteUserClaimCommandHandler(
            IUserOperationClaimRepository userClaimRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userClaimBusinessRules
        )
        {
            _userClaimRepository = userClaimRepository;
            _mapper = mapper;
            _userClaimBusinessRules = userClaimBusinessRules;
        }

        public async Task<DeletedUserOperationClaimResponse> Handle(
            DeleteUserOperationClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            UserOperationClaim? userClaim = await _userClaimRepository.GetAsync(
                predicate: uoc => uoc.Id.Equals(request.Id),
                cancellationToken: cancellationToken
            );
            await _userClaimBusinessRules.UserClaimShouldExistWhenSelected(userClaim);

            await _userClaimRepository.DeleteAsync(userClaim!);

            DeletedUserOperationClaimResponse response = _mapper.Map<DeletedUserOperationClaimResponse>(userClaim);
            return response;
        }
    }
}
