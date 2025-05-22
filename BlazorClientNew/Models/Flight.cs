using System;
using System.Text.Json.Serialization;

namespace BlazorClientNew.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public FlightStatus Status { get; set; }
        public int AvailableSeats { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FlightStatus
    {
        CheckingIn,
        Boarding,
        Departed,
        Delayed,
        Cancelled
    }
} 