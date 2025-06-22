using Application.Features.UserClaims.Commands.Create;
using Application.Features.UserClaims.Commands.Delete;
using Application.Features.UserClaims.Commands.Update;
using Application.Features.UserClaims.Queries.GetById;
using Application.Features.UserClaims.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.UserClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<UserClaim, CreateUserClaimCommand>().ReverseMap();
        CreateMap<UserClaim, CreatedUserClaimResponse>().ReverseMap();
        CreateMap<UserClaim, UpdateUserClaimCommand>().ReverseMap();
        CreateMap<UserClaim, UpdatedUserClaimResponse>().ReverseMap();
        CreateMap<UserClaim, DeleteUserClaimCommand>().ReverseMap();
        CreateMap<UserClaim, DeletedUserClaimResponse>().ReverseMap();
        CreateMap<UserClaim, GetByIdUserClaimResponse>().ReverseMap();
        CreateMap<UserClaim, GetListUserClaimListItemDto>().ReverseMap();
        CreateMap<IPaginate<UserClaim>, GetListResponse<GetListUserClaimListItemDto>>().ReverseMap();
    }
}
