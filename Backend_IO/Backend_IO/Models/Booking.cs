using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{
    public class Booking
    {
        public int Id { get; set; } 

        [Required]
        public int UserId { get; set; } 

        [Required]
        public int FlightId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } 

        [Required]
        public string Status { get; set; } 

        public virtual User User { get; set; }

        public virtual Flight Flight { get; set; }
    }
}
