using Backend_IO.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Backend_IO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // [GET] Информация о текущем пользователе
        [Authorize]
        [HttpGet("me")]//<--------------------------------------------------------------------------***
        public IActionResult GetMyInfo()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Token invalid or missing.");

            int userId = int.Parse(userIdClaim);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return Unauthorized("User no longer exists.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }


        // [GET] Информация о любом пользователе (только для Employee)
        [Authorize(Roles = "Employee")]
        [HttpGet("user/{id}")]
        public IActionResult GetUserInfo(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound("Пользователь не найден.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Role,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }

        [Authorize]
        [HttpDelete("delete-account")]
        public IActionResult DeleteAccount()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            var activeBookings = _context.Bookings
            .Where(b => b.UserId == userId && b.Status != "Completed" && b.Status != "Cancelled")
            .ToList();

            if (activeBookings.Any())
            {
                return BadRequest("Cannot delete user because there are active bookings.");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            HttpContext.SignOutAsync();

            return Ok("Account deleted successfully.");
        }


        [Authorize(Roles = "Employee")]
        [HttpDelete("delete-account/{userId}")]
        public IActionResult DeleteAccountById(int userId)
        {
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // проверка на логин в текущее время

            var activeBookings = _context.Bookings
            .Where(b => b.UserId == userId && b.Status != "Completed" && b.Status != "Cancelled")
            .ToList();

            if (activeBookings.Any())
            {
                return BadRequest("Cannot delete user because there are active bookings.");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("Account deleted successfully.");
        }

        [HttpGet("all-users")]
        [Authorize(Roles = "Employee")]
        public IActionResult AllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Role,  
                    u.FirstName,  
                    u.LastName, 
                    u.DateOfBirth
                })
                .ToList();

            return Ok(users);
        }

        [HttpGet("all-flights")]
        public IActionResult AllFlights()
        {
         
            var flights = _context.Flights
                .Select(flight => new
                {
                    flight.Id,
                    flight.FlightNumber,
                    flight.Origin,
                    flight.Destination,
                    flight.DepartureTime,
                    flight.ArrivalTime,
                    flight.Price,
                })
                .ToList();

            return Ok(flights);
        }
    }
}
