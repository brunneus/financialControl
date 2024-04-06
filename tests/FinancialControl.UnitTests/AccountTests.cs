using FinanceControl.Domain;
using FluentAssertions;

namespace FinancialControl.UnitTests
{
    public class AccountTests
    {
        [Fact]
        public void Add_Transaction_Which_Exceed_Category_Threshold()
        {
            var account = new Account("testAccount");
            var category = new TransactionCategory("category", "category description", TransactionType.Expense);
            var budgetAlert = new BudgetAlert(account, category, 1000, 0.90m);
            var transaction = new Transaction("market expense", 950, 950, category, account);

            account.AddBudgetAlert(budgetAlert);
            account.AddTransaction(transaction);

            account.Events.Should().BeEquivalentTo(new[] { new BudgetAlertDomainEvent(budgetAlert) });
        }
    }
}
