using FinanceControl.Domain;

public class Account : Entity
{
    private readonly List<Transaction> _transactions;
    private readonly List<BudgetAlert> _budgetAlerts;
    private readonly List<IDomainEvent> _domainEvents;

    public Account(string name)
    {
        UpdatedAt = DateTime.UtcNow;
        Balance = 0;
        Name = name;
        _transactions = [];
        _budgetAlerts = [];
        _domainEvents = [];
    }

    public string Name { get; private set; }
    public decimal Balance { get; private set; }
    public List<Transaction> Transactions => _transactions;
    public List<BudgetAlert> BudgetAlerts => _budgetAlerts;
    public IEnumerable<IDomainEvent> Events => _domainEvents;

    public void AddTransaction(Transaction transaction)
    {
        if (transaction.Type == TransactionType.Income)
        {
            Balance += transaction.Value;
        }
        else
        {
            Balance -= transaction.Value;
        }

        Transactions.Add(transaction);

        var alerts = _budgetAlerts.Where(ba => ba.HasExceedThreshold(this));

        foreach (var alert in alerts)
        {
            _domainEvents.Add(new BudgetAlertDomainEvent(alert));
        }
    }

    public void AddBudgetAlert(BudgetAlert budgetAlert)
    {
        _budgetAlerts.Add(budgetAlert);
    }
}

public interface IDomainEvent { }

public class BudgetAlertDomainEvent(BudgetAlert BudgetAlert) : IDomainEvent
{
    public BudgetAlert BudgetAlert { get; } = BudgetAlert;
}
