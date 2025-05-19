namespace Backend_IO.DTO
{
    /*
     * Data Transfer Object (DTO) used to create a new Flight record.
     * 
     * This DTO carries the necessary data for creating a flight,
     * including route, schedule, and pricing information.
     * 
     * Properties:
     * - FlightNumber (string): Unique identifier for the flight.
     * - Origin (string): Departure location.
     * - Destination (string): Arrival location.
     * - DepartureTime (DateTime): Scheduled departure date and time.
     * - ArrivalTime (DateTime): Scheduled arrival date and time.
     * - Price (decimal): Cost of the flight ticket.
     * 
     * All fields are required to successfully create a flight.
     */
    public class FlightCreateDto
    {
        public string FlightNumber { get; set; } // Unique flight number
        public string Origin { get; set; } // Departure airport or city
        public string Destination { get; set; } // Arrival airport or city

        public DateTime DepartureTime { get; set; } // Date and time of departure

        public DateTime ArrivalTime { get; set; } // Date and time of arrival
        public decimal Price { get; set; } // Ticket price for the flight
    }
}
