using Microsoft.EntityFrameworkCore;
using Backend_IO.Models;

namespace Backend_IO.Data
{
    /*
      * Entity Framework Core DbContext for the main application database.
      *
      * This class is responsible for managing the database connection and 
      * providing access to the core entities: Users, Flights, and Bookings.
      * It configures the database context using options passed from startup.
      *
      * Properties:
      * - Users: Represents the Users table in the database.
      * - Flights: Represents the Flights table in the database.
      * - Bookings: Represents the Bookings table in the database.
      *
      * Usage:
      * The ApplicationDbContext is injected into services/controllers to perform
      * CRUD operations on the application's main data entities.
      */

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } // DbSet representing Users in the database

        public DbSet<Flight> Flights { get; set; } // DbSet representing Flights in the database

        public DbSet<Booking> Bookings { get; set; } // DbSet representing Bookings in the database

    }
}
