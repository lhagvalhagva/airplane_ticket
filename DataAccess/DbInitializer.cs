using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AirportDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Flights.AnyAsync())
            {
                return;
            }

            var flights = new List<Flight>
            {
                new Flight
                {
                    FlightNumber = "MN101",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Beijing",
                    DepartureTime = DateTime.Now.AddHours(2),
                    ArrivalTime = DateTime.Now.AddHours(4),
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN202",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Seoul",
                    DepartureTime = DateTime.Now.AddHours(3),
                    ArrivalTime = DateTime.Now.AddHours(6),
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN303",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Tokyo",
                    DepartureTime = DateTime.Now.AddHours(5),
                    ArrivalTime = DateTime.Now.AddHours(9),
                    Status = FlightStatus.CheckingIn
                }
            };

            await context.Flights.AddRangeAsync(flights);
            await context.SaveChangesAsync();

            var passengers = new List<Passenger>
            {
                new Passenger
                {
                    FirstName = "Bat",
                    LastName = "Bold",
                    PassportNumber = "AA123456",
                    Email = "batbold@example.com",
                    PhoneNumber = "+97699112233"
                },
                new Passenger
                {
                    FirstName = "Tsetseg",
                    LastName = "Mungun",
                    PassportNumber = "BB654321",
                    Email = "tsetseg@example.com",
                    PhoneNumber = "+97688223344"
                },
                new Passenger
                {
                    FirstName = "Suren",
                    LastName = "Bayar",
                    PassportNumber = "CC987654",
                    Email = "suren@example.com",
                    PhoneNumber = "+97677334455"
                }
            };

            await context.Passengers.AddRangeAsync(passengers);
            await context.SaveChangesAsync();

            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger
                {
                    FlightId = flights[0].Id,
                    PassengerId = passengers[0].Id
                },
                new FlightPassenger
                {
                    FlightId = flights[0].Id,
                    PassengerId = passengers[1].Id
                },
                new FlightPassenger
                {
                    FlightId = flights[1].Id,
                    PassengerId = passengers[2].Id
                }
            };

            await context.FlightPassengers.AddRangeAsync(flightPassengers);
            await context.SaveChangesAsync();

            var allSeats = new List<Seat>();
            foreach (var flight in flights)
            {
                for (int row = 1; row <= 6; row++)
                {
                    foreach (var col in new[] { "A", "B", "C", "D", "E" })
                    {
                        allSeats.Add(new Seat
                        {
                            FlightId = flight.Id,
                            SeatNumber = $"{row}{col}",
                            IsOccupied = false
                        });
                    }
                }
            }

            await context.Seats.AddRangeAsync(allSeats);
            await context.SaveChangesAsync();
        }
    }
} 