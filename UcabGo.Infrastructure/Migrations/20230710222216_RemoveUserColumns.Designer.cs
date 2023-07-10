﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UcabGo.Infrastructure.Data;

#nullable disable

namespace UcabGo.Infrastructure.Migrations
{
    [DbContext(typeof(UcabgoContext))]
    [Migration("20230710222216_RemoveUserColumns")]
    partial class RemoveUserColumns
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("UcabGo.Core.Entities.Chatmessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Ride")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeSent")
                        .HasColumnType("datetime");

                    b.Property<int>("User")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Ride" }, "ChatRide");

                    b.HasIndex(new[] { "User" }, "ChatUser");

                    b.ToTable("chatmessage", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Evaluated")
                        .HasColumnType("int");

                    b.Property<DateTime>("EvaluationDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Evaluator")
                        .HasColumnType("int");

                    b.Property<int>("Ride")
                        .HasColumnType("int");

                    b.Property<int>("Stars")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)")
                        .HasDefaultValueSql("('')");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Evaluated" }, "Evaluated");

                    b.HasIndex(new[] { "Ride" }, "EvaluatedRide");

                    b.HasIndex(new[] { "Evaluator" }, "Evaluator");

                    b.ToTable("evaluation", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Detail")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("('0')");

                    b.Property<bool>("IsHome")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("('0')");

                    b.Property<float>("Latitude")
                        .HasColumnType("real");

                    b.Property<float>("Longitude")
                        .HasColumnType("real");

                    b.Property<int>("User")
                        .HasColumnType("int");

                    b.Property<string>("Zone")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "User" }, "User1");

                    b.ToTable("locations", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Passenger", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("FinalLocation")
                        .HasColumnType("int");

                    b.Property<float>("LatitudeOrigin")
                        .HasColumnType("real");

                    b.Property<float>("LongitudeOrigin")
                        .HasColumnType("real");

                    b.Property<int>("Ride")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeAccepted")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("TimeCancelled")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("TimeFinished")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("TimeIgnored")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("TimeSolicited")
                        .HasColumnType("datetime");

                    b.Property<int>("User")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "FinalLocation" }, "InitialLocation");

                    b.HasIndex(new[] { "User" }, "Passenger");

                    b.HasIndex(new[] { "Ride" }, "Ride");

                    b.ToTable("passenger", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Ride", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Driver")
                        .HasColumnType("int");

                    b.Property<int>("FinalLocation")
                        .HasColumnType("int");

                    b.Property<bool>("IsAvailable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("('0')");

                    b.Property<float>("LatitudeOrigin")
                        .HasColumnType("real");

                    b.Property<float>("LongitudeOrigin")
                        .HasColumnType("real");

                    b.Property<int>("SeatQuantity")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeCanceled")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("TimeCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("('0001-01-01 00:00:00')");

                    b.Property<DateTime?>("TimeEnded")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("TimeStarted")
                        .HasColumnType("datetime");

                    b.Property<int>("Vehicle")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "FinalLocation" }, "Destination");

                    b.HasIndex(new[] { "Driver" }, "Driver");

                    b.HasIndex(new[] { "Vehicle" }, "Vehicle");

                    b.ToTable("ride", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Soscontact", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("User")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "User" }, "IdUser");

                    b.ToTable("soscontact", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ProfilePicture")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<double?>("WalkingDistance")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("float")
                        .HasDefaultValueSql("('0')");

                    b.HasKey("Id");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Plate")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("User")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "User" }, "IdUser1");

                    b.ToTable("vehicle", (string)null);
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Chatmessage", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.Ride", "RideNavigation")
                        .WithMany("Chatmessages")
                        .HasForeignKey("Ride")
                        .IsRequired()
                        .HasConstraintName("chatmessage_ibfk_1");

                    b.HasOne("UcabGo.Core.Entities.User", "UserNavigation")
                        .WithMany("Chatmessages")
                        .HasForeignKey("User")
                        .IsRequired()
                        .HasConstraintName("chatmessage_ibfk_2");

                    b.Navigation("RideNavigation");

                    b.Navigation("UserNavigation");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Evaluation", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.User", "EvaluatedNavigation")
                        .WithMany("EvaluationEvaluatedNavigations")
                        .HasForeignKey("Evaluated")
                        .IsRequired()
                        .HasConstraintName("evaluation_ibfk_3");

                    b.HasOne("UcabGo.Core.Entities.User", "EvaluatorNavigation")
                        .WithMany("EvaluationEvaluatorNavigations")
                        .HasForeignKey("Evaluator")
                        .IsRequired()
                        .HasConstraintName("evaluation_ibfk_2");

                    b.HasOne("UcabGo.Core.Entities.Ride", "RideNavigation")
                        .WithMany("Evaluations")
                        .HasForeignKey("Ride")
                        .IsRequired()
                        .HasConstraintName("evaluation_ibfk_1");

                    b.Navigation("EvaluatedNavigation");

                    b.Navigation("EvaluatorNavigation");

                    b.Navigation("RideNavigation");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Location", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.User", "UserNavigation")
                        .WithMany("Locations")
                        .HasForeignKey("User")
                        .IsRequired()
                        .HasConstraintName("locations_ibfk_1");

                    b.Navigation("UserNavigation");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Passenger", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.Location", "FinalLocationNavigation")
                        .WithMany("Passengers")
                        .HasForeignKey("FinalLocation")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("passenger_ibfk_3");

                    b.HasOne("UcabGo.Core.Entities.Ride", "RideNavigation")
                        .WithMany("Passengers")
                        .HasForeignKey("Ride")
                        .IsRequired()
                        .HasConstraintName("passenger_ibfk_1");

                    b.HasOne("UcabGo.Core.Entities.User", "UserNavigation")
                        .WithMany("Passengers")
                        .HasForeignKey("User")
                        .IsRequired()
                        .HasConstraintName("passenger_ibfk_2");

                    b.Navigation("FinalLocationNavigation");

                    b.Navigation("RideNavigation");

                    b.Navigation("UserNavigation");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Ride", b =>
                {
                    b.HasOne("UcabGo.Core.Entities.User", "DriverNavigation")
                        .WithMany("Rides")
                        .HasForeignKey("Driver")
                        .IsRequired()
                        .HasConstraintName("ride_ibfk_1");

                    b.HasOne("UcabGo.Core.Entities.Location", "FinalLocationNavigation")
                        .WithMany("Rides")
                        .HasForeignKey("FinalLocation")
                        .IsRequired()
                        .HasConstraintName("ride_ibfk_3");

                    b.HasOne("UcabGo.Core.Entities.Vehicle", "VehicleNavigation")
                        .WithMany("Rides")
                        .HasForeignKey("Vehicle")
                        .IsRequired()
                        .HasConstraintName("ride_ibfk_2");

                    b.Navigation("DriverNavigation");

                    b.Navigation("FinalLocationNavigation");

                    b.Navigation("VehicleNavigation");
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

            modelBuilder.Entity("UcabGo.Core.Entities.Location", b =>
                {
                    b.Navigation("Passengers");

                    b.Navigation("Rides");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Ride", b =>
                {
                    b.Navigation("Chatmessages");

                    b.Navigation("Evaluations");

                    b.Navigation("Passengers");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.User", b =>
                {
                    b.Navigation("Chatmessages");

                    b.Navigation("EvaluationEvaluatedNavigations");

                    b.Navigation("EvaluationEvaluatorNavigations");

                    b.Navigation("Locations");

                    b.Navigation("Passengers");

                    b.Navigation("Rides");

                    b.Navigation("Soscontacts");

                    b.Navigation("Vehicles");
                });

            modelBuilder.Entity("UcabGo.Core.Entities.Vehicle", b =>
                {
                    b.Navigation("Rides");
                });
#pragma warning restore 612, 618
        }
    }
}
