using Application.Features.UserSecurityClaims.Commands.Create;
using Application.Features.UserSecurityClaims.Commands.Delete;
using Application.Features.UserSecurityClaims.Commands.Update;
using Application.Features.UserSecurityClaims.Queries.GetById;
using Application.Features.UserSecurityClaims.Queries.GetList;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.UserSecurityClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<UserSecurityClaim, CreateUserSecurityClaimCommand>().ReverseMap();
        CreateMap<UserSecurityClaim, CreatedUserSecurityClaimResponse>().ReverseMap();
        CreateMap<UserSecurityClaim, UpdateUserSecurityClaimCommand>().ReverseMap();
        CreateMap<UserSecurityClaim, UpdatedUserSecurityClaimResponse>().ReverseMap();
        CreateMap<UserSecurityClaim, DeleteUserSecurityClaimCommand>().ReverseMap();
        CreateMap<UserSecurityClaim, DeletedUserSecurityClaimResponse>().ReverseMap();
        CreateMap<UserSecurityClaim, GetByIdUserSecurityClaimResponse>().ReverseMap();
        CreateMap<UserSecurityClaim, GetListUserSecurityClaimListItemDto>().ReverseMap();
        CreateMap<IPaginate<UserSecurityClaim>, GetListResponse<GetListUserSecurityClaimListItemDto>>().ReverseMap();
    }
}
