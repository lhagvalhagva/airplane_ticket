using System;
using System.Text.Json.Serialization;

namespace BlazorClient.Models
{
    // Нислэгийн статусыг илтгэх enum
    public enum FlightStatus
    {
        CheckingIn = 0,
        Boarding = 1,
        Departed = 2,
        Delayed = 3,
        Cancelled = 4
    }        


    // API-с ирэх Flight төрөл
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FlightStatus Status { get; set; }
    }
}
