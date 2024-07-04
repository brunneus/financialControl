using FinanceControl.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infra
{
    public class FinanceControlDbContext : DbContext
    {
        public FinanceControlDbContext(DbContextOptions<FinanceControlDbContext> options)
            : base(options)
        {
            
        }

        public FinanceControlDbContext(string connectionString)
        {
            Database.SetConnectionString(connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.BankAccounts)
                .WithOne();

            modelBuilder.Entity<BankAccount>()
                .HasMany(b => b.Transactions)
                .WithOne();
        }
    }
}
