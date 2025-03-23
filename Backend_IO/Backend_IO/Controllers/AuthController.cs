using Backend_IO.Models;
using Backend_IO.Services;
using Microsoft.AspNetCore.Mvc;

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

        // Регистрация
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var success = _authService.Register(user.Username, user.PasswordHash, user.Role);
            if (!success)
            {
                return BadRequest("Пользователь с таким логином уже существует.");
            }

            return Ok("Пользователь успешно зарегистрирован.");
        }

        // Логин
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var success = _authService.Login(user.Username, user.PasswordHash);
            if (!success)
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            return Ok("Логин успешен.");
        }
    }
}
