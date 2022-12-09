using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace booking.Entities
{
    public partial class DeviceBookingContext : DbContext
    {
        public DeviceBookingContext()
        {
            Database.EnsureCreated();
        }

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
        public virtual DbSet<ReportImage> ReportImages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_unicode_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("department");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Info).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Device>(entity =>
            {
                entity.ToTable("device");

                entity.HasIndex(e => e.DepartmentId, "DepartmentId");

                entity.HasIndex(e => e.ImgId, "ImgId");

                entity.HasIndex(e => e.OsId, "OsId");

                entity.HasIndex(e => e.TypeId, "TypeId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Class).HasMaxLength(255);

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.ImgId).HasColumnType("int(11)");

                entity.Property(e => e.Info).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.OsId).HasColumnType("int(11)");

                entity.Property(e => e.Resolution).HasMaxLength(255);

                entity.Property(e => e.TypeId).HasColumnType("int(11)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("device_ibfk_5");

                entity.HasOne(d => d.Img)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.ImgId)
                    .HasConstraintName("device_ibfk_6");

                entity.HasOne(d => d.Os)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.OsId)
                    .HasConstraintName("device_ibfk_4");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("device_ibfk_3");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("image");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Path).HasMaxLength(255);
            });

            modelBuilder.Entity<Os>(entity =>
            {
                entity.ToTable("os");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Record>(entity =>
            {
                entity.ToTable("record");

                entity.HasIndex(e => e.DepartmentId, "DepartmentId");

                entity.HasIndex(e => e.DeviceId, "DeviceId");

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TimeFrom).HasColumnType("time");

                entity.Property(e => e.TimeTo).HasColumnType("time");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.DeviceId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.Property(e => e.Booked).HasColumnType("tinyint(1)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("record_ibfk_3");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("record_ibfk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Records)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("record_ibfk_2");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<TagInfo>(entity =>
            {
                entity.ToTable("tagInfo");

                entity.HasIndex(e => e.DeviceId, "DeviceId");

                entity.HasIndex(e => e.TagId, "TagId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.DeviceId).HasColumnType("int(11)");

                entity.Property(e => e.TagId).HasColumnType("int(11)");

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.TagInfos)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("taginfo_ibfk_1");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TagInfos)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("taginfo_ibfk_2");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("type");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.DepartmentId, "DepartmentId");

                entity.HasIndex(e => e.ImgId, "ImgId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();      

                entity.Property(e => e.Firstname).HasMaxLength(255);

                entity.Property(e => e.Secondname).HasMaxLength(255);

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.ConnectLink).HasMaxLength(255);

                entity.Property(e => e.ImgId).HasColumnType("int(11)");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.HasOne(e => e.Img)
                      .WithMany(p => p.Users)
                      .HasForeignKey(d => d.ImgId)
                      .HasConstraintName("user_ibfk_1");


                entity.HasOne(e => e.Department)
                      .WithMany(p => p.Users)
                      .HasForeignKey(d => d.DepartmentId)
                      .HasConstraintName("user_ibfk_2");

                entity.HasAlternateKey(e => e.Username);

                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ImageInfo>(entity =>
            {
                entity.ToTable("imageInfo");

                entity.HasIndex(e => e.ImageId, "ImageId");

                entity.HasIndex(e => e.ReportId, "ReportId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ImageId).HasColumnType("int(11)");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.ImageInfos)
                    .HasForeignKey(d => d.ImageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("imageinfo_ibfk_2");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.ImageInfos)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("imageinfo_ibfk_1");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("report");

                entity.HasIndex(e => e.UserId, "UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                     .WithMany(p => p.Reports)
                     .HasForeignKey(d => d.UserId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("report_ibfk_1");
            });

            modelBuilder.Entity<ReportImage>(entity =>
            {
                entity.ToTable("reportImage");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Filename).HasMaxLength(255);

                entity.Property(e => e.Path)
                    .HasMaxLength(255)
                    .HasColumnName("path");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
