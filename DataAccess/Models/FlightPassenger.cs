using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class FlightPassenger
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        [Required]
        public int PassengerId { get; set; }
        
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("FlightId")]
        public virtual Flight Flight { get; set; } = null!;
        
        [ForeignKey("PassengerId")]
        public virtual Passenger Passenger { get; set; } = null!;
    }
}