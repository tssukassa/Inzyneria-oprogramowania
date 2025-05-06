using Backend_IO.Data;
using Backend_IO.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend_IO.DTO;


namespace Backend_IO.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }
        // Хеширование пароля

        public string HashPassword(string password)
        {
            // Генерация соли
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Хеширование пароля с солью
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Сохранение соли и хеша пароля (соль хранится в открытом виде для проверки)
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        // Проверка пароля
        public bool VerifyPassword(string hashedPassword, string password)
        {
            var parts = hashedPassword.Split('.');
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashToCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash == hashToCheck;
        }

        // Регистрация пользователя
        public bool Register(RegisterDto dto)
        {
            // Проверяем, есть ли уже такой пользователь
            var existingUser = _context.Users.SingleOrDefault(u => u.Username == dto.Username);
            if (existingUser != null)
            {
                return false; // Пользователь с таким логином уже существует
            }

            string role = dto.RoleKey switch
            {
                "worker123" => "Employee",
                "partner321" => "Partner",
                _ => "Client" // если ключ не введён или неправильный
            };

            // Хешируем пароль
            string hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Role = role,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth
            };

            // Добавляем пользователя в базу данных
            _context.Users.Add(user);
            _context.SaveChanges();

            return true;
        }

        // Логин пользователя
        public bool Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false; // Пользователь не найден
            }

            // Проверяем, совпадает ли хешированный пароль
            return VerifyPassword(user.PasswordHash, password);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("userId", user.Id.ToString()) // Добавляем ID пользователя в токен
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User GetUser(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

    }

}
