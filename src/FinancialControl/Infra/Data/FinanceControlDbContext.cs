using FinanceControl.Domain.Expenses;
using FinanceControl.Domain.Incomes;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infra.Data
{
    public class FinanceControlDbContext : DbContext
    {
        public FinanceControlDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Income> Incomes { get; set; }
    }
}