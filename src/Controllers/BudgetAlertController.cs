using FinanceControl.Domain;
using FinanceControl.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Controllers;

public record CreateBudgetAlertRequest(decimal Value, decimal Threshold, string CategoryId, string AccountId);

public record CreateBudgetAlertResponse(string Id, decimal Value, decimal Threshold, string CategoryId, string AccountId)
{
    public static implicit operator CreateBudgetAlertResponse(BudgetAlert ba) => new CreateBudgetAlertResponse(
        ba.Id,
        ba.Value,
        ba.Threshold,
        ba.CategoryId,
        ba.AccountId
    );
}

[ApiController]
[Route("budget-alert")]
public class BudgetAlertController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBudgetAlert(
        [FromBody] CreateBudgetAlertRequest request,
        [FromServices] FinanceControlDbContext financeControlDbContext)
    {
        var account = await financeControlDbContext.Accounts.FirstOrDefaultAsync(ac => ac.Id == request.AccountId);
        var category = await financeControlDbContext.TransactionCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);

        if (account == null)
        {
            return BadRequest("Conta não existe");
        }

        if (category == null)
        {
            return BadRequest("Categoria não existe");
        }

        var budgetAlert = new BudgetAlert(account, category, request.Value, request.Threshold);

        account.AddBudgetAlert(budgetAlert);

        await financeControlDbContext.SaveChangesAsync();

        return Ok((CreateBudgetAlertResponse)budgetAlert);
    }
}
