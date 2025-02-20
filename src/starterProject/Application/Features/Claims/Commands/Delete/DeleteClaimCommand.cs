using Application.Features.Claims.Constants;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Claims.Constants.Claims;

namespace Application.Features.Claims.Commands.Delete;

public class DeleteClaimCommand : IRequest<DeletedClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => new[] { Admin, Write, Constants.Claims.Delete };

    public class DeleteClaimCommandHandler : IRequestHandler<DeleteClaimCommand, DeletedClaimResponse>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _claimBusinessRules;

        public DeleteClaimCommandHandler(
            IClaimRepository claimRepository,
            IMapper mapper,
            ClaimBusinessRules claimBusinessRules
        )
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
            _claimBusinessRules = claimBusinessRules;
        }

        public async Task<DeletedClaimResponse> Handle(
            DeleteClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            Claim? claim = await _claimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _claimBusinessRules.ClaimShouldExistWhenSelected(claim);

            await _claimRepository.DeleteAsync(entity: claim!);

            DeletedClaimResponse response = _mapper.Map<DeletedClaimResponse>(claim);
            return response;
        }
    }
}
