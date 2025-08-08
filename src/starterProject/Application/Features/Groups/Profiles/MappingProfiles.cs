using Application.Features.Groups.Commands.Create;
using Application.Features.Groups.Commands.Delete;
using Application.Features.Groups.Commands.Update;
using Application.Features.Groups.Queries.GetById;
using Application.Features.Groups.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;
using Domain.DTos;

namespace Application.Features.Groups.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateGroupCommand, Group>();
        CreateMap<Group, CreatedGroupResponse>();

        CreateMap<UpdateGroupCommand, Group>();
        CreateMap<Group, UpdatedGroupResponse>();

        CreateMap<DeleteGroupCommand, Group>();
        CreateMap<Group, DeletedGroupResponse>();

        CreateMap<Group, GetByIdGroupResponse>();
        CreateMap<OperationClaim, OperationClaimDto>();

        CreateMap<Group, GetListGroupListItemDto>();
        CreateMap<IPaginate<Group>, GetListResponse<GetListGroupListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<Group, GetListGroupListItemDto>());
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