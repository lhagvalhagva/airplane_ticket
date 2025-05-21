using System;
using System.Collections.Generic;

namespace AirplaneTicket.WPF.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PassportNumber { get; set; }
        public string Nationality { get; set; }
        public bool CheckedIn { get; set; }
        public string? SeatNumber { get; set; }
        public List<FlightPassenger> FlightPassengers { get; set; } = new List<FlightPassenger>();
    }

    public class FlightPassenger
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Flight Flight { get; set; }
        public Passenger Passenger { get; set; }
    }
} 