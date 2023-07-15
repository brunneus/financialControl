using FinanceControl.Domain.Base;

namespace FinanceControl.Domain.Expenses
{
    public class Expense : Entity
    {
        public decimal Value { get; set; }
        public ExpenseType Type { get; set; }
        public DateTime Date { get; set; }
        public bool IsRecurrent { get; set; }
    }
}