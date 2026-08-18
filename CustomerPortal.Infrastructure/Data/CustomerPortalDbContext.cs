using CustomerPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Data
{
    public class CustomerPortalDbContext : DbContext
    {
        public CustomerPortalDbContext(DbContextOptions<CustomerPortalDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<OtpChallenge> OtpChallenges => Set<OtpChallenge>();

        public DbSet<UserPin> UserPins => Set<UserPin>();

        public DbSet<UserConsent> UserConsents => Set<UserConsent>();

        public DbSet<UserDevice> UserDevices => Set<UserDevice>();

        public DbSet<ConsentDocument> ConsentDocuments => Set<ConsentDocument>();

        public DbSet<OnboardingRequirement> OnboardingRequirements => Set<OnboardingRequirement>();

        public DbSet<HomeContentCard> HomeContentCards => Set<HomeContentCard>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Uniqueness is filtered on IsActive so that a soft-deleted account does not keep its
            // email address or mobile number reserved forever.
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .HasDatabaseName(IndexNames.UserEmail)
                .IsUnique()
                .HasFilter(ActiveRowFilter);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Mobile)
                .HasDatabaseName(IndexNames.UserMobile)
                .IsUnique()
                .HasFilter(ActiveRowFilter);

            // Every OTP read is "the latest row for this user and channel", so the index carries
            // CreatedAt to make the ordering free.
            modelBuilder.Entity<OtpChallenge>()
                .HasIndex(challenge => new
                {
                    challenge.FkUser,
                    challenge.Channel,
                    challenge.CreatedAt
                })
                .HasDatabaseName(IndexNames.OtpChallengeUserChannelCreatedAt);

            modelBuilder.Entity<UserPin>()
                .HasIndex(userPin => userPin.FkUser)
                .HasDatabaseName(IndexNames.UserPinUser)
                .IsUnique()
                .HasFilter(ActiveRowFilter);

            modelBuilder.Entity<UserConsent>()
                .HasIndex(consent => new { consent.FkUser, consent.DocumentType })
                .HasDatabaseName(IndexNames.UserConsentUserDocumentType);

            modelBuilder.Entity<UserDevice>()
                .HasIndex(device => new { device.FkUser, device.DeviceId })
                .HasDatabaseName(IndexNames.UserDeviceUserDevice)
                .IsUnique()
                .HasFilter(ActiveRowFilter);

            modelBuilder.Entity<ConsentDocument>()
                .HasIndex(document => new { document.DocumentType, document.IsActive })
                .HasDatabaseName(IndexNames.ConsentDocumentTypeActive);

            modelBuilder.Entity<OnboardingRequirement>()
                .HasIndex(requirement => new { requirement.IsActive, requirement.DisplayOrder })
                .HasDatabaseName(IndexNames.OnboardingRequirementActiveOrder);

            modelBuilder.Entity<HomeContentCard>()
                .HasIndex(card => new { card.IsActive, card.DisplayOrder })
                .HasDatabaseName(IndexNames.HomeContentCardActiveOrder);
        }

        private const string ActiveRowFilter = "[IsActive] = 1";

        private static class IndexNames
        {
            public const string UserEmail = "IX_User_Email";
            public const string UserMobile = "IX_User_Mobile";
            public const string OtpChallengeUserChannelCreatedAt = "IX_OtpChallenge_FkUser_Channel_CreatedAt";
            public const string UserPinUser = "IX_UserPin_FkUser";
            public const string UserConsentUserDocumentType = "IX_UserConsent_FkUser_DocumentType";
            public const string UserDeviceUserDevice = "IX_UserDevice_FkUser_DeviceId";
            public const string ConsentDocumentTypeActive = "IX_ConsentDocument_DocumentType_IsActive";
            public const string OnboardingRequirementActiveOrder = "IX_OnboardingRequirement_IsActive_DisplayOrder";
            public const string HomeContentCardActiveOrder = "IX_HomeContentCard_IsActive_DisplayOrder";
        }
    }
}
