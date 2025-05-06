using Microsoft.EntityFrameworkCore;
using Backend_IO.Models;

namespace Backend_IO.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // Таблица пользователей
        public DbSet<Flight> Flights { get; set; } // Таблица перелётов
        public DbSet<Booking> Bookings { get; set; } // Таблица бронирований
    }
}
