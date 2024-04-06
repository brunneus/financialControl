using FinanceControl.Infra;

namespace FinanceControl.Application
{
    public class AccountApplicationService
    {
        private readonly FinanceControlDbContext _dbContext;

        public AccountApplicationService(FinanceControlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultResponse<Account>> CreateAccountAsync(CreateAccountRequest request)
        {
            var account = new Account(request.Name);
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            return account;
        }

    }
}
