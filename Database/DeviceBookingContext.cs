using Microsoft.EntityFrameworkCore;

using Database.Models;
using Type = Database.Models.Type;

namespace Database
{
    public partial class DeviceBookingContext : DbContext
    {
        public DeviceBookingContext(DbContextOptions<DeviceBookingContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Device> Devices { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Os> Os { get; set; } = null!;
        public virtual DbSet<Record> Records { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<TagInfo> TagInfos { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<ImageInfo> ImageInfos { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Info).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("device");

                entity.HasIndex(e => e.DepartmentId);

                entity.HasIndex(e => e.ImageId);

                entity.HasIndex(e => e.QrId);

                entity.HasIndex(e => e.OsId);

                entity.HasIndex(e => e.TypeId);

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Class).HasMaxLength(255);

                entity.Property(e => e.DepartmentId).HasColumnType("bigint");

                entity.Property(e => e.ImageId).HasColumnType("bigint");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.OsId).HasColumnType("bigint");

                entity.Property(e => e.Resolution).HasMaxLength(255);

                entity.Property(e => e.TypeId).HasColumnType("bigint");

                entity.Property(e => e.QrId).HasColumnType("bigint");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.DepartmentId);

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.ImageId);

                entity.HasOne(d => d.Qr)
                    .WithMany(p => p.Qrs)
                    .HasForeignKey(d => d.QrId);

                entity.HasOne(d => d.Os)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.OsId);

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.TypeId);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("image");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Path).HasMaxLength(255);
            });

            modelBuilder.Entity<Os>(entity =>
            {
                entity.ToTable("os");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.ToTable("record");

                entity.HasIndex(e => e.DepartmentId);

                entity.HasIndex(e => e.DeviceId);

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TimeFrom).HasColumnType("time");

                entity.Property(e => e.TimeTo).HasColumnType("time");

                entity.Property(e => e.DepartmentId).HasColumnType("bigint");

                entity.Property(e => e.DeviceId).HasColumnType("bigint");

                entity.Property(e => e.UserId).HasColumnType("bigint");

                entity.Property(e => e.Booked).HasColumnType("tinyint(1)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.DepartmentId);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<TagInfo>(entity =>
            {
                entity.ToTable("tag_info");

                entity.HasIndex(e => e.DeviceId);

                entity.HasIndex(e => e.TagId);

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DeviceId).HasColumnType("bigint");

                entity.Property(e => e.TagId).HasColumnType("bigint");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.TagInfos)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TagInfos)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("type");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.DepartmentId);

                entity.HasIndex(e => e.ImageId);

                entity.Property(e => e.Id)
                    .HasColumnType("bigint")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Firstname).HasMaxLength(255);

                entity.Property(e => e.Secondname).HasMaxLength(255);

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.IsBlocked).HasColumnType("tinyint(1)");

                entity.Property(e => e.ConnectLink).HasMaxLength(255);

                entity.Property(e => e.ImageId).HasColumnType("bigint");

                entity.Property(e => e.DepartmentId).HasColumnType("bigint");

                entity.HasOne(e => e.Image)
                      .WithMany(e => e.Users)
                      .HasForeignKey(d => d.ImageId);

                entity.HasOne(e => e.Department)
                      .WithMany(e => e.Users)
                      .HasForeignKey(e => e.DepartmentId);

                entity.HasAlternateKey(e => e.Username);

                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ImageInfo>(entity =>
            {
                entity.ToTable("image_info");

                entity.HasIndex(e => e.ImageId);

                entity.HasIndex(e => e.ReportId);

                entity.Property(e => e.Id).HasColumnType("bigint");

                entity.Property(e => e.ImageId).HasColumnType("bigint");

                entity.Property(e => e.ReportId).HasColumnType("bigint");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ImageInfos)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ImageInfos)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("report");

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).HasColumnType("bigint");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnType("bigint");

                entity.HasOne(e => e.User)
                     .WithMany(e => e.Reports)
                     .HasForeignKey(e => e.UserId)
                     .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.Reviewer)
                     .WithMany(e => e.ConsideredReports)
                     .HasForeignKey(e => e.ReviewerId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
