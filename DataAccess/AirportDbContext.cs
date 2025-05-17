using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess
{
    public class AirportDbContext : DbContext
    {
        public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<BoardingPass> BoardingPasses { get; set; }
        public DbSet<FlightPassenger> FlightPassengers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FlightPassenger>()
                .HasOne(fp => fp.Flight)
                .WithMany()
                .HasForeignKey(fp => fp.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlightPassenger>()
                .HasOne(fp => fp.Passenger)
                .WithMany()
                .HasForeignKey(fp => fp.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.BoardingPass)
                .WithOne(bp => bp.Seat)
                .HasForeignKey<BoardingPass>(bp => bp.SeatId);
        }
    }
} 