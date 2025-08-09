using Application.Features.UserSessions.Commands.Create;
using Application.Features.UserSessions.Commands.Delete;
using Application.Features.UserSessions.Commands.Update;
using Application.Features.UserSessions.Queries.GetById;
using Application.Features.UserSessions.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.UserSessions.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateUserSessionCommand, UserSession>();
        CreateMap<UserSession, CreatedUserSessionResponse>();

        CreateMap<UpdateUserSessionCommand, UserSession>();
        CreateMap<UserSession, UpdatedUserSessionResponse>();

        CreateMap<DeleteUserSessionCommand, UserSession>();
        CreateMap<UserSession, DeletedUserSessionResponse>();

        CreateMap<UserSession, GetByIdUserSessionResponse>();

        CreateMap<UserSession, GetListUserSessionListItemDto>();
        CreateMap<IPaginate<UserSession>, GetListResponse<GetListUserSessionListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<UserSession, GetListUserSessionListItemDto>());
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