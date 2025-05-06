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

            // Удаляем рейс
            _context.Flights.Remove(flight);
            _context.SaveChanges();

            return Ok("Flight deleted successfully.");
        }

    }
}
