using Backend_IO.Data;
using Backend_IO.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

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
        public bool Register(string username, string password, string role)
        {
            // Проверяем, есть ли уже такой пользователь
            var existingUser = _context.Users.SingleOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                return false; // Пользователь с таким логином уже существует
            }

            // Хешируем пароль
            string hashedPassword = HashPassword(password);

            // Создаём нового пользователя
            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role
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
    }


}
