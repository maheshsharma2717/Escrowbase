using SR.EscrowBaseWeb.EscrowUserNote;
using SR.EscrowBaseWeb.EscrowFileReminder;
using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using SR.EscrowBaseWeb.EsignCompany;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SrEscrowUserMapping;
using SR.EscrowBaseWeb.SREscrowClient;
using System;
using System.Linq;
using Abp.Organizations;
using SR.EscrowBaseWeb.Authorization.Roles;
using SR.EscrowBaseWeb.MultiTenancy;

namespace SR.EscrowBaseWeb.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            typeof(EscrowUserNotes),
            typeof(SrEscrowFileReminder),
            typeof(SREscrowFileMaster),
            typeof(EscrowFileHistory),
            typeof(EsignCompanyMapping),
            typeof(SrEscrow),
            typeof(SrFileMapping),
            typeof(EscrowUserMapping),
            typeof(EscrowClient),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            typeof(EscrowUserNotes),
            typeof(OrganizationUnit), typeof(Role)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(TenantSideTrackedTypes)
                .GroupBy(type => type.FullName)
                .Select(types => types.First())
                .ToArray();
    }
}