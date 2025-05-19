using Microsoft.EntityFrameworkCore;
using Backend_IO.Models;

namespace Backend_IO.Data
{
    /*
     * BankDbContext
     * 
     * This class represents the Entity Framework Core database context for the banking system.
     * It is responsible for managing access to the BankCards table, which contains details about
     * users' payment cards such as card number, CVV2, expiration date, and current balance.
     * 
     * Constructor:
     * - BankDbContext(DbContextOptions<BankDbContext> options): Initializes the context with the provided options.
     * 
     * Properties:
     * - DbSet<BankCard> BankCards: Exposes the BankCards table to allow querying, adding, updating, and deleting card records.
     * 
     * Usage:
     * This context is typically used for validating card information and processing payments or refunds
     * in booking operations.
     */

    public class BankDbContext : DbContext 
    {
        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options) { }

        public DbSet<BankCard> BankCards { get; set; } // Table representing all bank cards in the system

    }
}
