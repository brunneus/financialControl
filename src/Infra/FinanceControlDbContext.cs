using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infra
{
    public class FinanceControlDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
