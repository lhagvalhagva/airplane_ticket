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
                // Өгөгдлийн сан аль хэдийн эхлүүлсэн байна
                return;
            }

            var random = new Random();

            // Нислэгүүд үүсгэх
            var flights = new List<Flight>
            {
                new Flight
                {
                    FlightNumber = "MN101",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Beijing",
                    DepartureTime = DateTime.Now.AddDays(1),
                    ArrivalTime = DateTime.Now.AddDays(1).AddHours(2),
                    AvailableSeats = 30,
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN102",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Tokyo",
                    DepartureTime = DateTime.Now.AddDays(2),
                    ArrivalTime = DateTime.Now.AddDays(2).AddHours(3),
                    AvailableSeats = 30,
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN103",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Seoul",
                    DepartureTime = DateTime.Now.AddDays(3),
                    ArrivalTime = DateTime.Now.AddDays(3).AddHours(2),
                    AvailableSeats = 30,
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN104",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Moscow",
                    DepartureTime = DateTime.Now.AddDays(4),
                    ArrivalTime = DateTime.Now.AddDays(4).AddHours(6),
                    AvailableSeats = 30,
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN105",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Berlin",
                    DepartureTime = DateTime.Now.AddDays(5),
                    ArrivalTime = DateTime.Now.AddDays(5).AddHours(8),
                    AvailableSeats = 30,
                    Status = FlightStatus.CheckingIn
                },
                new Flight
                {
                    FlightNumber = "MN106",
                    DepartureCity = "Ulaanbaatar",
                    ArrivalCity = "Singapore",
                    DepartureTime = DateTime.Now.AddDays(6),
                    ArrivalTime = DateTime.Now.AddDays(6).AddHours(7),
                    AvailableSeats = 30,
                    Status = FlightStatus.CheckingIn
                }
            };

            await context.Flights.AddRangeAsync(flights);
            await context.SaveChangesAsync();

            // Зорчигчид үүсгэх
            var passengers = new List<Passenger>
            {
                new Passenger
                {
                    FirstName = "Bat",
                    LastName = "Dorj",
                    PassportNumber = "MN1234",
                    Nationality = "Mongolian",
                    Email = "bat.dorj@gmail.com",
                    PhoneNumber = "99112233",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Bold",
                    LastName = "Suren",
                    PassportNumber = "MN2345",
                    Nationality = "Mongolian",
                    Email = "bold.suren@gmail.com",
                    PhoneNumber = "99223344",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Tsetseg",
                    LastName = "Bat",
                    PassportNumber = "MN3456",
                    Nationality = "Mongolian",
                    Email = "tsetseg.bat@gmail.com",
                    PhoneNumber = "99334455",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Oyun",
                    LastName = "Dulam",
                    PassportNumber = "MN4567",
                    Nationality = "Mongolian",
                    Email = "oyun.dulam@gmail.com",
                    PhoneNumber = "99445566",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Saraa",
                    LastName = "Erdene",
                    PassportNumber = "MN5678",
                    Nationality = "Mongolian",
                    Email = "saraa.erdene@gmail.com",
                    PhoneNumber = "99556677",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Tuya",
                    LastName = "Gantulga",
                    PassportNumber = "MN6789",
                    Nationality = "Mongolian",
                    Email = "tuya.gantulga@gmail.com",
                    PhoneNumber = "99667788",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Altai",
                    LastName = "Binderya",
                    PassportNumber = "MN7890",
                    Nationality = "Mongolian",
                    Email = "altai.binderya@gmail.com",
                    PhoneNumber = "99778899",
                    CheckedIn = false
                },
                new Passenger
                {
                    FirstName = "Zaya",
                    LastName = "Tumur",
                    PassportNumber = "MN8901",
                    Nationality = "Mongolian",
                    Email = "zaya.tumur@gmail.com",
                    PhoneNumber = "99889900",
                    CheckedIn = false
                }
            };

            await context.Passengers.AddRangeAsync(passengers);
            await context.SaveChangesAsync();

            // Зорчигчдыг нислэгүүдэд хуваарилах
            var flightPassengers = new List<FlightPassenger>();
            foreach (var flight in flights)
            {
                // Хэрэв FlightPassengers-ийн ИД заасан байвал (алдааны эх үүсвэр) засах
                // Дараах кодуудаас ИД хасах шаардлагатай
                flightPassengers.Add(new FlightPassenger { FlightId = flight.Id, PassengerId = passengers[0].Id, RegistrationDate = DateTime.Now });
                flightPassengers.Add(new FlightPassenger { FlightId = flight.Id, PassengerId = passengers[1].Id, RegistrationDate = DateTime.Now });
                flightPassengers.Add(new FlightPassenger { FlightId = flight.Id, PassengerId = passengers[2].Id, RegistrationDate = DateTime.Now });
                flightPassengers.Add(new FlightPassenger { FlightId = flight.Id, PassengerId = passengers[3].Id, RegistrationDate = DateTime.Now });
                
                // Random зорчигчид нэмэх
                var remainingPassengers = passengers.Skip(4).OrderBy(p => random.Next()).Take(4).ToList();
                foreach (var passenger in remainingPassengers)
                {
                    // Энд бас ИД заасан бол хасах
                    flightPassengers.Add(new FlightPassenger { FlightId = flight.Id, PassengerId = passenger.Id, RegistrationDate = DateTime.Now });
                }
            }

            await context.FlightPassengers.AddRangeAsync(flightPassengers);
            await context.SaveChangesAsync();

            // Суудлууд үүсгэх
            foreach (var flight in flights)
            {
                var flightSeats = new List<Seat>();
                for (int i = 1; i <= 30; i++)
                {
                    flightSeats.Add(new Seat
                    {
                        FlightId = flight.Id,
                        SeatNumber = $"{i}A",
                        IsOccupied = false
                    });
                }

                await context.Seats.AddRangeAsync(flightSeats);
                await context.SaveChangesAsync();

                // Зорчигчдын ойролцоогоор тал хувьд суудал оноох
                var assignedPassengers = flightPassengers.Where(fp => fp.FlightId == flight.Id).OrderBy(x => random.Next()).Take(4).ToList();
                var availableSeats = flightSeats.ToList();
                for (int i = 0; i < assignedPassengers.Count; i++)
                {
                    var seat = availableSeats[i];
                    var passenger = assignedPassengers[i];
                    seat.IsOccupied = true;
                    seat.PassengerId = passenger.PassengerId;
                    seat.CheckInTime = DateTime.Now.AddMinutes(-random.Next(1, 120));

                    // Зорчигчийн мэдээлэл шинэчлэх
                    var passengerEntity = passengers.FirstOrDefault(p => p.Id == passenger.PassengerId);
                    if (passengerEntity != null)
                    {
                        passengerEntity.CheckedIn = true;
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}