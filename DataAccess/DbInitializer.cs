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
                    PassengerId = passengers[0].Id,
                    RegistrationDate = DateTime.Now.AddHours(-5)
                },
                new FlightPassenger
                {
                    FlightId = flights[0].Id,
                    PassengerId = passengers[1].Id,
                    RegistrationDate = DateTime.Now.AddHours(-4)
                },
                new FlightPassenger
                {
                    FlightId = flights[1].Id,
                    PassengerId = passengers[2].Id,
                    RegistrationDate = DateTime.Now.AddHours(-3)
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

            // Суудлын заримыг захиалагдсан болгох
            var seat1A = allSeats.FirstOrDefault(s => s.FlightId == flights[0].Id && s.SeatNumber == "1A");
            var seat2B = allSeats.FirstOrDefault(s => s.FlightId == flights[0].Id && s.SeatNumber == "2B");
            var seat3C = allSeats.FirstOrDefault(s => s.FlightId == flights[1].Id && s.SeatNumber == "3C");

            if (seat1A != null && seat2B != null && seat3C != null)
            {
                seat1A.IsOccupied = true;
                seat2B.IsOccupied = true;
                seat3C.IsOccupied = true;

                await context.SaveChangesAsync();

                // Тасалбарууд үүсгэх
                var boardingPasses = new List<BoardingPass>
                {
                    new BoardingPass
                    {
                        FlightId = flights[0].Id,
                        PassengerId = passengers[0].Id,
                        SeatId = seat1A.Id,
                        CheckInTime = DateTime.Now.AddHours(-1)
                    },
                    new BoardingPass
                    {
                        FlightId = flights[0].Id,
                        PassengerId = passengers[1].Id,
                        SeatId = seat2B.Id,
                        CheckInTime = DateTime.Now.AddMinutes(-30)
                    },
                    new BoardingPass
                    {
                        FlightId = flights[1].Id,
                        PassengerId = passengers[2].Id,
                        SeatId = seat3C.Id,
                        CheckInTime = DateTime.Now.AddMinutes(-45)
                    }
                };

                await context.BoardingPasses.AddRangeAsync(boardingPasses);
                await context.SaveChangesAsync();
            }
        }
    }
} 