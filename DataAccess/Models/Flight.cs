using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccess.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string FlightNumber { get; set; } = string.Empty;
        
        [Required]
        public string DepartureCity { get; set; } = string.Empty;
        
        [Required]
        public string ArrivalCity { get; set; } = string.Empty;
        
        [Required]
        public DateTime DepartureTime { get; set; }
        
        [Required]
        public DateTime ArrivalTime { get; set; }
        
        [Required]
        public FlightStatus Status { get; set; } = FlightStatus.CheckingIn;

        [Required]
        public int AvailableSeats { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<FlightPassenger> FlightPassengers { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }

    public enum FlightStatus
    {
        CheckingIn,      // Бүртгэж байна
        Boarding,        // Онгоцонд сууж байна
        Departed,        // Ниссэн
        Delayed,         // Хойшилсон
        Cancelled        // Цуцалсан
    }
} 