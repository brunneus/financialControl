using FinanceControl.Domain.Incomes;

namespace FinanceControl.Application.Expenses
{
    public record IncomeRequest(decimal Value, DateTime Date, IncomeType Type, bool Recurrent);
}