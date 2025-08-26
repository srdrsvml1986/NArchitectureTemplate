using Application.Features.UserNotificationSettings.Commands.Create;
using Application.Features.UserNotificationSettings.Commands.Delete;
using Application.Features.UserNotificationSettings.Commands.Update;
using Application.Features.UserNotificationSettings.Queries.GetById;
using Application.Features.UserNotificationSettings.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.UserNotificationSettings.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateUserNotificationSettingCommand, UserNotificationSetting>();
        CreateMap<UserNotificationSetting, CreatedUserNotificationSettingResponse>();

        CreateMap<UpdateUserNotificationSettingCommand, UserNotificationSetting>();
        CreateMap<UserNotificationSetting, UpdatedUserNotificationSettingResponse>();

        CreateMap<DeleteUserNotificationSettingCommand, UserNotificationSetting>();
        CreateMap<UserNotificationSetting, DeletedUserNotificationSettingResponse>();

        CreateMap<UserNotificationSetting, GetByIdUserNotificationSettingResponse>();

        CreateMap<UserNotificationSetting, GetListUserNotificationSettingListItemDto>();
        CreateMap<IPaginate<UserNotificationSetting>, GetListResponse<GetListUserNotificationSettingListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<UserNotificationSetting, GetListUserNotificationSettingListItemDto>());
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