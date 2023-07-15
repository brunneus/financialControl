namespace FinanceControl.Domain.Incomes
{
    public class IncomeAddedEvent
    {
        public decimal Value { get; set; }

        public string EventName => nameof(IncomeAddedEvent);
    }
}
