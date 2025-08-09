using Application.Features.GroupOperationClaims.Commands.Create;
using Application.Features.GroupOperationClaims.Commands.Delete;
using Application.Features.GroupOperationClaims.Commands.Update;
using Application.Features.GroupOperationClaims.Queries.GetById;
using Application.Features.GroupOperationClaims.Queries.GetList;
using AutoMapper;
using NArchitectureTemplate.Core.Application.Responses;
using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Paging;

namespace Application.Features.GroupOperationClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateGroupOperationClaimCommand, GroupOperationClaim>();
        CreateMap<GroupOperationClaim, CreatedGroupOperationClaimResponse>();

        CreateMap<UpdateGroupOperationClaimCommand, GroupOperationClaim>();
        CreateMap<GroupOperationClaim, UpdatedGroupOperationClaimResponse>();

        CreateMap<DeleteGroupOperationClaimCommand, GroupOperationClaim>();
        CreateMap<GroupOperationClaim, DeletedGroupOperationClaimResponse>();

        CreateMap<GroupOperationClaim, GetByIdGroupOperationClaimResponse>();

        CreateMap<GroupOperationClaim, GetListGroupOperationClaimListItemDto>();
        CreateMap<IPaginate<GroupOperationClaim>, GetListResponse<GetListGroupOperationClaimListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<GroupOperationClaim, GetListGroupOperationClaimListItemDto>());
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