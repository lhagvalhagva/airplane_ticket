using System;

namespace AirplaneTicket.WPF.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
        public int FlightId { get; set; }
        public int? PassengerId { get; set; }
        public bool IsOccupied { get; set; }
        public DateTime? BookingTime { get; set; }
        public string Status { get; set; } // Available, Occupied, Reserved
    }
} 