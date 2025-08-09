using Application.Features.GroupRoles.Commands.Create;
using Application.Features.GroupRoles.Commands.Delete;
using Application.Features.GroupRoles.Commands.Update;
using Application.Features.GroupRoles.Queries.GetById;
using Application.Features.GroupRoles.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.GroupRoles.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateGroupRoleCommand, GroupRole>();
        CreateMap<GroupRole, CreatedGroupRoleResponse>();

        CreateMap<UpdateGroupRoleCommand, GroupRole>();
        CreateMap<GroupRole, UpdatedGroupRoleResponse>();

        CreateMap<DeleteGroupRoleCommand, GroupRole>();
        CreateMap<GroupRole, DeletedGroupRoleResponse>();

        CreateMap<GroupRole, GetByIdGroupRoleResponse>();

        CreateMap<GroupRole, GetListGroupRoleListItemDto>();
        CreateMap<IPaginate<GroupRole>, GetListResponse<GetListGroupRoleListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<GroupRole, GetListGroupRoleListItemDto>());
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