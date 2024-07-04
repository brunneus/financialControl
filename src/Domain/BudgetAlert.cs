namespace FinanceControl.Domain;

public class BudgetAlert : Entity
{
    private BudgetAlert()
    {

    }

    public BudgetAlert(BankAccount account, TransactionCategory category, decimal value, decimal threshold)
    {
        AccountId = account.Id;
        CategoryId = category.Id;
        Value = value;
        Threshold = threshold;
    }

    public decimal Value { get; private set; }
    public decimal Threshold { get; private set; }
    public string CategoryId { get; init; }
    public string AccountId { get; init; }

    public bool HasExceedThreshold(BankAccount account)
    {
        var categoryTransactions = account.Transactions
            .Where(ac => ac.CategoryId == CategoryId);

        var transactionsSum = account.Transactions.Sum(t => t.Value);

        return (transactionsSum / Value) > Threshold;
    }
}
