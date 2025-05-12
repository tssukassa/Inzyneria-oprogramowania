using Backend_IO.Models;
using Backend_IO.Services;
using Microsoft.AspNetCore.Mvc;
using Backend_IO.DTO;
using Microsoft.AspNetCore.Authorization;


namespace Backend_IO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var success = _authService.Register(dto);
            if (!success)
            {
                return BadRequest("A user with this login already exists.");
            }

            return Ok("The user has been successfully registered.");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _authService.GetUser(dto.Username);
            if (user == null || !_authService.VerifyPassword(user.PasswordHash, dto.Password))
            {
                return Unauthorized("Incorrect login or password.");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }
    }
}
