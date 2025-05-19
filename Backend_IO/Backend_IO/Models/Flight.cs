using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_IO.Models
{
    public class Flight
    {

        /*
        * Represents a flight available for booking in the system.
        * 
        * Properties:
        * - Id (int): Unique identifier for the flight (Primary Key).
        * - Origin (string): The departure location of the flight.
        * - Destination (string): The arrival location of the flight.
        * - DepartureTime (DateTime): Scheduled date and time of departure.
        * - ArrivalTime (DateTime): Scheduled date and time of arrival.
        * - Price (decimal): Ticket price for the flight (stored as REAL in SQLite).
        * - FlightNumber (string): Official flight number identifier (e.g., "AA123").
        * 
        * Notes:
        * - All fields are required for flight creation.
        * - Used in the booking system to reference available flights.
        */

        public int Id { get; set; } // Primary Key: Unique identifier of the flight

        [Required]
        public string Origin { get; set; }  // Departure location (e.g., "New York")

        [Required]
        public string Destination { get; set; }  // Arrival location (e.g., "London")


        [Required]
        public DateTime DepartureTime { get; set; } // Date and time of flight departure

        [Required]
        public DateTime ArrivalTime { get; set; } // Date and time of flight arrival


        [Required]
        [Column(TypeName = "REAL")]
        public decimal Price { get; set; } // Ticket price for the flight

        [Required]
        public string FlightNumber { get; set; } // Unique flight number identifier
    }
}
