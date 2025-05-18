using Microsoft.EntityFrameworkCore;
using Backend_IO.Models;

namespace Backend_IO.Data
{
    public class BankDbContext : DbContext 
    {
        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options) { }

        public DbSet<BankCard> BankCards { get; set; }
    }
}
