using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UcabGo.Core.Entities;

namespace UcabGo.Infrastructure.Data;

public partial class UcabgoContext : DbContext
{
    public UcabgoContext()
    {
    }

    public UcabgoContext(DbContextOptions<UcabgoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.SecondLastName).HasMaxLength(255);
            entity.Property(e => e.SecondName).HasMaxLength(255);
        });
    }
}
