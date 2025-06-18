using SR.EscrowBaseWeb.TagsAndFileMapping.Dtos;
using SR.EscrowBaseWeb.TagsAndFileMapping;
using SR.EscrowBaseWeb.EscrowFileTag.Dtos;
using SR.EscrowBaseWeb.EscrowFileTag;
using SR.EscrowBaseWeb.EscrowUserNote.Dtos;
using SR.EscrowBaseWeb.EscrowUserNote;
using SR.EscrowBaseWeb.EscrowDirectMessage.Dtos;
using SR.EscrowBaseWeb.EscrowDirectMessage;
using SR.EscrowBaseWeb.EscrowFileReminder.Dtos;
using SR.EscrowBaseWeb.EscrowFileReminder;
using SR.EscrowBaseWeb.EscrowFileMaster.Dtos;
using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.SREscrowFileHistory.Dtos;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using SR.EscrowBaseWeb.EsignRoleMapping.Dtos;
using SR.EscrowBaseWeb.EsignRoleMapping;
using SR.EscrowBaseWeb.EsignCompany.Dtos;
using SR.EscrowBaseWeb.EsignCompany;
using SR.EscrowBaseWeb.SrAssignedFilesDetails.Dtos;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;
using SR.EscrowBaseWeb.E_SignRecords.Dtos;
using SR.EscrowBaseWeb.E_SignRecords;
using SR.EscrowBaseWeb.SrEscrows.Dtos;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SrInvitationRecords.Dtos;
using SR.EscrowBaseWeb.SrInvitationRecords;
using SR.EscrowBaseWeb.EscrowDetails.Dtos;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.UserFileLogs.Dtos;
using SR.EscrowBaseWeb.UserFileLogs;
using SR.EscrowBaseWeb.SRFileMapping.Dtos;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SrEscrowUserMapping.Dtos;
using SR.EscrowBaseWeb.SrEscrowUserMapping;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;
using SR.EscrowBaseWeb.SREscrowClient;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.DynamicEntityProperties;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Abp.Webhooks;
using AutoMapper;
using SR.EscrowBaseWeb.Auditing.Dto;
using SR.EscrowBaseWeb.Authorization.Accounts.Dto;
using SR.EscrowBaseWeb.Authorization.Delegation;
using SR.EscrowBaseWeb.Authorization.Permissions.Dto;
using SR.EscrowBaseWeb.Authorization.Roles;
using SR.EscrowBaseWeb.Authorization.Roles.Dto;
using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.Authorization.Users.Delegation.Dto;
using SR.EscrowBaseWeb.Authorization.Users.Dto;
using SR.EscrowBaseWeb.Authorization.Users.Importing.Dto;
using SR.EscrowBaseWeb.Authorization.Users.Profile.Dto;
using SR.EscrowBaseWeb.Chat;
using SR.EscrowBaseWeb.Chat.Dto;
using SR.EscrowBaseWeb.DynamicEntityProperties.Dto;
using SR.EscrowBaseWeb.Editions;
using SR.EscrowBaseWeb.Editions.Dto;
using SR.EscrowBaseWeb.Friendships;
using SR.EscrowBaseWeb.Friendships.Cache;
using SR.EscrowBaseWeb.Friendships.Dto;
using SR.EscrowBaseWeb.Localization.Dto;
using SR.EscrowBaseWeb.MultiTenancy;
using SR.EscrowBaseWeb.MultiTenancy.Dto;
using SR.EscrowBaseWeb.MultiTenancy.HostDashboard.Dto;
using SR.EscrowBaseWeb.MultiTenancy.Payments;
using SR.EscrowBaseWeb.MultiTenancy.Payments.Dto;
using SR.EscrowBaseWeb.Notifications.Dto;
using SR.EscrowBaseWeb.Organizations.Dto;
using SR.EscrowBaseWeb.Sessions.Dto;
using SR.EscrowBaseWeb.WebHooks.Dto;
using SR.EscrowBaseWeb.SRUserAnswer.Dtos;
using SR.EscrowBaseWeb.SRUserAnswer;
using SR.EscrowBaseWeb.SRSecurityQuestion.Dtos;
using SR.EscrowBaseWeb.SRSecurityQuestion;
using SR.EscrowBaseWeb.SREnterprise.Dtos;
using SR.EscrowBaseWeb.SREnterprise;
using Abp.Extensions;

namespace SR.EscrowBaseWeb
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditTagsAndFileMappingsDto, TagsAndFileMappings>().ReverseMap();
            configuration.CreateMap<TagsAndFileMappingsDto, TagsAndFileMappings>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowFileTagsDto, EscrowFileTags>().ReverseMap();
            configuration.CreateMap<EscrowFileTagsDto, EscrowFileTags>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowUserNotesDto, EscrowUserNotes>().ReverseMap();
            configuration.CreateMap<EscrowUserNotesDto, EscrowUserNotes>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowDirectMessageDetailsDto, EscrowDirectMessageDetails>().ReverseMap();
            configuration.CreateMap<EscrowDirectMessageDetailsDto, EscrowDirectMessageDetails>().ReverseMap();
            configuration.CreateMap<CreateOrEditSrEscrowFileReminderDto, SrEscrowFileReminder>().ReverseMap();
            configuration.CreateMap<SrEscrowFileReminderDto, SrEscrowFileReminder>().ReverseMap();
            configuration.CreateMap<CreateOrEditSREscrowFileMasterDto, SREscrowFileMaster>().ReverseMap();
            configuration.CreateMap<SREscrowFileMasterDto, SREscrowFileMaster>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowFileHistoryDto, EscrowFileHistory>().ReverseMap();
            configuration.CreateMap<EscrowFileHistoryDto, EscrowFileHistory>().ReverseMap();
            configuration.CreateMap<CreateOrEditEsignRoleMappingsDto, EsignRoleMappings>().ReverseMap();
            configuration.CreateMap<EsignRoleMappingsDto, EsignRoleMappings>().ReverseMap();
            configuration.CreateMap<CreateOrEditEsignCompanyMappingDto, EsignCompanyMapping>().ReverseMap();
            configuration.CreateMap<EsignCompanyMappingDto, EsignCompanyMapping>().ReverseMap();
            configuration.CreateMap<CreateOrEditSrAssignedFilesDetailDto, SrAssignedFilesDetail>().ReverseMap();
            configuration.CreateMap<SrAssignedFilesDetailDto, SrAssignedFilesDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditE_SignRecordDto, E_SignRecord>().ReverseMap();
            configuration.CreateMap<E_SignRecordDto, E_SignRecord>().ReverseMap();
            configuration.CreateMap<CreateOrEditSrEscrowDto, SrEscrow>().ReverseMap();
            configuration.CreateMap<SrEscrowDto, SrEscrow>().ReverseMap();
            configuration.CreateMap<CreateOrEditSrInvitationRecordDto, SrInvitationRecord>().ReverseMap();
            configuration.CreateMap<SrInvitationRecordDto, SrInvitationRecord>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowDetailDto, EscrowDetail>().ReverseMap();
            configuration.CreateMap<EscrowDetailDto, EscrowDetail>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserFileLogDto, UserFileLog>().ReverseMap();
            configuration.CreateMap<UserFileLogDto, UserFileLog>().ReverseMap();
            configuration.CreateMap<CreateOrEditSrFileMappingDto, SrFileMapping>().ReverseMap();
            configuration.CreateMap<SrFileMappingDto, SrFileMapping>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowUserMappingDto, EscrowUserMapping>().ReverseMap();
            configuration.CreateMap<EscrowUserMappingDto, EscrowUserMapping>().ReverseMap();
            configuration.CreateMap<CreateOrEditEscrowClientDto, EscrowClient>().ReverseMap();
            configuration.CreateMap<EscrowClientDto, EscrowClient>().ReverseMap();
            configuration.CreateMap<CreateOrEditEnterpriseDto, Enterprise>().ReverseMap();
            configuration.CreateMap<EnterpriseDto, Enterprise>().ReverseMap();
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();

            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            //Webhooks
            configuration.CreateMap<WebhookSubscription, GetAllSubscriptionsOutput>();
            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOutput>()
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.WebhookName,
                    options => options.MapFrom(l => l.WebhookEvent.WebhookName))
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.Data,
                    options => options.MapFrom(l => l.WebhookEvent.Data));

            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOfWebhookEventOutput>();

            configuration.CreateMap<DynamicProperty, DynamicPropertyDto>().ReverseMap();
            configuration.CreateMap<DynamicPropertyValue, DynamicPropertyValueDto>().ReverseMap();
            configuration.CreateMap<DynamicEntityProperty, DynamicEntityPropertyDto>()
                .ForMember(dto => dto.DynamicPropertyName,
                    options => options.MapFrom(entity => entity.DynamicProperty.DisplayName.IsNullOrEmpty() ? entity.DynamicProperty.PropertyName : entity.DynamicProperty.DisplayName));
            configuration.CreateMap<DynamicEntityPropertyDto, DynamicEntityProperty>();

            configuration.CreateMap<DynamicEntityPropertyValue, DynamicEntityPropertyValueDto>().ReverseMap();
            //User Delegations
            configuration.CreateMap<CreateUserDelegationDto, UserDelegation>();

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
        }
    }
}