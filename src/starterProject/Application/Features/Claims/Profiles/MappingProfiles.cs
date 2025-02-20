using Application.Features.Claims.Commands.Create;
using Application.Features.Claims.Commands.Delete;
using Application.Features.Claims.Commands.Update;
using Application.Features.Claims.Queries.GetById;
using Application.Features.Claims.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Claims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Claim, CreateClaimCommand>().ReverseMap();
        CreateMap<Claim, CreatedClaimResponse>().ReverseMap();
        CreateMap<Claim, UpdateClaimCommand>().ReverseMap();
        CreateMap<Claim, UpdatedClaimResponse>().ReverseMap();
        CreateMap<Claim, DeleteClaimCommand>().ReverseMap();
        CreateMap<Claim, DeletedClaimResponse>().ReverseMap();
        CreateMap<Claim, GetByIdClaimResponse>().ReverseMap();
        CreateMap<Claim, GetListClaimListItemDto>().ReverseMap();
        CreateMap<IPaginate<Claim>, GetListResponse<GetListClaimListItemDto>>().ReverseMap();
    }
}
