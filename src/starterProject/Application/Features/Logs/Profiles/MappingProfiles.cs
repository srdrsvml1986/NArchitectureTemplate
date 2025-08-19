using Application.Features.Logs.Commands.Create;
using Application.Features.Logs.Commands.Delete;
using Application.Features.Logs.Commands.Update;
using Application.Features.Logs.Queries.GetById;
using Application.Features.Logs.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.Logs.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateLogCommand, Log>();
        CreateMap<Log, CreatedLogResponse>();

        CreateMap<UpdateLogCommand, Log>();
        CreateMap<Log, UpdatedLogResponse>();

        CreateMap<DeleteLogCommand, Log>();
        CreateMap<Log, DeletedLogResponse>();

        CreateMap<Log, GetByIdLogResponse>();

        CreateMap<Log, GetListLogListItemDto>();
        CreateMap<IPaginate<Log>, GetListResponse<GetListLogListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<Log, GetListLogListItemDto>());
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