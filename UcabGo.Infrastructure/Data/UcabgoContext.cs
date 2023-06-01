using Microsoft.EntityFrameworkCore;
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

        public virtual DbSet<Chatmessage> Chatmessages { get; set; } = null!;
        public virtual DbSet<Destination> Destinations { get; set; } = null!;
        public virtual DbSet<Evaluation> Evaluations { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Passenger> Passengers { get; set; } = null!;
        public virtual DbSet<Ride> Rides { get; set; } = null!;
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

            modelBuilder.Entity<Chatmessage>(entity =>
            {
                entity.ToTable("chatmessage");

                entity.HasIndex(e => e.Ride, "ChatRide");

                entity.HasIndex(e => e.User, "ChatUser");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Ride).HasColumnType("int(11)");

                entity.Property(e => e.TimeSent).HasColumnType("datetime");

                entity.Property(e => e.User).HasColumnType("int(11)");

                entity.HasOne(d => d.RideNavigation)
                    .WithMany(p => p.Chatmessages)
                    .HasForeignKey(d => d.Ride)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("chatmessage_ibfk_1");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Chatmessages)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("chatmessage_ibfk_2");
            });

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

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.ToTable("evaluation");

                entity.HasIndex(e => e.Evaluated, "Evaluated");

                entity.HasIndex(e => e.Ride, "EvaluatedRide");

                entity.HasIndex(e => e.Evaluator, "Evaluator");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Evaluated).HasColumnType("int(11)");

                entity.Property(e => e.EvaluationDate).HasColumnType("datetime");

                entity.Property(e => e.Evaluator).HasColumnType("int(11)");

                entity.Property(e => e.Ride).HasColumnType("int(11)");

                entity.Property(e => e.Stars).HasColumnType("int(11)");

                entity.Property(e => e.Type).HasMaxLength(5);

                entity.HasOne(d => d.EvaluatedNavigation)
                    .WithMany(p => p.EvaluationEvaluatedNavigations)
                    .HasForeignKey(d => d.Evaluated)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("evaluation_ibfk_3");

                entity.HasOne(d => d.EvaluatorNavigation)
                    .WithMany(p => p.EvaluationEvaluatorNavigations)
                    .HasForeignKey(d => d.Evaluator)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("evaluation_ibfk_2");

                entity.HasOne(d => d.RideNavigation)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(d => d.Ride)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("evaluation_ibfk_1");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.HasIndex(e => e.User, "User1");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Alias).HasMaxLength(255);

                entity.Property(e => e.Detail).HasMaxLength(255);

                entity.Property(e => e.IsHome)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.User).HasColumnType("int(11)");

                entity.Property(e => e.Zone).HasMaxLength(255);

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("locations_ibfk_1");
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.ToTable("passenger");

                entity.HasIndex(e => e.InitialLocation, "InitialLocation");

                entity.HasIndex(e => e.User, "Passenger");

                entity.HasIndex(e => e.Ride, "Ride");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.InitialLocation).HasColumnType("int(11)");

                entity.Property(e => e.Ride).HasColumnType("int(11)");

                entity.Property(e => e.TimeAccepted).HasColumnType("datetime");

                entity.Property(e => e.TimeCancelled).HasColumnType("datetime");

                entity.Property(e => e.TimeFinished).HasColumnType("datetime");

                entity.Property(e => e.TimeIgnored).HasColumnType("datetime");

                entity.Property(e => e.TimeSolicited).HasColumnType("datetime");

                entity.Property(e => e.User).HasColumnType("int(11)");

                entity.HasOne(d => d.InitialLocationNavigation)
                    .WithMany(p => p.Passengers)
                    .HasForeignKey(d => d.InitialLocation)
                    .HasConstraintName("passenger_ibfk_3");

                entity.HasOne(d => d.RideNavigation)
                    .WithMany(p => p.Passengers)
                    .HasForeignKey(d => d.Ride)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("passenger_ibfk_1");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Passengers)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("passenger_ibfk_2");
            });

            modelBuilder.Entity<Ride>(entity =>
            {
                entity.ToTable("ride");

                entity.HasIndex(e => e.Destination, "Destination");

                entity.HasIndex(e => e.Driver, "Driver");

                entity.HasIndex(e => e.Vehicle, "Vehicle");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Destination).HasColumnType("int(11)");

                entity.Property(e => e.Driver).HasColumnType("int(11)");

                entity.Property(e => e.IsAvailable)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.SeatQuantity).HasColumnType("int(11)");

                entity.Property(e => e.TimeCanceled).HasColumnType("datetime");

                entity.Property(e => e.TimeCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'0001-01-01 00:00:00'");

                entity.Property(e => e.TimeEnded).HasColumnType("datetime");

                entity.Property(e => e.TimeStarted).HasColumnType("datetime");

                entity.Property(e => e.Vehicle).HasColumnType("int(11)");

                entity.HasOne(d => d.DestinationNavigation)
                    .WithMany(p => p.Rides)
                    .HasForeignKey(d => d.Destination)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ride_ibfk_3");

                entity.HasOne(d => d.DriverNavigation)
                    .WithMany(p => p.Rides)
                    .HasForeignKey(d => d.Driver)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ride_ibfk_1");

                entity.HasOne(d => d.VehicleNavigation)
                    .WithMany(p => p.Rides)
                    .HasForeignKey(d => d.Vehicle)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ride_ibfk_2");
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

                entity.Property(e => e.WalkingDistance).HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("vehicle");

                entity.HasIndex(e => e.User, "IdUser1");

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
