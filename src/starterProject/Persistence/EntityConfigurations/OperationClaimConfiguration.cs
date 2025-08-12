using Application.Features.Auth.Constants;
using Application.Features.OperationClaims.Constants;
using Application.Features.UserOperationClaims.Constants;
using Application.Features.Users.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NArchitectureTemplate.Core.Security.Constants;
using Application.Features.Groups.Constants;
using Application.Features.GroupOperationClaims.Constants;
using Application.Features.UserGroups.Constants;
using Application.Features.UserSessions.Constants;
using Application.Features.Logs.Constants;
using Application.Features.ExceptionLogs.Constants;
using Application.Features.Logs.Constants;


namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims").HasKey(oc => oc.Id);

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
    private IEnumerable<OperationClaim> _seeds
    {
        get
        {
            yield return new() { Id = AdminId, Name = GeneralClaims.Admin };

            IEnumerable<OperationClaim> featureClaims = getFeatureOperationClaims(AdminId);
            foreach (OperationClaim claim in featureClaims)
                yield return claim;
        }
    }

#pragma warning disable S1854 // Unused assignments should be removed
    private IEnumerable<OperationClaim> getFeatureOperationClaims(int initialId)
    {
        int lastId = initialId;
        List<OperationClaim> featureOperationClaims = new();

        #region Auth
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = AuthOperationClaims.Admin },
                new() { Id = ++lastId, Name = AuthOperationClaims.Read },
                new() { Id = ++lastId, Name = AuthOperationClaims.Write },
                new() { Id = ++lastId, Name = AuthOperationClaims.RevokeToken },
            ]
        );
        #endregion

        #region OperationClaims
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = OperationClaims.Admin },
                new() { Id = ++lastId, Name = OperationClaims.Read },
                new() { Id = ++lastId, Name = OperationClaims.Write },
                new() { Id = ++lastId, Name = OperationClaims.Create },
                new() { Id = ++lastId, Name = OperationClaims.Update },
                new() { Id = ++lastId, Name = OperationClaims.Delete },
            ]
        );
        #endregion

        #region UserClaims
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UserOperationClaims.Admin },
                new() { Id = ++lastId, Name = UserOperationClaims.Read },
                new() { Id = ++lastId, Name = UserOperationClaims.Write },
                new() { Id = ++lastId, Name = UserOperationClaims.Create },
                new() { Id = ++lastId, Name = UserOperationClaims.Update },
                new() { Id = ++lastId, Name = UserOperationClaims.Delete },
            ]
        );
        #endregion

        #region Users
        featureOperationClaims.AddRange(
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
        featureOperationClaims.AddRange(
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
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = GroupOperationClaims.Admin },
                new() { Id = ++lastId, Name = GroupOperationClaims.Read },
                new() { Id = ++lastId, Name = GroupOperationClaims.Write },
                new() { Id = ++lastId, Name = GroupOperationClaims.Create },
                new() { Id = ++lastId, Name = GroupOperationClaims.Update },
                new() { Id = ++lastId, Name = GroupOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region UserGroups CRUD
        featureOperationClaims.AddRange(
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
        
        
        #region UserSessions CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UserSessionsOperationClaims.Admin },
                new() { Id = ++lastId, Name = UserSessionsOperationClaims.Read },
                new() { Id = ++lastId, Name = UserSessionsOperationClaims.Write },
                new() { Id = ++lastId, Name = UserSessionsOperationClaims.Create },
                new() { Id = ++lastId, Name = UserSessionsOperationClaims.Update },
                new() { Id = ++lastId, Name = UserSessionsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        
        
        
        #region Logs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = LogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = LogsOperationClaims.Read },
                new() { Id = ++lastId, Name = LogsOperationClaims.Write },
                new() { Id = ++lastId, Name = LogsOperationClaims.Create },
                new() { Id = ++lastId, Name = LogsOperationClaims.Update },
                new() { Id = ++lastId, Name = LogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region ExceptionLogs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Read },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Write },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Create },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Update },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Logs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = LogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = LogsOperationClaims.Read },
                new() { Id = ++lastId, Name = LogsOperationClaims.Write },
                new() { Id = ++lastId, Name = LogsOperationClaims.Create },
                new() { Id = ++lastId, Name = LogsOperationClaims.Update },
                new() { Id = ++lastId, Name = LogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Logs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = LogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = LogsOperationClaims.Read },
                new() { Id = ++lastId, Name = LogsOperationClaims.Write },
                new() { Id = ++lastId, Name = LogsOperationClaims.Create },
                new() { Id = ++lastId, Name = LogsOperationClaims.Update },
                new() { Id = ++lastId, Name = LogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Logs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = LogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = LogsOperationClaims.Read },
                new() { Id = ++lastId, Name = LogsOperationClaims.Write },
                new() { Id = ++lastId, Name = LogsOperationClaims.Create },
                new() { Id = ++lastId, Name = LogsOperationClaims.Update },
                new() { Id = ++lastId, Name = LogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region ExceptionLogs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Read },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Write },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Create },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Update },
                new() { Id = ++lastId, Name = ExceptionLogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Logs CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = LogsOperationClaims.Admin },
                new() { Id = ++lastId, Name = LogsOperationClaims.Read },
                new() { Id = ++lastId, Name = LogsOperationClaims.Write },
                new() { Id = ++lastId, Name = LogsOperationClaims.Create },
                new() { Id = ++lastId, Name = LogsOperationClaims.Update },
                new() { Id = ++lastId, Name = LogsOperationClaims.Delete },
            ]
        );
        #endregion
        
        return featureOperationClaims;
    }
#pragma warning restore S1854 // Unused assignments should be removed
}
