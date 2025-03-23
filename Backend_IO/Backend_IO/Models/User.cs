using System.ComponentModel.DataAnnotations;

namespace Backend_IO.Models
{

        public class User
        {
            public int Id { get; set; } // Идентификатор пользователя

            [Required]
            public string Username { get; set; } // Логин пользователя

            [Required]
            public string PasswordHash { get; set; } // Хэш пароля пользователя

            [Required]
            public string Role { get; set; } // Роль пользователя (например, Client или Employee)
        }
    }

