using Application.Features.DeviceTokens.Commands.Create;
using Application.Features.DeviceTokens.Commands.Delete;
using Application.Features.DeviceTokens.Commands.Update;
using Application.Features.DeviceTokens.Queries.GetById;
using Application.Features.DeviceTokens.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.DeviceTokens.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateDeviceTokenCommand, DeviceToken>();
        CreateMap<DeviceToken, CreatedDeviceTokenResponse>();

        CreateMap<UpdateDeviceTokenCommand, DeviceToken>();
        CreateMap<DeviceToken, UpdatedDeviceTokenResponse>();

        CreateMap<DeleteDeviceTokenCommand, DeviceToken>();
        CreateMap<DeviceToken, DeletedDeviceTokenResponse>();

        CreateMap<DeviceToken, GetByIdDeviceTokenResponse>();

        CreateMap<DeviceToken, GetListDeviceTokenListItemDto>();
        CreateMap<IPaginate<DeviceToken>, GetListResponse<GetListDeviceTokenListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<DeviceToken, GetListDeviceTokenListItemDto>());
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