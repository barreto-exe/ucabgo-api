﻿using Microsoft.EntityFrameworkCore;
using UcabGo.Core.Entities;

namespace UcabGo.Infrastructure.Data
{
    public partial class UcabgoContext : DbContext
    {
        public UcabgoContext()
        {
        }

        public UcabgoContext(DbContextOptions<UcabgoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Destination> Destinations { get; set; } = null!;
        public virtual DbSet<Soscontact> Soscontacts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Vehicle> Vehicles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Destination>(entity =>
            {
                entity.ToTable("destinations");

                entity.HasIndex(e => e.User, "User");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasComment(" ");

                entity.Property(e => e.Alias).HasMaxLength(255);

                entity.Property(e => e.Detail).HasMaxLength(255);

                entity.Property(e => e.User).HasColumnType("int(11)");

                entity.Property(e => e.Zone).HasMaxLength(255);

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Destinations)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("destinations_ibfk_1");
            });

            modelBuilder.Entity<Soscontact>(entity =>
            {
                entity.ToTable("soscontact");

                entity.HasIndex(e => e.User, "IdUser");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.User).HasColumnType("int(11)");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Soscontacts)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("soscontact_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.SecondLastName).HasMaxLength(255);

                entity.Property(e => e.SecondName).HasMaxLength(255);
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicle");

                entity.HasIndex(e => e.User, "IdUser");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Brand).HasMaxLength(100);

                entity.Property(e => e.Color).HasMaxLength(20);

                entity.Property(e => e.Model).HasMaxLength(100);

                entity.Property(e => e.Plate).HasMaxLength(20);

                entity.Property(e => e.User).HasColumnType("int(11)");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehicle_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
