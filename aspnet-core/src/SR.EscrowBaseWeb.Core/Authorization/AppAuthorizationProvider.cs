using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.MultiTenancy;

namespace SR.EscrowBaseWeb.Authorization
{
    /// <summary>
    /// Application's authorization provider.
    /// Defines permissions for the application.
    /// See <see cref="AppPermissions"/> for all permission names.
    /// </summary>
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        private readonly bool _isMultiTenancyEnabled;

        public AppAuthorizationProvider(bool isMultiTenancyEnabled)
        {
            _isMultiTenancyEnabled = isMultiTenancyEnabled;
        }

        public AppAuthorizationProvider(IMultiTenancyConfig multiTenancyConfig)
        {
            _isMultiTenancyEnabled = multiTenancyConfig.IsEnabled;
        }

        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //COMMON PERMISSIONS (FOR BOTH OF TENANTS AND HOST)

            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var tagsAndFileMappingses = pages.CreateChildPermission(AppPermissions.Pages_TagsAndFileMappingses, L("TagsAndFileMappingses"), multiTenancySides: MultiTenancySides.Host);
            tagsAndFileMappingses.CreateChildPermission(AppPermissions.Pages_TagsAndFileMappingses_Create, L("CreateNewTagsAndFileMappings"), multiTenancySides: MultiTenancySides.Host);
            tagsAndFileMappingses.CreateChildPermission(AppPermissions.Pages_TagsAndFileMappingses_Edit, L("EditTagsAndFileMappings"), multiTenancySides: MultiTenancySides.Host);
            tagsAndFileMappingses.CreateChildPermission(AppPermissions.Pages_TagsAndFileMappingses_Delete, L("DeleteTagsAndFileMappings"), multiTenancySides: MultiTenancySides.Host);

            var escrowFileTagses = pages.CreateChildPermission(AppPermissions.Pages_EscrowFileTagses, L("EscrowFileTagses"));
            escrowFileTagses.CreateChildPermission(AppPermissions.Pages_EscrowFileTagses_Create, L("CreateNewEscrowFileTags"));
            escrowFileTagses.CreateChildPermission(AppPermissions.Pages_EscrowFileTagses_Edit, L("EditEscrowFileTags"));
            escrowFileTagses.CreateChildPermission(AppPermissions.Pages_EscrowFileTagses_Delete, L("DeleteEscrowFileTags"));

            var escrowUserNoteses = pages.CreateChildPermission(AppPermissions.Pages_EscrowUserNoteses, L("EscrowUserNoteses"));
            escrowUserNoteses.CreateChildPermission(AppPermissions.Pages_EscrowUserNoteses_Create, L("CreateNewEscrowUserNotes"));
            escrowUserNoteses.CreateChildPermission(AppPermissions.Pages_EscrowUserNoteses_Edit, L("EditEscrowUserNotes"));
            escrowUserNoteses.CreateChildPermission(AppPermissions.Pages_EscrowUserNoteses_Delete, L("DeleteEscrowUserNotes"));

            var escrowDirectMessageDetailses = pages.CreateChildPermission(AppPermissions.Pages_EscrowDirectMessageDetailses, L("EscrowDirectMessageDetailses"), multiTenancySides: MultiTenancySides.Host);
            escrowDirectMessageDetailses.CreateChildPermission(AppPermissions.Pages_EscrowDirectMessageDetailses_Create, L("CreateNewEscrowDirectMessageDetails"), multiTenancySides: MultiTenancySides.Host);
            escrowDirectMessageDetailses.CreateChildPermission(AppPermissions.Pages_EscrowDirectMessageDetailses_Edit, L("EditEscrowDirectMessageDetails"), multiTenancySides: MultiTenancySides.Host);
            escrowDirectMessageDetailses.CreateChildPermission(AppPermissions.Pages_EscrowDirectMessageDetailses_Delete, L("DeleteEscrowDirectMessageDetails"), multiTenancySides: MultiTenancySides.Host);

            var srEscrowFileReminders = pages.CreateChildPermission(AppPermissions.Pages_SrEscrowFileReminders, L("SrEscrowFileReminders"), multiTenancySides: MultiTenancySides.Host);
            srEscrowFileReminders.CreateChildPermission(AppPermissions.Pages_SrEscrowFileReminders_Create, L("CreateNewSrEscrowFileReminder"), multiTenancySides: MultiTenancySides.Host);
            srEscrowFileReminders.CreateChildPermission(AppPermissions.Pages_SrEscrowFileReminders_Edit, L("EditSrEscrowFileReminder"), multiTenancySides: MultiTenancySides.Host);
            srEscrowFileReminders.CreateChildPermission(AppPermissions.Pages_SrEscrowFileReminders_Delete, L("DeleteSrEscrowFileReminder"), multiTenancySides: MultiTenancySides.Host);

            var srEscrowFileMasters = pages.CreateChildPermission(AppPermissions.Pages_SREscrowFileMasters, L("SREscrowFileMasters"), multiTenancySides: MultiTenancySides.Host);
            srEscrowFileMasters.CreateChildPermission(AppPermissions.Pages_SREscrowFileMasters_Create, L("CreateNewSREscrowFileMaster"), multiTenancySides: MultiTenancySides.Host);
            srEscrowFileMasters.CreateChildPermission(AppPermissions.Pages_SREscrowFileMasters_Edit, L("EditSREscrowFileMaster"), multiTenancySides: MultiTenancySides.Host);
            srEscrowFileMasters.CreateChildPermission(AppPermissions.Pages_SREscrowFileMasters_Delete, L("DeleteSREscrowFileMaster"), multiTenancySides: MultiTenancySides.Host);

            var escrowFileHistories = pages.CreateChildPermission(AppPermissions.Pages_EscrowFileHistories, L("EscrowFileHistories"), multiTenancySides: MultiTenancySides.Host);
            escrowFileHistories.CreateChildPermission(AppPermissions.Pages_EscrowFileHistories_Create, L("CreateNewEscrowFileHistory"), multiTenancySides: MultiTenancySides.Host);
            escrowFileHistories.CreateChildPermission(AppPermissions.Pages_EscrowFileHistories_Edit, L("EditEscrowFileHistory"), multiTenancySides: MultiTenancySides.Host);
            escrowFileHistories.CreateChildPermission(AppPermissions.Pages_EscrowFileHistories_Delete, L("DeleteEscrowFileHistory"), multiTenancySides: MultiTenancySides.Host);

            var esignRoleMappingses = pages.CreateChildPermission(AppPermissions.Pages_EsignRoleMappingses, L("EsignRoleMappingses"), multiTenancySides: MultiTenancySides.Tenant);
            esignRoleMappingses.CreateChildPermission(AppPermissions.Pages_EsignRoleMappingses_Create, L("CreateNewEsignRoleMappings"), multiTenancySides: MultiTenancySides.Tenant);
            esignRoleMappingses.CreateChildPermission(AppPermissions.Pages_EsignRoleMappingses_Edit, L("EditEsignRoleMappings"), multiTenancySides: MultiTenancySides.Tenant);
            esignRoleMappingses.CreateChildPermission(AppPermissions.Pages_EsignRoleMappingses_Delete, L("DeleteEsignRoleMappings"), multiTenancySides: MultiTenancySides.Tenant);

            var esignCompanyMappings = pages.CreateChildPermission(AppPermissions.Pages_EsignCompanyMappings, L("EsignCompanyMappings"), multiTenancySides: MultiTenancySides.Host);
            esignCompanyMappings.CreateChildPermission(AppPermissions.Pages_EsignCompanyMappings_Create, L("CreateNewEsignCompanyMapping"), multiTenancySides: MultiTenancySides.Host);
            esignCompanyMappings.CreateChildPermission(AppPermissions.Pages_EsignCompanyMappings_Edit, L("EditEsignCompanyMapping"), multiTenancySides: MultiTenancySides.Host);
            esignCompanyMappings.CreateChildPermission(AppPermissions.Pages_EsignCompanyMappings_Delete, L("DeleteEsignCompanyMapping"), multiTenancySides: MultiTenancySides.Host);

            var srAssignedFilesDetails = pages.CreateChildPermission(AppPermissions.Pages_SrAssignedFilesDetails, L("SrAssignedFilesDetails"), multiTenancySides: MultiTenancySides.Host);
            srAssignedFilesDetails.CreateChildPermission(AppPermissions.Pages_SrAssignedFilesDetails_Create, L("CreateNewSrAssignedFilesDetail"), multiTenancySides: MultiTenancySides.Host);
            srAssignedFilesDetails.CreateChildPermission(AppPermissions.Pages_SrAssignedFilesDetails_Edit, L("EditSrAssignedFilesDetail"), multiTenancySides: MultiTenancySides.Host);
            srAssignedFilesDetails.CreateChildPermission(AppPermissions.Pages_SrAssignedFilesDetails_Delete, L("DeleteSrAssignedFilesDetail"), multiTenancySides: MultiTenancySides.Host);

            var e_SignRecords = pages.CreateChildPermission(AppPermissions.Pages_E_SignRecords, L("E_SignRecords"), multiTenancySides: MultiTenancySides.Host);
            e_SignRecords.CreateChildPermission(AppPermissions.Pages_E_SignRecords_Create, L("CreateNewE_SignRecord"), multiTenancySides: MultiTenancySides.Host);
            e_SignRecords.CreateChildPermission(AppPermissions.Pages_E_SignRecords_Edit, L("EditE_SignRecord"), multiTenancySides: MultiTenancySides.Host);
            e_SignRecords.CreateChildPermission(AppPermissions.Pages_E_SignRecords_Delete, L("DeleteE_SignRecord"), multiTenancySides: MultiTenancySides.Host);

            var srEscrows = pages.CreateChildPermission(AppPermissions.Pages_SrEscrows, L("SrEscrows"), multiTenancySides: MultiTenancySides.Host);
            srEscrows.CreateChildPermission(AppPermissions.Pages_SrEscrows_Create, L("CreateNewSrEscrow"), multiTenancySides: MultiTenancySides.Host);
            srEscrows.CreateChildPermission(AppPermissions.Pages_SrEscrows_Edit, L("EditSrEscrow"), multiTenancySides: MultiTenancySides.Host);
            srEscrows.CreateChildPermission(AppPermissions.Pages_SrEscrows_Delete, L("DeleteSrEscrow"), multiTenancySides: MultiTenancySides.Host);

            var srInvitationRecords = pages.CreateChildPermission(AppPermissions.Pages_SrInvitationRecords, L("SrInvitationRecords"), multiTenancySides: MultiTenancySides.Host);
            srInvitationRecords.CreateChildPermission(AppPermissions.Pages_SrInvitationRecords_Create, L("CreateNewSrInvitationRecord"), multiTenancySides: MultiTenancySides.Host);
            srInvitationRecords.CreateChildPermission(AppPermissions.Pages_SrInvitationRecords_Edit, L("EditSrInvitationRecord"), multiTenancySides: MultiTenancySides.Host);
            srInvitationRecords.CreateChildPermission(AppPermissions.Pages_SrInvitationRecords_Delete, L("DeleteSrInvitationRecord"), multiTenancySides: MultiTenancySides.Host);

            var userFileLogs = pages.CreateChildPermission(AppPermissions.Pages_UserFileLogs, L("UserFileLogs"), multiTenancySides: MultiTenancySides.Host);
            userFileLogs.CreateChildPermission(AppPermissions.Pages_UserFileLogs_Create, L("CreateNewUserFileLog"), multiTenancySides: MultiTenancySides.Host);
            userFileLogs.CreateChildPermission(AppPermissions.Pages_UserFileLogs_Edit, L("EditUserFileLog"), multiTenancySides: MultiTenancySides.Host);
            userFileLogs.CreateChildPermission(AppPermissions.Pages_UserFileLogs_Delete, L("DeleteUserFileLog"), multiTenancySides: MultiTenancySides.Host);

            var escrowDetails = pages.CreateChildPermission(AppPermissions.Pages_EscrowDetails, L("EscrowDetails"), multiTenancySides: MultiTenancySides.Host);
            escrowDetails.CreateChildPermission(AppPermissions.Pages_EscrowDetails_Create, L("CreateNewEscrowDetail"), multiTenancySides: MultiTenancySides.Host);
            escrowDetails.CreateChildPermission(AppPermissions.Pages_EscrowDetails_Edit, L("EditEscrowDetail"), multiTenancySides: MultiTenancySides.Host);
            escrowDetails.CreateChildPermission(AppPermissions.Pages_EscrowDetails_Delete, L("DeleteEscrowDetail"), multiTenancySides: MultiTenancySides.Host);

            var srFileMappings = pages.CreateChildPermission(AppPermissions.Pages_SrFileMappings, L("SrFileMappings"), multiTenancySides: MultiTenancySides.Host);
            srFileMappings.CreateChildPermission(AppPermissions.Pages_SrFileMappings_Create, L("CreateNewSrFileMapping"), multiTenancySides: MultiTenancySides.Host);
            srFileMappings.CreateChildPermission(AppPermissions.Pages_SrFileMappings_Edit, L("EditSrFileMapping"), multiTenancySides: MultiTenancySides.Host);
            srFileMappings.CreateChildPermission(AppPermissions.Pages_SrFileMappings_Delete, L("DeleteSrFileMapping"), multiTenancySides: MultiTenancySides.Host);

            var escrowUserMappings = pages.CreateChildPermission(AppPermissions.Pages_EscrowUserMappings, L("EscrowUserMappings"), multiTenancySides: MultiTenancySides.Host);
            escrowUserMappings.CreateChildPermission(AppPermissions.Pages_EscrowUserMappings_Create, L("CreateNewEscrowUserMapping"), multiTenancySides: MultiTenancySides.Host);
            escrowUserMappings.CreateChildPermission(AppPermissions.Pages_EscrowUserMappings_Edit, L("EditEscrowUserMapping"), multiTenancySides: MultiTenancySides.Host);
            escrowUserMappings.CreateChildPermission(AppPermissions.Pages_EscrowUserMappings_Delete, L("DeleteEscrowUserMapping"), multiTenancySides: MultiTenancySides.Host);

            var escrowClients = pages.CreateChildPermission(AppPermissions.Pages_EscrowClients, L("EscrowClients"), multiTenancySides: MultiTenancySides.Host);
            escrowClients.CreateChildPermission(AppPermissions.Pages_EscrowClients_Create, L("CreateNewEscrowClient"), multiTenancySides: MultiTenancySides.Host);
            escrowClients.CreateChildPermission(AppPermissions.Pages_EscrowClients_Edit, L("EditEscrowClient"), multiTenancySides: MultiTenancySides.Host);
            escrowClients.CreateChildPermission(AppPermissions.Pages_EscrowClients_Delete, L("DeleteEscrowClient"), multiTenancySides: MultiTenancySides.Host);

            var userTypes = pages.CreateChildPermission(AppPermissions.Pages_UserTypes, L("UserTypes"), multiTenancySides: MultiTenancySides.Host);
            userTypes.CreateChildPermission(AppPermissions.Pages_UserTypes_Create, L("CreateNewUserType"), multiTenancySides: MultiTenancySides.Host);
            userTypes.CreateChildPermission(AppPermissions.Pages_UserTypes_Edit, L("EditUserType"), multiTenancySides: MultiTenancySides.Host);
            userTypes.CreateChildPermission(AppPermissions.Pages_UserTypes_Delete, L("DeleteUserType"), multiTenancySides: MultiTenancySides.Host);

            var srInvitees = pages.CreateChildPermission(AppPermissions.Pages_SRInvitees, L("SRInvitees"), multiTenancySides: MultiTenancySides.Host);
            srInvitees.CreateChildPermission(AppPermissions.Pages_SRInvitees_Create, L("CreateNewSRInvitee"), multiTenancySides: MultiTenancySides.Host);
            srInvitees.CreateChildPermission(AppPermissions.Pages_SRInvitees_Edit, L("EditSRInvitee"), multiTenancySides: MultiTenancySides.Host);
            srInvitees.CreateChildPermission(AppPermissions.Pages_SRInvitees_Delete, L("DeleteSRInvitee"), multiTenancySides: MultiTenancySides.Host);

            var userAnswers = pages.CreateChildPermission(AppPermissions.Pages_UserAnswers, L("UserAnswers"), multiTenancySides: MultiTenancySides.Host);
            userAnswers.CreateChildPermission(AppPermissions.Pages_UserAnswers_Create, L("CreateNewUserAnswer"), multiTenancySides: MultiTenancySides.Host);
            userAnswers.CreateChildPermission(AppPermissions.Pages_UserAnswers_Edit, L("EditUserAnswer"), multiTenancySides: MultiTenancySides.Host);
            userAnswers.CreateChildPermission(AppPermissions.Pages_UserAnswers_Delete, L("DeleteUserAnswer"), multiTenancySides: MultiTenancySides.Host);

            var securityQuestions = pages.CreateChildPermission(AppPermissions.Pages_SecurityQuestions, L("SecurityQuestions"), multiTenancySides: MultiTenancySides.Host);
            securityQuestions.CreateChildPermission(AppPermissions.Pages_SecurityQuestions_Create, L("CreateNewSecurityQuestion"), multiTenancySides: MultiTenancySides.Host);
            securityQuestions.CreateChildPermission(AppPermissions.Pages_SecurityQuestions_Edit, L("EditSecurityQuestion"), multiTenancySides: MultiTenancySides.Host);
            securityQuestions.CreateChildPermission(AppPermissions.Pages_SecurityQuestions_Delete, L("DeleteSecurityQuestion"), multiTenancySides: MultiTenancySides.Host);

            var enterprises = pages.CreateChildPermission(AppPermissions.Pages_Enterprises, L("Enterprises"));
            enterprises.CreateChildPermission(AppPermissions.Pages_Enterprises_Create, L("CreateNewEnterprise"));
            enterprises.CreateChildPermission(AppPermissions.Pages_Enterprises_Edit, L("EditEnterprise"));
            enterprises.CreateChildPermission(AppPermissions.Pages_Enterprises_Delete, L("DeleteEnterprise"));

            pages.CreateChildPermission(AppPermissions.Pages_DemoUiComponents, L("DemoUiComponents"));

            var administration = pages.CreateChildPermission(AppPermissions.Pages_Administration, L("Administration"));

            var roles = administration.CreateChildPermission(AppPermissions.Pages_Administration_Roles, L("Roles"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Create, L("CreatingNewRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Edit, L("EditingRole"));
            roles.CreateChildPermission(AppPermissions.Pages_Administration_Roles_Delete, L("DeletingRole"));

            var users = administration.CreateChildPermission(AppPermissions.Pages_Administration_Users, L("Users"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Create, L("CreatingNewUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Edit, L("EditingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Delete, L("DeletingUser"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_ChangePermissions, L("ChangingPermissions"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Impersonation, L("LoginForUsers"));
            users.CreateChildPermission(AppPermissions.Pages_Administration_Users_Unlock, L("Unlock"));

            var languages = administration.CreateChildPermission(AppPermissions.Pages_Administration_Languages, L("Languages"));
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Create, L("CreatingNewLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Edit, L("EditingLanguage"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_Delete, L("DeletingLanguages"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            languages.CreateChildPermission(AppPermissions.Pages_Administration_Languages_ChangeTexts, L("ChangingTexts"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_AuditLogs, L("AuditLogs"));

            var organizationUnits = administration.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles, L("ManagingRoles"));

            administration.CreateChildPermission(AppPermissions.Pages_Administration_UiCustomization, L("VisualSettings"));

            var webhooks = administration.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription, L("Webhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Create, L("CreatingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Edit, L("EditingWebhooks"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_ChangeActivity, L("ChangingWebhookActivity"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_WebhookSubscription_Detail, L("DetailingSubscription"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ListSendAttempts, L("ListingSendAttempts"));
            webhooks.CreateChildPermission(AppPermissions.Pages_Administration_Webhook_ResendWebhook, L("ResendingWebhook"));

            var dynamicProperties = administration.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties, L("DynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Create, L("CreatingDynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Edit, L("EditingDynamicProperties"));
            dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicProperties_Delete, L("DeletingDynamicProperties"));

            var dynamicPropertyValues = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue, L("DynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Create, L("CreatingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Edit, L("EditingDynamicPropertyValue"));
            dynamicPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicPropertyValue_Delete, L("DeletingDynamicPropertyValue"));

            var dynamicEntityProperties = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties, L("DynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Create, L("CreatingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Edit, L("EditingDynamicEntityProperties"));
            dynamicEntityProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityProperties_Delete, L("DeletingDynamicEntityProperties"));

            var dynamicEntityPropertyValues = dynamicProperties.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue, L("EntityDynamicPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Create, L("CreatingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Edit, L("EditingDynamicEntityPropertyValue"));
            dynamicEntityPropertyValues.CreateChildPermission(AppPermissions.Pages_Administration_DynamicEntityPropertyValue_Delete, L("DeletingDynamicEntityPropertyValue"));

            //TENANT-SPECIFIC PERMISSIONS

            pages.CreateChildPermission(AppPermissions.Pages_Tenant_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Tenant);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement, L("Subscription"), multiTenancySides: MultiTenancySides.Tenant);

            //HOST-SPECIFIC PERMISSIONS

            var editions = pages.CreateChildPermission(AppPermissions.Pages_Editions, L("Editions"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Create, L("CreatingNewEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Edit, L("EditingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_Delete, L("DeletingEdition"), multiTenancySides: MultiTenancySides.Host);
            editions.CreateChildPermission(AppPermissions.Pages_Editions_MoveTenantsToAnotherEdition, L("MoveTenantsToAnotherEdition"), multiTenancySides: MultiTenancySides.Host);

            var tenants = pages.CreateChildPermission(AppPermissions.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Create, L("CreatingNewTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Edit, L("EditingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_ChangeFeatures, L("ChangingFeatures"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Delete, L("DeletingTenant"), multiTenancySides: MultiTenancySides.Host);
            tenants.CreateChildPermission(AppPermissions.Pages_Tenants_Impersonation, L("LoginForTenants"), multiTenancySides: MultiTenancySides.Host);

            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Settings, L("Settings"), multiTenancySides: MultiTenancySides.Host);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Maintenance, L("Maintenance"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_HangfireDashboard, L("HangfireDashboard"), multiTenancySides: _isMultiTenancyEnabled ? MultiTenancySides.Host : MultiTenancySides.Tenant);
            administration.CreateChildPermission(AppPermissions.Pages_Administration_Host_Dashboard, L("Dashboard"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, EscrowBaseWebConsts.LocalizationSourceName);
        }
    }
}