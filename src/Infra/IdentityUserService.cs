using FinanceControl.Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinanceControl.Infra;

public class IdentityUserService(FinanceControlDbContext context, IHttpContextAccessor httpContextAccessor)
{
    public async Task<User> GetCurrentUserAsync()
    {
        var currentUserEmail = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.Email)!;

        var user = await context
            .Users
            .Include(us => us.BankAccounts)
            .ThenInclude(ba => ba.Transactions)
            .Include(us => us.BankAccounts)
            .ThenInclude(ba => ba.BudgetAlerts)
            .FirstAsync(us => us.Email == currentUserEmail.Value);

        return user;
    }
}
