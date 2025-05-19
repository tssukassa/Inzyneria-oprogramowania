using Backend_IO.Data;
using Backend_IO.DTO;
using Backend_IO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_IO.Controllers
{

    /*
     * FlightController
     *
     * Handles all operations related to flight management including:
     * - Creating flights (restricted to Employees and Partners)
     * - Deleting flights (only if no active bookings exist)
     * - Searching for available flights based on filters
     *
     * Authorization:
     * - Creating and deleting flights requires Employee or Partner role
     * - Searching for flights is publicly accessible
     */
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * POST: /api/flight/create
         *
         * Creates a new flight in the system.
         * Only available to users with "Employee" or "Partner" roles.
         *
         * Request Body:
         * - FlightCreateDto with fields:
         *   - FlightNumber (string)
         *   - Origin (string)
         *   - Destination (string)
         *   - DepartureTime (DateTime)
         *   - ArrivalTime (DateTime)
         *   - Price (decimal)
         *
         * Returns:
         * - 200 OK if flight is successfully created
         * - 400 BadRequest if input is invalid
         * - 401 Unauthorized if user is not authenticated or role not allowed
         */
        [HttpPost("create")]
        [Authorize(Roles = "Employee,Partner")]
        public IActionResult CreateFlight([FromBody] FlightCreateDto dto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no exists.");

            if (dto == null)
                return BadRequest("Invalid data.");

            var flight = new Flight
            {
                FlightNumber = dto.FlightNumber,
                Origin = dto.Origin,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                Price = dto.Price
            };

            _context.Flights.Add(flight);
            _context.SaveChanges();

            return Ok("Flight added successfully.");
        }

        /*
        * DELETE: /api/flight/delete-flight/{flightId}
        *
        * Deletes a flight by ID.
        * Only allowed if there are no active (non-cancelled, non-completed) bookings.
        *
        * Path Parameter:
        * - flightId (int): ID of the flight to delete
        *
        * Returns:
        * - 200 OK if flight is deleted
        * - 400 BadRequest if there are active bookings
        * - 404 NotFound if flight does not exist
        * - 401 Unauthorized if user is not authenticated
        */
        [Authorize(Roles = "Partner,Employee")]
        [HttpDelete("delete-flight/{flightId}")]
        public IActionResult DeleteFlight(int flightId)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no exists.");

            var flight = _context.Flights.Find(flightId);

            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            var activeBookings = _context.Bookings
           .Where(b => b.FlightId == flightId && b.Status != "Completed" && b.Status != "Cancelled")
           .ToList();

            if (activeBookings.Any())
            {
                return BadRequest("Cannot delete flight because there are active bookings.");
            }

            _context.Flights.Remove(flight);
            _context.SaveChanges();

            return Ok("Flight deleted successfully.");
        }

        /*
        * GET: /api/flight/search-flights
        *
        * Searches flights based on optional filters:
        * - Flight number
        * - Origin
        * - Destination
        * - Departure date
        * - Arrival date
        * - Exact price
        *
        * Query Parameters (optional):
        * - flightNumber (string)
        * - origin (string)
        * - destination (string)
        * - departureDate (DateTime)
        * - arrivalDate (DateTime)
        * - price (decimal)
        *
        * Returns:
        * - 200 OK with matching flight list
        */
        [HttpGet("search-flights")]
        public IActionResult SearchFlights(
        string? flightNumber,
        string? origin,
        string? destination,
        DateTime? departureDate,
        DateTime? arrivalDate,
        decimal? price)
        {
            var query = _context.Flights.AsQueryable();

            if (!string.IsNullOrWhiteSpace(flightNumber))
                query = query.Where(f => f.FlightNumber.Contains(flightNumber));

            if (!string.IsNullOrWhiteSpace(origin))
                query = query.Where(f => f.Origin.Contains(origin));

            if (!string.IsNullOrWhiteSpace(destination))
                query = query.Where(f => f.Destination.Contains(destination));

            if (departureDate.HasValue)
                query = query.Where(f => f.DepartureTime.Date == departureDate.Value.Date);

            if (arrivalDate.HasValue)
                query = query.Where(f => f.ArrivalTime.Date == arrivalDate.Value.Date);

            if (price.HasValue)
                query = query.Where(f => f.Price == price.Value);

            var results = query.Select(f => new
            {
                f.Id,
                f.FlightNumber,
                f.Origin,
                f.Destination,
                f.DepartureTime,
                f.ArrivalTime,
                f.Price
            }).ToList();

            return Ok(results);
        }


    }
}
