using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_IO.Models
{
    public class Flight
    {
        public int Id { get; set; } 

        [Required]
        public string Origin { get; set; } 

        [Required]
        public string Destination { get; set; } 

        [Required]
        public DateTime DepartureTime { get; set; } 

        [Required]
        public DateTime ArrivalTime { get; set; } 

        [Required]
        [Column(TypeName = "REAL")]
        public decimal Price { get; set; } 

        [Required]
        public string FlightNumber { get; set; } 
    }
}
