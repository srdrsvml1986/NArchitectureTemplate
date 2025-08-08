using Application.Features.UserGroups.Commands.Create;
using Application.Features.UserGroups.Commands.Delete;
using Application.Features.UserGroups.Commands.Update;
using Application.Features.UserGroups.Queries.GetById;
using Application.Features.UserGroups.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.UserGroups.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateUserGroupCommand, UserGroup>();
        CreateMap<UserGroup, CreatedUserGroupResponse>();

        CreateMap<UpdateUserGroupCommand, UserGroup>();
        CreateMap<UserGroup, UpdatedUserGroupResponse>();

        CreateMap<DeleteUserGroupCommand, UserGroup>();
        CreateMap<UserGroup, DeletedUserGroupResponse>();

        CreateMap<UserGroup, GetByIdUserGroupResponse>();

        CreateMap<UserGroup, GetListUserGroupListItemDto>();
        CreateMap<IPaginate<UserGroup>, GetListResponse<GetListUserGroupListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<UserGroup, GetListUserGroupListItemDto>());
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