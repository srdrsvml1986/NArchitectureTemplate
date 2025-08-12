using Application.Features.ExceptionLogs.Commands.Create;
using Application.Features.ExceptionLogs.Commands.Delete;
using Application.Features.ExceptionLogs.Commands.Update;
using Application.Features.ExceptionLogs.Queries.GetById;
using Application.Features.ExceptionLogs.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.ExceptionLogs.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateExceptionLogCommand, ExceptionLog>();
        CreateMap<ExceptionLog, CreatedExceptionLogResponse>();

        CreateMap<UpdateExceptionLogCommand, ExceptionLog>();
        CreateMap<ExceptionLog, UpdatedExceptionLogResponse>();

        CreateMap<DeleteExceptionLogCommand, ExceptionLog>();
        CreateMap<ExceptionLog, DeletedExceptionLogResponse>();

        CreateMap<ExceptionLog, GetByIdExceptionLogResponse>();

        CreateMap<ExceptionLog, GetListExceptionLogListItemDto>();
        CreateMap<IPaginate<ExceptionLog>, GetListResponse<GetListExceptionLogListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<ExceptionLog, GetListExceptionLogListItemDto>());
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