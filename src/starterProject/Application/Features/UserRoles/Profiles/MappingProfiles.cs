using Application.Features.UserRoles.Commands.Create;
using Application.Features.UserRoles.Commands.Delete;
using Application.Features.UserRoles.Commands.Update;
using Application.Features.UserRoles.Queries.GetById;
using Application.Features.UserRoles.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.UserRoles.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateUserRoleCommand, UserRole>();
        CreateMap<UserRole, CreatedUserRoleResponse>();

        CreateMap<UpdateUserRoleCommand, UserRole>();
        CreateMap<UserRole, UpdatedUserRoleResponse>();

        CreateMap<DeleteUserRoleCommand, UserRole>();
        CreateMap<UserRole, DeletedUserRoleResponse>();

        CreateMap<UserRole, GetByIdUserRoleResponse>();

        CreateMap<UserRole, GetListUserRoleListItemDto>();
        CreateMap<IPaginate<UserRole>, GetListResponse<GetListUserRoleListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<UserRole, GetListUserRoleListItemDto>());
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