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

        [Required]
        public string FirstName { get; set; }       // Имя (необязательно)

        [Required]
        public string LastName { get; set; }        // Фамилия (необязательно)

        [EmailAddress]
        public string? Email { get; set; }           // Email (необязательно)

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }   // Дата рождения (необязательно)
    }
}

