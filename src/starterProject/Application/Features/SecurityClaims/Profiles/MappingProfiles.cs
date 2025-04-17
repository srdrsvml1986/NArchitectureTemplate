using Application.Features.SecurityClaims.Commands.Create;
using Application.Features.SecurityClaims.Commands.Delete;
using Application.Features.SecurityClaims.Commands.Update;
using Application.Features.SecurityClaims.Queries.GetById;
using Application.Features.SecurityClaims.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.SecurityClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<SecurityClaim, CreateSecurityClaimCommand>().ReverseMap();
        CreateMap<SecurityClaim, CreatedSecurityClaimResponse>().ReverseMap();
        CreateMap<SecurityClaim, UpdateSecurityClaimCommand>().ReverseMap();
        CreateMap<SecurityClaim, UpdateSecurityClaimResponse>().ReverseMap();
        CreateMap<SecurityClaim, DeleteSecurityClaimCommand>().ReverseMap();
        CreateMap<SecurityClaim, DeleteSecurityClaimResponse>().ReverseMap();
        CreateMap<SecurityClaim, GetByIdSecurityClaimResponse>().ReverseMap();
        CreateMap<SecurityClaim, GetListSecurityClaimListItemDto>().ReverseMap();
        CreateMap<IPaginate<SecurityClaim>, GetListResponse<GetListSecurityClaimListItemDto>>().ReverseMap();
    }
}
