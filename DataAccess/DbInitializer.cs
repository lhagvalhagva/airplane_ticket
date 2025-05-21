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
        /// <summary>
        /// Өгөгдлийн санг шууд үүсгэж, анхны өгөгдлүүдийг оруулна
        /// </summary>
        public static async Task CreateAndInitializeDatabase()
        {
            using (var context = new AirportDbContext())
            {
                await InitializeAsync(context);
            }
        }

        public static async Task InitializeAsync(AirportDbContext context)
        {
            // Одоогийн өгөгдлийн санг устгаж, шинээр үүсгэх
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            
            // Өгөгдөл байгаа эсэхийг шалгах шаардлагагүй болсон
            // Мэдээллийн санг устгасан тул одоо хоосон

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
                },
                new Flight
                {
                    FlightNumber = "MN404",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Moscow",
                    DepartureTime = DateTime.Now.AddHours(4),
                    ArrivalTime = DateTime.Now.AddHours(7),
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN505",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Berlin",
                    DepartureTime = DateTime.Now.AddHours(6),
                    ArrivalTime = DateTime.Now.AddHours(12),
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN606",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Istanbul",
                    DepartureTime = DateTime.Now.AddHours(7),
                    ArrivalTime = DateTime.Now.AddHours(11),
                    Status = FlightStatus.CheckingIn
                }
            };

            await context.Flights.AddRangeAsync(flights);
            await context.SaveChangesAsync();

            // Register 8 passengers per flight, assign seats to about half
            var random = new Random();
            var allSeats = new List<Seat>();
            var allFlightPassengers = new List<FlightPassenger>();
            int passengerCounter = 1;
            foreach (var flight in flights)
            {
                var flightPassengers = new List<Passenger>();
                for (int i = 0; i < 8; i++)
                {
                    var passenger = new Passenger
                    {
                        FirstName = $"TestFirst{passengerCounter}",
                        LastName = $"TestLast{passengerCounter}",
                        PassportNumber = $"P{flight.Id}{i:0000}",
                        Email = $"test{passengerCounter}@example.com",
                        PhoneNumber = $"+976{random.Next(10000000,99999999)}"
                    };
                    flightPassengers.Add(passenger);
                    passengerCounter++;
                }
                await context.Passengers.AddRangeAsync(flightPassengers);
                await context.SaveChangesAsync();

                // Register all passengers to the flight
                foreach (var p in flightPassengers)
                {
                    allFlightPassengers.Add(new FlightPassenger
                    {
                        FlightId = flight.Id,
                        PassengerId = p.Id,
                        RegistrationDate = DateTime.Now.AddMinutes(-random.Next(10, 300))
                    });
                }
                await context.FlightPassengers.AddRangeAsync(allFlightPassengers);
                await context.SaveChangesAsync();

                // Create seats
                var flightSeats = new List<Seat>();
                for (int row = 1; row <= 6; row++)
                {
                    foreach (var col in new[] { "A", "B", "C", "D", "E" })
                    {
                        flightSeats.Add(new Seat
                        {
                            FlightId = flight.Id,
                            SeatNumber = $"{row}{col}",
                            IsOccupied = false
                        });
                    }
                }
                await context.Seats.AddRangeAsync(flightSeats);
                await context.SaveChangesAsync();

                // Assign seats to about half the passengers
                var assignedPassengers = flightPassengers.OrderBy(x => random.Next()).Take(4).ToList();
                var availableSeats = flightSeats.ToList();
                for (int i = 0; i < assignedPassengers.Count; i++)
                {
                    var seat = availableSeats[i];
                    var passenger = assignedPassengers[i];
                    seat.IsOccupied = true;
                    seat.PassengerId = passenger.Id;
                    seat.CheckInTime = DateTime.Now.AddMinutes(-random.Next(1, 120));

                    // Update passenger info to reflect seat assignment
                    passenger.CheckedIn = true;
                }
                await context.SaveChangesAsync();
            }
        }
    }
} 