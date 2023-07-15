using FinanceControl.Domain.Incomes;

namespace FinanceControl.Application.Expenses
{
    public class IncomeResponse
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }

        public static implicit operator IncomeResponse(Income income) => new IncomeResponse
        {
            Id = income.Id,
            Value = income.Value,
            Date = income.Date,
        };
    }
}