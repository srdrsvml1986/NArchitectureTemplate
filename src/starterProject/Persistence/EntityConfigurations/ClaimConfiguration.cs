using Application.Features.Auth.Constants;
using Application.Features.Claims.Constants;
using Application.Features.UserClaims.Constants;
using Application.Features.Users.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NArchitecture.Core.Security.Constants;
using Application.Features.Groups.Constants;
using Application.Features.GroupClaims.Constants;
using Application.Features.UserGroups.Constants;

namespace Persistence.EntityConfigurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims").HasKey(oc => oc.Id);

        builder.Property(oc => oc.Id).HasColumnName("Id").IsRequired();
        builder.Property(oc => oc.Name).HasColumnName("Name").IsRequired();
        builder.Property(oc => oc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(oc => oc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(oc => oc.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(oc => !oc.DeletedDate.HasValue);

        builder.HasData(_seeds);

        builder.HasBaseType((string)null!);
    }

    public static int AdminId => 1;
    private IEnumerable<Claim> _seeds
    {
        get
        {
            yield return new() { Id = AdminId, Name = GeneralClaims.Admin };

            IEnumerable<Claim> featureClaims = getFeatureOperationClaims(AdminId);
            foreach (Claim claim in featureClaims)
                yield return claim;
        }
    }

#pragma warning disable S1854 // Unused assignments should be removed
    private IEnumerable<Claim> getFeatureOperationClaims(int initialId)
    {
        int lastId = initialId;
        List<Claim> featureClaims = new();

        #region Auth
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = AuthOperationClaims.Admin },
                new() { Id = ++lastId, Name = AuthOperationClaims.Read },
                new() { Id = ++lastId, Name = AuthOperationClaims.Write },
                new() { Id = ++lastId, Name = AuthOperationClaims.RevokeToken },
            ]
        );
        #endregion

        #region OperationClaims
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = Claims.Admin },
                new() { Id = ++lastId, Name = Claims.Read },
                new() { Id = ++lastId, Name = Claims.Write },
                new() { Id = ++lastId, Name = Claims.Create },
                new() { Id = ++lastId, Name = Claims.Update },
                new() { Id = ++lastId, Name = Claims.Delete },
            ]
        );
        #endregion

        #region UserOperationClaims
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UserClaims.Admin },
                new() { Id = ++lastId, Name = UserClaims.Read },
                new() { Id = ++lastId, Name = UserClaims.Write },
                new() { Id = ++lastId, Name = UserClaims.Create },
                new() { Id = ++lastId, Name = UserClaims.Update },
                new() { Id = ++lastId, Name = UserClaims.Delete },
            ]
        );
        #endregion

        #region Users
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UsersOperationClaims.Admin },
                new() { Id = ++lastId, Name = UsersOperationClaims.Read },
                new() { Id = ++lastId, Name = UsersOperationClaims.Write },
                new() { Id = ++lastId, Name = UsersOperationClaims.Create },
                new() { Id = ++lastId, Name = UsersOperationClaims.Update },
                new() { Id = ++lastId, Name = UsersOperationClaims.Delete },
            ]
        );
        #endregion

        
        #region Groups CRUD
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = GroupsOperationClaims.Admin },
                new() { Id = ++lastId, Name = GroupsOperationClaims.Read },
                new() { Id = ++lastId, Name = GroupsOperationClaims.Write },
                new() { Id = ++lastId, Name = GroupsOperationClaims.Create },
                new() { Id = ++lastId, Name = GroupsOperationClaims.Update },
                new() { Id = ++lastId, Name = GroupsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region GroupClaims CRUD
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = GroupClaimsOperationClaims.Admin },
                new() { Id = ++lastId, Name = GroupClaimsOperationClaims.Read },
                new() { Id = ++lastId, Name = GroupClaimsOperationClaims.Write },
                new() { Id = ++lastId, Name = GroupClaimsOperationClaims.Create },
                new() { Id = ++lastId, Name = GroupClaimsOperationClaims.Update },
                new() { Id = ++lastId, Name = GroupClaimsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region UserGroups CRUD
        featureClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UserGroupsClaims.Admin },
                new() { Id = ++lastId, Name = UserGroupsClaims.Read },
                new() { Id = ++lastId, Name = UserGroupsClaims.Write },
                new() { Id = ++lastId, Name = UserGroupsClaims.Create },
                new() { Id = ++lastId, Name = UserGroupsClaims.Update },
                new() { Id = ++lastId, Name = UserGroupsClaims.Delete },
            ]
        );
        #endregion
        
        return featureClaims;
    }
#pragma warning restore S1854 // Unused assignments should be removed
}
