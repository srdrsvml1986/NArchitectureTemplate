using Application.Features.RoleClaims.Commands.Create;
using Application.Features.RoleClaims.Commands.Delete;
using Application.Features.RoleClaims.Commands.Update;
using Application.Features.RoleClaims.Queries.GetById;
using Application.Features.RoleClaims.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.RoleClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateRoleClaimCommand, RoleClaim>();
        CreateMap<RoleClaim, CreatedRoleClaimResponse>();

        CreateMap<UpdateRoleClaimCommand, RoleClaim>();
        CreateMap<RoleClaim, UpdatedRoleClaimResponse>();

        CreateMap<DeleteRoleClaimCommand, RoleClaim>();
        CreateMap<RoleClaim, DeletedRoleClaimResponse>();

        CreateMap<RoleClaim, GetByIdRoleClaimResponse>();

        CreateMap<RoleClaim, GetListRoleClaimListItemDto>();
        CreateMap<IPaginate<RoleClaim>, GetListResponse<GetListRoleClaimListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<RoleClaim, GetListRoleClaimListItemDto>());
    }
}
public class PaginateToGetListResponseConverter<TSource, TDestination>
    : ITypeConverter<IPaginate<TSource>, GetListResponse<TDestination>>
{
    public GetListResponse<TDestination> Convert(
        IPaginate<TSource> source,
        GetListResponse<TDestination> destination,
        ResolutionContext context)
    {
        return new GetListResponse<TDestination>
        {
            Items = context.Mapper.Map<List<TDestination>>(source.Items ?? new List<TSource>()),
            Index = source.Index,
            Size = source.Size,
            Count = source.Count,
            Pages = source.Pages
        };
    }
}