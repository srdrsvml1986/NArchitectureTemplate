using Application.Features.Claims.Constants;
using Application.Features.Claims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using static Application.Features.Claims.Constants.Claims;

namespace Application.Features.Claims.Commands.Update;

public class UpdateClaimCommand : IRequest<UpdateClaimResponse>, ISecuredRequest
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

    public class UpdateClaimCommandHandler : IRequestHandler<UpdateClaimCommand, UpdateClaimResponse>
    {
        private readonly IClaimRepository _securityClaimRepository;
        private readonly IMapper _mapper;
        private readonly ClaimBusinessRules _securityClaimBusinessRules;

        public UpdateClaimCommandHandler(
            IClaimRepository securityClaimRepository,
            IMapper mapper,
            ClaimBusinessRules securityClaimBusinessRules
        )
        {
            _securityClaimRepository = securityClaimRepository;
            _mapper = mapper;
            _securityClaimBusinessRules = securityClaimBusinessRules;
        }

        public async Task<UpdateClaimResponse> Handle(
            UpdateClaimCommand request,
            CancellationToken cancellationToken
        )
        {
            Claim? claim = await _securityClaimRepository.GetAsync(
                predicate: oc => oc.Id == request.Id,
                cancellationToken: cancellationToken
            );
            await _securityClaimBusinessRules.ClaimShouldExistWhenSelected(claim);
            await _securityClaimBusinessRules.ClaimNameShouldNotExistWhenUpdating(request.Id, request.Name);
            Claim mappedClaim = _mapper.Map(request, destination: claim!);

            Claim updatedClaim = await _securityClaimRepository.UpdateAsync(mappedClaim);

            UpdateClaimResponse response = _mapper.Map<UpdateClaimResponse>(updatedClaim);
            return response;
        }
    }
}
