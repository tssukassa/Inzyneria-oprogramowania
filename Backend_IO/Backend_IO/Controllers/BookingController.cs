using Backend_IO.Data;
using Backend_IO.DTO;
using Backend_IO.Models;
using Backend_IO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public BookingController(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [Authorize] // Только для авторизованных пользователей
    [HttpPost("create")]
    public IActionResult CreateBooking([FromBody] BookingDto bookingDto)
    {
        // Получаем userId из токена
        var userId = User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        // Проверяем, существует ли рейс
        var flight = _context.Flights.SingleOrDefault(f => f.Id == bookingDto.FlightId);
        if (flight == null)
        {
            return BadRequest("Рейс с таким ID не найден.");
        }

        // Создаём новый заказ
        var booking = new Booking
        {
            UserId = int.Parse(userId),  // Преобразуем строку в int
            FlightId = bookingDto.FlightId,
            BookingDate = DateTime.Now,  // Текущая дата
            Status = "Pending"  // Начальный статус
        };

        // Добавляем заказ в базу данных
        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return Ok("Заказ успешно создан.");
    }

    [HttpGet("my-bookings")]
    [Authorize]
    public IActionResult GetBookings()
    {
        var username = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            return Unauthorized();

        IQueryable<Booking> bookings;

        // Остальные получают только свои бронирования
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
            return Unauthorized();

        bookings = _context.Bookings
            .Where(b => b.UserId == user.Id)
            .Include(b => b.Flight);
        

        // Проецируем в DTO или возвращаем как есть (если DTO пока нет)
        var result = bookings.Select(b => new
        {
            b.Id,
            b.Status,
            b.BookingDate,
            FlightNumber = b.Flight.FlightNumber,
            From = b.Flight.Origin,
            To = b.Flight.Destination,
            Departure = b.Flight.DepartureTime,
            Arrival = b.Flight.ArrivalTime,
            Price = b.Flight.Price,
        }).ToList();

        return Ok(result);
    }

    [HttpGet("all-bookings")]
    [Authorize(Roles = "Employee")]
    public IActionResult AllBookings()
    {
        var username = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            return Unauthorized();

        IQueryable<Booking> bookings;

        // Работник получает все бронирования
        bookings = _context.Bookings
            .Include(b => b.Flight)
            .Include(b => b.User);

        // Проецируем в DTO или возвращаем как есть (если DTO пока нет)
        var result = bookings.Select(b => new
        {
            b.Id,
            b.Status,
            b.BookingDate,
            FlightNumber = b.Flight.FlightNumber,
            From = b.Flight.Origin,
            To = b.Flight.Destination,
            Departure = b.Flight.DepartureTime,
            Arrival = b.Flight.ArrivalTime,
            Price = b.Flight.Price,
            User = b.User.Username
        }).ToList();

        return Ok(result);
    }

    [Authorize]
    [HttpPost("cancel-booking/{bookingId}")]
    public IActionResult CancelBookingByUser(int bookingId)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value);

        var booking = _context.Bookings
            .FirstOrDefault(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found or does not belong to the user.");

        booking.Status = "Cancelled";
        _context.SaveChanges();

        return Ok("Booking cancelled successfully.");
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("cancel-booking-admin/{bookingId}")]
    public IActionResult CancelBookingByEmployee(int bookingId)
    {
        var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);

        if (booking == null)
            return NotFound("Booking not found.");

        booking.Status = "Cancelled";
        _context.SaveChanges();

        return Ok("Booking cancelled by employee.");
    }

    [Authorize(Roles = "Employee,Partner")]
    [HttpPost("cancel-flight-bookings/{flightId}")]
    public IActionResult CancelAllBookingsByFlight(int flightId)
    {
        var bookings = _context.Bookings
            .Where(b => b.FlightId == flightId && b.Status != "Cancelled")
            .ToList();

        if (!bookings.Any())
            return NotFound("No active bookings found for this flight.");

        foreach (var booking in bookings)
        {
            booking.Status = "Cancelled";
        }

        _context.SaveChanges();

        return Ok("All bookings for the flight have been cancelled.");
    }

    [Authorize(Roles = "Employee,Partner")]
    [HttpPost("update-flight-bookings-status/{flightId}")]
    public IActionResult UpdateFlightBookingsStatus(int flightId, [FromBody] string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            return BadRequest("Status is required.");

        var bookings = _context.Bookings
            .Where(b => b.FlightId == flightId)
            .ToList();

        if (!bookings.Any())
            return NotFound("No bookings found for this flight.");

        foreach (var booking in bookings)
        {
            booking.Status = newStatus;
        }

        _context.SaveChanges();

        return Ok($"All bookings for flight ID {flightId} updated to status '{newStatus}'.");
    }

}

