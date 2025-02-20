using Application.Features.Claims.Constants;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Claims.Constants.Claims;

namespace Application.Features.Claims.Commands.Update;

public class UpdateClaimCommand : IRequest<UpdatedClaimResponse>, ISecuredRequest
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateClaimCommand()
    {
        Name = string.Empty;
    }

    public UpdateClaimCommand(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public string[] Roles => new[] { Admin, Write, Constants.Claims.Update };

    public class UpdateClaimCommandHandler : IRequestHandler<UpdateClaimCommand, UpdatedClaimResponse>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _claimBusinessRules;

        public UpdateClaimCommandHandler(
            IClaimRepository claimRepository,
            IMapper mapper,
            ClaimBusinessRules claimBusinessRules
        )
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
            _claimBusinessRules = claimBusinessRules;
        }

        public async Task<UpdatedClaimResponse> Handle(
            UpdateClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            Claim? claim = await _claimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _claimBusinessRules.ClaimShouldExistWhenSelected(claim);
            await _claimBusinessRules.ClaimNameShouldNotExistWhenUpdating(request.Id, request.Name);
            Claim mappedClaim = _mapper.Map(request, destination: claim!);

            Claim updatedClaim = await _claimRepository.UpdateAsync(mappedClaim);

            UpdatedClaimResponse response = _mapper.Map<UpdatedClaimResponse>(updatedClaim);
            return response;
        }
    }
}
