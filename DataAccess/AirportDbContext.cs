using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using System.IO;
using System.Reflection;

namespace DataAccess
{
    public class AirportDbContext : DbContext
    {
        // For dependency injection
        public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
        {
        }

        // For design-time and direct initialization
        public AirportDbContext()
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<FlightPassenger> FlightPassengers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Get the path to the RestApi project directory
                string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string restApiPath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "RestApi"));
                string dbPath = Path.Combine(restApiPath, "airport.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Flight configuration
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Seats)
                .WithOne(s => s.Flight)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // Passenger configuration
            modelBuilder.Entity<Passenger>()
                .HasMany(p => p.FlightPassengers)
                .WithOne(fp => fp.Passenger)
                .HasForeignKey(fp => fp.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seat configuration
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Passenger)
                .WithMany()
                .HasForeignKey(s => s.PassengerId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // FlightPassenger configuration
            modelBuilder.Entity<FlightPassenger>()
                .HasOne(fp => fp.Flight)
                .WithMany(f => f.FlightPassengers)
                .HasForeignKey(fp => fp.FlightId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 