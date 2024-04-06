using FinanceControl.Domain;

public class TransactionCategory : Entity
{
    public TransactionCategory(string name, string description, TransactionType type)
    {
        Name = name;
        Description = description;
        Type = type;
    }

    public string Name { get; init; }
    public string Description { get; init; }
    public TransactionType Type { get; init; }
}
