using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace skillsBackend.Models
{
    public partial class SkillsContext : DbContext
    {

        public SkillsContext(DbContextOptions<SkillsContext> options) : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<ClientImagesMap> ClientImagesMap { get; set; }
        public virtual DbSet<CustomerPayments> CustomerPayments { get; set; }
        public virtual DbSet<FeedbackFromClients> FeedbackFromClients { get; set; }
        public virtual DbSet<FeedbackFromProviders> FeedbackFromProviders { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<JobDetails> JobDetails { get; set; }
        public virtual DbSet<JobImages> JobImages { get; set; }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<MapJobImages> MapJobImages { get; set; }
        public virtual DbSet<MapJobProvider> MapJobProvider { get; set; }
        public virtual DbSet<MapJobSkills> MapJobSkills { get; set; }
        public virtual DbSet<MapProviderImages> MapProviderImages { get; set; }
        public virtual DbSet<MapProviderSkills> MapProviderSkills { get; set; }
        public virtual DbSet<Offers> Offers { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<SMSVerifications> SMSVerifications { get; set; }
        public virtual DbSet<UserClientDetails> UserClientDetails { get; set; }
        public virtual DbSet<UserProviderDetails> UserProviderDetails { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Wallet> Wallet { get; set; }
        public virtual DbSet<WalletHistory> WalletHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer(@"Server=localhost;Database=skillontime;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer(@"Server=localhost;Database=letskills;Trusted_Connection=True;");

                //optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressLine1).HasMaxLength(50);

                entity.Property(e => e.AddressLine2).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PostCode).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddressT_UserId_To_UsersT_ID");
            });

            modelBuilder.Entity<Applications>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                
                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.Property(e => e.WillingToPayForTheOffer);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Applications)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ApplicationsT_JobsID_To_JobsT_ID");
            });

            modelBuilder.Entity<ClientImagesMap>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientImagesMap)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClientImagesMapT_ID_To_UsersT_ID");
            });

            modelBuilder.Entity<CustomerPayments>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ammount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PaymentDetails)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CustomerPayments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerPaymentsT_UserID_To_Users_ID");
            });

            modelBuilder.Entity<FeedbackFromClients>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FeedBackDescription).HasMaxLength(1000);

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
                          
                entity.Property(e => e.Skills);

                entity.Property(e => e.Communication);

                entity.Property(e => e.Punctuality);

        entity.HasOne(d => d.Job)
                    .WithMany(p => p.FeedbackFromClients)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackFromClientsT_JobID_To_Jobs_ID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedbackFromClients)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackFromClientsT_ProviderID_To_Users_ID");
            });

            modelBuilder.Entity<FeedbackFromProviders>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FeedBackDescription).HasMaxLength(1000);

                entity.Property(e => e.FeedBackRating);

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.FeedbackFromProviders)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackFromProvidersT_JobID_To_Jobs_ID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedbackFromProviders)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackFromProvidersT_ClientID_To_Users_ID");
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.UploadTimestamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<JobImages>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.Url).IsRequired().HasMaxLength(1000);

                entity.Property(e => e.Description).HasMaxLength(100);

                //entity.Property(e => e.UploadTimestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobImages)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobImagesT_JobID_To_Jobs_ID");
            });

            modelBuilder.Entity<JobDetails>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.JobImageId).HasColumnName("JobImageID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobDetails)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobDetailsT_JobID_To_Jobs_ID");
            });

            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.ContactEmail).HasMaxLength(100);

                entity.Property(e => e.ContactTelephone1)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ContactTelephone2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.LocationPostCode).HasMaxLength(10);

                entity.Property(e => e.PlannedFinishDate).HasColumnType("datetime");

                entity.Property(e => e.PlannedStartDate).HasColumnType("datetime");

                entity.Property(e => e.JobTitle)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.RateFixed).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.RatePerHour).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.LocationLat).HasColumnType("decimal(10, 7)");

                entity.Property(e => e.LocationLng).HasColumnType("decimal(10, 7)");
                
                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.Property(e => e.Telephone1Verified)
                    .HasMaxLength(20)
                    .IsUnicode(false); ;

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobsT_ClientID_To_Users_ID");

                entity.HasOne(d => d.Skills)
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobsT_SkillID_To_SkillsT_ID");
            });

            modelBuilder.Entity<MapJobImages>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.JobId).HasColumnName("JobID");
            });

            modelBuilder.Entity<MapJobProvider>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            });

            modelBuilder.Entity<MapJobSkills>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");
            });

            modelBuilder.Entity<MapProviderImages>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            });

            modelBuilder.Entity<MapProviderSkills>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");
            });

            modelBuilder.Entity<Offers>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AcceptanceTimestamp).HasColumnType("datetime");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.OfferTimestamp).HasColumnType("datetime");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.OffersClient)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OffersT_ClientID_To_Users_ID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OffersT_JobID_To_Jobs_ID");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.OffersProvider)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OffersT_ProviderID_To_Users_ID");
            });

            modelBuilder.Entity<Payments>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ammount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.PayerUserId).HasColumnName("PayerUserID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.PaymentsClient)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentsT_ClientID_To_Users_ID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentsT_JobID_To_Jobs_ID");

                entity.HasOne(d => d.Provider)
                    .WithMany(p => p.PaymentsProvider)
                    .HasForeignKey(d => d.ProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PaymentsT_ProviderID_To_Users_ID");
            });

            modelBuilder.Entity<Skills>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<SMSVerifications>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ObjectId);

                entity.Property(e => e.ObjectIdType);

                entity.Property(e => e.TelephoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.SMSCode).HasMaxLength(4);

                entity.Property(e => e.SMSSentTime).HasColumnType("datetime");

                entity.Property(e => e.SMSVerified);

                entity.Property(e => e.SMSVerifiedTime).HasColumnType("datetime");
            });
            
            modelBuilder.Entity<UserClientDetails>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ContactTelephone1).HasMaxLength(20);

                entity.Property(e => e.ContactTelephone2).HasMaxLength(20);

                entity.Property(e => e.ContactEmail).HasMaxLength(50);

                entity.Property(e => e.ProfileImageUrl).HasMaxLength(200);

                entity.Property(e => e.Rating);

                entity.Property(e => e.MaxJobsAllowed);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClientDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserClientDetailsT_UserID_To_Users_ID");
            });

            modelBuilder.Entity<UserProviderDetails>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ContactTelephone1).HasMaxLength(20);

                entity.Property(e => e.ContactTelephone2).HasMaxLength(20);

                entity.Property(e => e.ContactEmail).HasMaxLength(50);

                entity.Property(e => e.ProfileImageUrl).HasMaxLength(200);

                entity.Property(e => e.HaveAcar);

                entity.Property(e => e.LocationLat).HasColumnType("decimal(10, 7)");

                entity.Property(e => e.LocationLng).HasColumnType("decimal(10, 7)");

                entity.Property(e => e.Searchable);

                entity.Property(e => e.Rating);

                entity.Property(e => e.Telephone1Verified).HasMaxLength(20);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserProviderDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProviderDetailsT_UserID_To_Users_ID");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddressId)
                    .HasColumnName("AddressID")
                    .HasMaxLength(50);

                entity.Property(e => e.LastLoginTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("binary(64)");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AvailableAmmount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BlockedAmmount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.LastChangeTime).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Wallet)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WalletT_UserID_To_Users_ID");
            });

            modelBuilder.Entity<WalletHistory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AvailableAmountBefore).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BlockedAmountBefore).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.AvailableAmountAfter).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BlockedAmountAfter).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.WalletId).HasColumnName("WalletID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WalletHistory)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WalletHistoryT_UserID_To_UsersT_ID");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.WalletHistory)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WalletHistoryT_WalletID_To_WalletT_ID");
            });
        }
    }
}
