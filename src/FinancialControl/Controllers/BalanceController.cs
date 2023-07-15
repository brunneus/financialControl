using FinanceControl.Application.Balance;
using FinanceControl.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Controllers;

[ApiController]
[Route("[controller]")]
public class BalanceController : ControllerBase
{
    [HttpGet(Name = "GetBalance")]
    public async Task<IActionResult> GetBalanceAsync([FromServices] FinanceControlDbContext context)
    {
        var expenses = await context.Expenses.SumAsync(expense => expense.Value);
        var incomes = await context.Incomes.SumAsync(income => income.Value);

        var response = new BalanceResponse
        {
            Balance = incomes - expenses
        };

        return Ok(response);
    }
}
