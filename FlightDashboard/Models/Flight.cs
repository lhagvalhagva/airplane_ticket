using System;

namespace FlightDashboard.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public FlightStatus Status { get; set; }
        public int AvailableSeats { get; set; }
    }

    public enum FlightStatus
    {
        Scheduled = 0,
        Boarding = 1,
        Departed = 2,
        Landed = 3,
        Cancelled = 4
    }
}
