using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        
        // Navigation properties
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
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