using Backend_IO.Data;
using Backend_IO.DTO;
using Backend_IO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_IO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Employee,Partner")]
        public IActionResult CreateFlight([FromBody] FlightCreateDto dto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no exists.");

            if (dto == null)
                return BadRequest("Некорректные данные.");

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

            return Ok("Рейс успешно добавлен.");
        }

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
