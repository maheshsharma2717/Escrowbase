using SR.EscrowBaseWeb.TagsAndFileMapping;
using SR.EscrowBaseWeb.EscrowFileTag;
using SR.EscrowBaseWeb.EscrowUserNote;
using SR.EscrowBaseWeb.EscrowDirectMessage;
using SR.EscrowBaseWeb.EscrowFileReminder;
using SR.EscrowBaseWeb.EscrowFileMaster;
using SR.EscrowBaseWeb.SREscrowFileHistory;
using SR.EscrowBaseWeb.EsignRoleMapping;
using SR.EscrowBaseWeb.EsignCompany;
using SR.EscrowBaseWeb.SrAssignedFilesDetails;
using SR.EscrowBaseWeb.E_SignRecords;
using SR.EscrowBaseWeb.SrEscrows;
using SR.EscrowBaseWeb.SrInvitationRecords;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.UserFileLogs;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SrEscrowUserMapping;
using SR.EscrowBaseWeb.SREscrowClient;
using Abp.Organizations;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SR.EscrowBaseWeb.Authorization.Delegation;
using SR.EscrowBaseWeb.Authorization.Roles;
using SR.EscrowBaseWeb.Authorization.Users;
using SR.EscrowBaseWeb.Chat;
using SR.EscrowBaseWeb.Editions;
using SR.EscrowBaseWeb.Friendships;
using SR.EscrowBaseWeb.MultiTenancy;
using SR.EscrowBaseWeb.MultiTenancy.Accounting;
using SR.EscrowBaseWeb.MultiTenancy.Payments;
using SR.EscrowBaseWeb.Storage;
using SR.EscrowBaseWeb.SREnterprise;
using SR.EscrowBaseWeb.Invitee;
using SR.EscrowBaseWeb.SRUserAnswer;
using SR.EscrowBaseWeb.SRSecurityQuestion;
using Abp.IdentityServer4vNext;
using SR.EscrowBaseWeb.EsignCompany;


namespace SR.EscrowBaseWeb.EntityFrameworkCore
{
    public class EscrowBaseWebDbContext : AbpZeroDbContext<Tenant, Role, User, EscrowBaseWebDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<TagsAndFileMappings> TagsAndFileMappingses { get; set; }

        public virtual DbSet<EscrowFileTags> EscrowFileTagses { get; set; }

        public virtual DbSet<EscrowUserNotes> EscrowUserNoteses { get; set; }

        public virtual DbSet<EscrowDirectMessageDetails> EscrowDirectMessageDetailses { get; set; }

        public virtual DbSet<SrEscrowFileReminder> SrEscrowFileReminders { get; set; }

        public virtual DbSet<SREscrowFileMaster> SREscrowFileMasters { get; set; }

        public virtual DbSet<EscrowFileHistory> EscrowFileHistories { get; set; }

        public virtual DbSet<EsignRoleMappings> EsignRoleMappingses { get; set; }

        public virtual DbSet<EsignCompanyMapping> EsignCompanyMappings { get; set; }

        public virtual DbSet<SrAssignedFilesDetail> SrAssignedFilesDetails { get; set; }

        public virtual DbSet<E_SignRecord> E_SignRecords { get; set; }

        public virtual DbSet<SrEscrow> SrEscrows { get; set; }

        public virtual DbSet<SrInvitationRecord> SrInvitationRecords { get; set; }

        public virtual DbSet<EscrowDetail> EscrowDetails { get; set; }

        public virtual DbSet<UserFileLog> UserFileLogs { get; set; }

        public virtual DbSet<SrFileMapping> SrFileMappings { get; set; }

        public virtual DbSet<EscrowUserMapping> EscrowUserMappings { get; set; }

        public virtual DbSet<EscrowClient> EscrowClients { get; set; }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }
        public virtual DbSet<Enterprise> Enterprises { get; set; }
        public virtual DbSet<SRInvitee> SRInvitees { get; set; }

        public virtual DbSet<UserAnswer> UserAnswers { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual DbSet<ESignCompany> ESignCompanies { get; set; }


        public EscrowBaseWebDbContext(DbContextOptions<EscrowBaseWebDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EscrowFileTags>(x =>
            {
                x.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<EscrowUserNotes>(x =>
                       {
                           x.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<EsignRoleMappings>(x =>
                       {
                           x.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<UserFileLog>(u =>
                                  {
                                      //u.HasIndex(e => new { e.TenantId });
                                  });
            modelBuilder.Entity<BinaryObject>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
            {
                b.HasQueryFilter(m => !m.IsDeleted)
                    .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
                    .IsUnique();
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.SourceUserId });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId });
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}