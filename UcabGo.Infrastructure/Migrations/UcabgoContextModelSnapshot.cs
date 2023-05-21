﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UcabGo.Infrastructure.Data;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    [DbContext(typeof(UcabgoContext))]
    partial class UcabgoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("latin1_swedish_ci")
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "latin1");

            modelBuilder.Entity("UcabGo.Core.Entities.Soscontact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<int>("User")
                        .HasColumnType("int(11)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "User" }, "IdUser");

                    b.ToTable("soscontact", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("SecondLastName")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("SecondName")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Plate")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<int>("User")
                        .HasColumnType("int(11)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "User" }, "IdUser")
                        .HasDatabaseName("IdUser1");

                    b.ToTable("vehicle", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Soscontact", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.User", "UserNavigation")
                        .WithMany("Soscontacts")
                        .HasForeignKey("User")
                        .IsRequired()
                        .HasConstraintName("soscontact_ibfk_1");

                    b.Navigation("UserNavigation");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Vehicle", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.User", "UserNavigation")
                        .WithMany("Vehicles")
                        .HasForeignKey("User")
                        .IsRequired()
                        .HasConstraintName("vehicle_ibfk_1");

                    b.Navigation("UserNavigation");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.User", b =>
                {
                    b.Navigation("Soscontacts");

                    b.Navigation("Vehicles");
                });
#pragma warning restore 612, 618
        }
    }
}
