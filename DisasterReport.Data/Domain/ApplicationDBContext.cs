using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Domain;

public partial class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlacklistEntry> BlacklistEntries { get; set; }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DisasterTopic> DisasterTopics { get; set; }

    public virtual DbSet<DisastersReport> DisastersReports { get; set; }

    public virtual DbSet<DonateRequest> DonateRequests { get; set; }

    public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<ExternalLogin> ExternalLogins { get; set; }

    public virtual DbSet<ImpactType> ImpactTypes { get; set; }

    public virtual DbSet<ImpactUrl> ImpactUrls { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OrganizationDoc> OrganizationDocs { get; set; }

    public virtual DbSet<OrganizationMember> OrganizationMembers { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<SupportType> SupportTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public DbSet<DisasterReportMapDto> DisasterReportMapDtos { get; set; }// for sp

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlacklistEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blacklis__3214EC077E8C81EA");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.BlacklistEntries)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blacklist__UserI__5441852A");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Certific__3214EC0701DE3C0F");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonarName).HasMaxLength(150);
            entity.Property(e => e.SupportType).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Donation).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.DonationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__Donat__797309D9");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comments__3214EC07396C243E");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DisasterReport).WithMany(p => p.Comments)
                .HasForeignKey(d => d.DisasterReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__Disast__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__UserId__6C190EBB");
        });

        modelBuilder.Entity<DisasterTopic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Disaster__3214EC07817D8BBF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TopicName).HasMaxLength(225);
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Admin).WithMany(p => p.DisasterTopics)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__DisasterT__Admin__5AEE82B9");
        });

        modelBuilder.Entity<DisastersReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Disaster__3214EC0739C4D4CB");

            entity.Property(e => e.Category).HasMaxLength(225);
            entity.Property(e => e.ReportedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(225);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DisasterTopics).WithMany(p => p.DisastersReports)
                .HasForeignKey(d => d.DisasterTopicsId)
                .HasConstraintName("FK__DisasterR__Disas__6477ECF3");

            entity.HasOne(d => d.Location).WithMany(p => p.DisastersReports)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DisasterR__Locat__6383C8BA");

            entity.HasOne(d => d.Reporter).WithMany(p => p.DisastersReports)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DisasterR__Repor__628FA481");

            entity.HasMany(d => d.ImpactTypes).WithMany(p => p.DisasterReports)
                .UsingEntity<Dictionary<string, object>>(
                    "DisasterReportImpactType",
                    r => r.HasOne<ImpactType>().WithMany()
                        .HasForeignKey("ImpactTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DisasterR__Impac__01142BA1"),
                    l => l.HasOne<DisastersReport>().WithMany()
                        .HasForeignKey("DisasterReportId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DisasterR__Disas__00200768"),
                    j =>
                    {
                        j.HasKey("DisasterReportId", "ImpactTypeId").HasName("PK__Disaster__84CF7BB14AA6A659");
                        j.ToTable("DisasterReportImpactTypes");
                    });

            entity.HasMany(d => d.SupportTypes).WithMany(p => p.DisasterReports)
                .UsingEntity<Dictionary<string, object>>(
                    "DisasterReportSupportType",
                    r => r.HasOne<SupportType>().WithMany()
                        .HasForeignKey("SupportTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DisasterR__Suppo__04E4BC85"),
                    l => l.HasOne<DisastersReport>().WithMany()
                        .HasForeignKey("DisasterReportId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__DisasterR__Disas__03F0984C"),
                    j =>
                    {
                        j.HasKey("DisasterReportId", "SupportTypeId").HasName("PK__Disaster__E4D8CC9EDD29D163");
                        j.ToTable("DisasterReportSupportTypes");
                    });
        });

        modelBuilder.Entity<DonateRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DonateRe__3214EC07FAB37244");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DonatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileSizeKb).HasColumnName("FileSizeKB");
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.PaymentSlipUrl).HasMaxLength(500);
            entity.Property(e => e.SupportType).HasMaxLength(100);

            entity.HasOne(d => d.DisasterReport).WithMany(p => p.DonateRequests)
                .HasForeignKey(d => d.DisasterReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonateReq__Disas__72C60C4A");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.DonateRequests)
                .HasForeignKey(d => d.RequestedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DonateReq__Reque__71D1E811");
        });

        modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Donation__3214EC078B399ACF");

            entity.Property(e => e.DonatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DonateRequest).WithMany(p => p.Donations)
                .HasForeignKey(d => d.DonateRequestId)
                .HasConstraintName("FK__Donations__Donat__76969D2E");
        });

        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__External__3214EC07063359AA");

            entity.HasIndex(e => e.ProviderKey, "UQ__External__8DE43C5F97D2EDE5").IsUnique();

            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.ProviderKey).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.ExternalLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExternalL__UserI__4F7CD00D");
        });

        modelBuilder.Entity<ImpactType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImpactTy__3214EC0791F10505");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ImpactUrl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImpactUr__3214EC070295DCD3");

            entity.Property(e => e.FileSizeKb).HasColumnName("FileSizeKB");
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.PublicId).HasMaxLength(255);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.DisasterReport).WithMany(p => p.ImpactUrls)
                .HasForeignKey(d => d.DisasterReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImpactUrl__Disas__68487DD7");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3214EC072D3A1BA3");

            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.RegionName).HasMaxLength(225);
            entity.Property(e => e.TownshipName).HasMaxLength(225);
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Organiza__3214EC07A956E791");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.OrganizationEmail).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.Organizations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__Organizat__Appro__4316F928");
        });

        modelBuilder.Entity<OrganizationDoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Organiza__3214EC07D9BB7D76");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationDocs)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Organizat__Organ__46E78A0C");
        });

        modelBuilder.Entity<OrganizationMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Organiza__3214EC07D3ADA759");

            entity.HasIndex(e => e.UserId, "UQ_OrganizationMember_UserId").IsUnique();

            entity.Property(e => e.InvitedEmail).HasMaxLength(255);
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleInOrg).HasMaxLength(50);

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationMembers)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Organizat__Organ__4AB81AF0");

            entity.HasOne(d => d.User).WithOne(p => p.OrganizationMember)
                .HasForeignKey<OrganizationMember>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Organizat__UserI__4BAC3F29");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC0764F6CF1C");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.ReplacedByToken).HasMaxLength(256);
            entity.Property(e => e.RevokedAt).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(256);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RefreshTo__UserI__19DFD96B");
        });

        modelBuilder.Entity<SupportType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SupportT__3214EC0790B3A1E7");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07B33BDA21");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053431E8732D").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleId__3E52440B");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07D6EEA9D4");

            entity.HasIndex(e => e.RoleName, "UQ__UserRole__8A2B6160810FEF00").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<DisasterReportMapDto>()
       .HasNoKey()
       .ToView(null); // Important: EF should not treat it as a table/view

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
