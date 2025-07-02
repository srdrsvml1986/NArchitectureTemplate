using Application.Features.RoleOperationClaims.Commands.Create;
using Application.Features.RoleOperationClaims.Commands.Delete;
using Application.Features.RoleOperationClaims.Commands.Update;
using Application.Features.RoleOperationClaims.Queries.GetById;
using Application.Features.RoleOperationClaims.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.RoleOperationClaims.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateRoleOperationClaimCommand, RoleOperationClaim>();
        CreateMap<RoleOperationClaim, CreatedRoleOperationClaimResponse>();

        CreateMap<UpdateRoleOperationClaimCommand, RoleOperationClaim>();
        CreateMap<RoleOperationClaim, UpdatedRoleOperationClaimResponse>();

        CreateMap<DeleteRoleOperationClaimCommand, RoleOperationClaim>();
        CreateMap<RoleOperationClaim, DeletedRoleOperationClaimResponse>();

        CreateMap<RoleOperationClaim, GetByIdRoleOperationClaimResponse>();

        CreateMap<RoleOperationClaim, GetListRoleOperationClaimListItemDto>();
        CreateMap<IPaginate<RoleOperationClaim>, GetListResponse<GetListRoleOperationClaimListItemDto>>().ConvertUsing(new PaginateToGetListResponseConverter<RoleOperationClaim, GetListRoleOperationClaimListItemDto>());
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