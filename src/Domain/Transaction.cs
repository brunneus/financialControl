using FinanceControl.Domain;

public class Transaction : Entity
{
    private Transaction()
    {

    }

    public Transaction(string description, decimal value, decimal usdValue, TransactionCategory category, Account account)
    {
        Description = description;
        Value = value;
        UsdValue = usdValue;
        Type = category.Type;
        CategoryId = category.Id;
        AccountId = account.Id;
    }

    public string Description { get; private set; }
    public decimal Value { get; private set; }
    public decimal UsdValue { get; init; }
    public TransactionType Type { get; init; }
    public string AccountId { get; init; }
    public string CategoryId { get; set; }
}