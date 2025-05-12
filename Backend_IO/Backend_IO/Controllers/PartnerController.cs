using Backend_IO.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Backend_IO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Partner")]
        [HttpGet("booking-stats")]
        public IActionResult GetBookingStats()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no exists.");

            var flightIds = _context.Flights
                .Select(f => f.Id)
                .ToList();

            var flightStats = flightIds.Select(flightId => new
            {
                FlightId = flightId,
                Count = _context.Bookings.Count(b => b.FlightId == flightId),
                TotalRevenue = _context.Bookings
                    .Where(b => b.FlightId == flightId)
                    .Sum(b => (decimal?)b.Flight.Price) ?? 0
            })
            .ToList();

            var flightStatsWithDefaults = flightStats.Select(flight => new
            {
                FlightId = flight.FlightId,
                Count = flight.Count > 0 ? flight.Count : 0, 
                TotalRevenue = flight.Count > 0 ? flight.TotalRevenue : 0  
            }).ToList();

            var totalBookings = flightStatsWithDefaults.Sum(f => f.Count);
            var totalRevenue = flightStatsWithDefaults.Sum(f => f.TotalRevenue);

            return Ok(new
            {
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                ByFlight = flightStatsWithDefaults
            });
        }
    }
}
