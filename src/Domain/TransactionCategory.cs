namespace FinanceControl.Domain;

public class TransactionCategory(string name, string description, TransactionType type) : Entity
{
    public string Name { get; init; } = name;
    public string Description { get; init; } = description;
    public TransactionType Type { get; init; } = type;
}
