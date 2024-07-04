namespace FinanceControl.Domain
{
    public class User : Entity
    {
        private readonly List<BankAccount> _bankAccounts;

        public User(string name, string email)
        {
            _bankAccounts = [];
            Email = email;
            Name = name;
        }

        public string Email { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<BankAccount> BankAccounts => _bankAccounts;

        public void AddBankAccount(BankAccount bankAccount)
        {
            _bankAccounts.Add(bankAccount);
        }
    }
}
