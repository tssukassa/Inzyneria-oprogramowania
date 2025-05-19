using Backend_IO.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_IO.Controllers
{

    /*
     * PartnerController
     * 
     * This controller provides endpoints related to partner-specific data and operations.
     * 
     * Authorization:
     * - Endpoints require the user to have the "Partner" role.
     */
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PartnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * GET: /api/partner/booking-stats
         * 
         * Retrieves booking statistics for all flights.
         * 
         * Authorization:
         * - Requires the authenticated user to have the "Partner" role.
         * 
         * Process:
         * - Validates that the requesting user exists in the database.
         * - For each flight, aggregates:
         *   - The total number of bookings (Count).
         *   - The total revenue generated from those bookings (TotalRevenue).
         * - Calculates overall totals across all flights.
         * 
         * Returns:
         * - 200 OK with a JSON object containing:
         *   - TotalBookings: total number of bookings across all flights.
         *   - TotalRevenue: total revenue across all flights.
         *   - ByFlight: an array with statistics for each flight (FlightId, Count, TotalRevenue).
         * - 401 Unauthorized if the requesting user does not exist.
         */
        [Authorize(Roles = "Partner")]
        [HttpGet("booking-stats")]
        public IActionResult GetBookingStats()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no exists.");

            var flightStats = _context.Flights
                .GroupJoin(
                    _context.Bookings,
                    flight => flight.Id,
                    booking => booking.FlightId,
                    (flight, bookings) => new
                    {
                        FlightId = flight.Id,
                        Count = bookings.Count(),
                        TotalRevenue = bookings.Sum(b => (decimal?)b.Flight.Price) ?? 0
                    })
                .ToList();

            var totalBookings = flightStats.Sum(f => f.Count);
            var totalRevenue = flightStats.Sum(f => f.TotalRevenue);

            return Ok(new
            {
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                ByFlight = flightStats
            });
        }
    }
}
