using System;

namespace AirplaneTicket.WPF.Models
{
    public class BoardingPass
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public string SeatNumber { get; set; }
        public DateTime BoardingTime { get; set; }
        public string Gate { get; set; }
        public string Status { get; set; }
        public Flight Flight { get; set; }
        public Passenger Passenger { get; set; }
    }
} 