using Application.Features.Roles.Commands.Create;
using Application.Features.Roles.Commands.Delete;
using Application.Features.Roles.Commands.Update;
using Application.Features.Roles.Queries.GetById;
using Application.Features.Roles.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Roles.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateRoleCommand, Role>();
        CreateMap<Role, CreatedRoleResponse>();

        CreateMap<UpdateRoleCommand, Role>();
        CreateMap<Role, UpdatedRoleResponse>();

        CreateMap<DeleteRoleCommand, Role>();
        CreateMap<Role, DeletedRoleResponse>();

        CreateMap<Role, GetByIdRoleResponse>();

        CreateMap<Role, GetListRoleListItemDto>();
        CreateMap<IPaginate<Role>, GetListResponse<GetListRoleListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<Role, GetListRoleListItemDto>());
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