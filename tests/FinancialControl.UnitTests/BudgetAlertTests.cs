using FinanceControl.Domain;

namespace FinancialControl.UnitTests
{
    public class BudgetAlertTests
    {
        [Fact]
        public void Transactions_with_value_bigger_than_threshold_should_notify()
        {
            var account = new Account("teste");
            var transactionCategory = new TransactionCategory("expenses", "expenses", TransactionType.Expense);
            var transaction = new Transaction("description", 175, 175, transactionCategory, account);

            account.Transactions.Add(transaction);

            var sut = new BudgetAlert(account, transactionCategory, 200, 0.8m);
            Assert.True(sut.HasExceedThreshold(account));
        }
    }
}