using Application.Features.GroupClaims.Commands.Create;
using Application.Features.GroupClaims.Commands.Delete;
using Application.Features.GroupClaims.Commands.Update;
using Application.Features.GroupClaims.Queries.GetById;
using Application.Features.GroupClaims.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.GroupClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateGroupClaimCommand, GroupClaim>();
        CreateMap<GroupClaim, CreatedGroupClaimResponse>();

        CreateMap<UpdateGroupClaimCommand, GroupClaim>();
        CreateMap<GroupClaim, UpdatedGroupClaimResponse>();

        CreateMap<DeleteGroupClaimCommand, GroupClaim>();
        CreateMap<GroupClaim, DeletedGroupClaimResponse>();

        CreateMap<GroupClaim, GetByIdGroupClaimResponse>();

        CreateMap<GroupClaim, GetListGroupClaimListItemDto>();
        CreateMap<IPaginate<GroupClaim>, GetListResponse<GetListGroupClaimListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<GroupClaim, GetListGroupClaimListItemDto>());
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