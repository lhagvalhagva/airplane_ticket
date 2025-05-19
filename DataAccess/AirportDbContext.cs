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
        public DbSet<BoardingPass> BoardingPasses { get; set; }
        public DbSet<FlightPassenger> FlightPassengers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // DataAccess хавтас руу заах замыг гаргах
                string dataAccessPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string dbPath = Path.Combine(dataAccessPath, "airport.db");
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
                .HasMany(p => p.BoardingPasses)
                .WithOne(bp => bp.Passenger)
                .HasForeignKey(bp => bp.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seat configuration
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.BoardingPass)
                .WithOne(bp => bp.Seat)
                .HasForeignKey<BoardingPass>(bp => bp.SeatId)
                .OnDelete(DeleteBehavior.SetNull);

            // BoardingPass configuration
            modelBuilder.Entity<BoardingPass>()
                .HasOne(bp => bp.Flight)
                .WithMany()
                .HasForeignKey(bp => bp.FlightId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 