using Asp.Versioning;
using FinanceControl.Application;
using FinanceControl.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Controllers;

public record CreateTransactionRequest(decimal Value, string Description, string CategoryId);

[ApiController]
[ApiVersion(1.0)]
[Route("api/accounts")]
public class AccountController(
    FinanceControlDbContext context,
    ILogger<AccountController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAccounts([FromQuery] int skip, int take = 10)
    {
        var accounts = await context
            .Accounts
            .Include(ac => ac.Transactions)
            .Include(ac => ac.BudgetAlerts)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IStatusCodeActionResult> CreateAccount(
        [FromServices] AccountApplicationService appservice,
        [FromBody] CreateAccountRequest request)
    {
        var result = await appservice.CreateAccountAsync(request);

        return result.ToActionResult(this);
    }

    [HttpPost("transactions/{accountId}")]
    public async Task<IActionResult> CreateIncomeTransaction(
        string accountId,
        [FromServices] FinanceControlDbContext context,
        [FromServices] IMessageBroker brokerService,
        [FromServices] ExchangeService exchangeService,
        [FromBody] CreateTransactionRequest request)
    {
        var account = await context.Accounts
            .Include(e => e.Transactions)
            .Include(e => e.BudgetAlerts)
            .FirstOrDefaultAsync(acc => acc.Id == accountId);

        var category = await context.TransactionCategories.FirstOrDefaultAsync(tc => tc.Id == request.CategoryId);

        if (account == null)
        {
            return BadRequest("Conta não existe");
        }

        if (category == null)
        {
            return BadRequest("Categoria não existe");
        }

        var usdValue = await exchangeService.GetUsdValueAsync(request.Value);
        var transaction = new Transaction(request.Description, request.Value, usdValue, category, account);

        account.AddTransaction(transaction);

        await context.SaveChangesAsync();

        foreach (var domainEvent in account.Events)
        {
            await brokerService.ProduceMessageAsync(domainEvent);
        }

        return Ok(account.Transactions.Last());
    }

}
