using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string PassportNumber { get; set; } = string.Empty;
        
        public string? Email { get; set; }
        
        public string? PhoneNumber { get; set; }

        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
        public virtual ICollection<BoardingPass> BoardingPasses { get; set; } = new List<BoardingPass>();
    }
} 