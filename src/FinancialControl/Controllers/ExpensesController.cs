using FinanceControl.Application.Expenses;
using FinanceControl.Domain.Expenses;
using FinanceControl.Infra;
using FinanceControl.Infra.Data;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;

[ApiController]
[Route("[controller]")]
public class ExpensesController : ControllerBase
{
    public ExpensesController()
    {
    }

    [HttpPost(Name = "CreateExpense")]
    public async Task<IActionResult> CreateExpenseAsync(
        [FromBody] ExpenseRequest request,
        [FromServices] FinanceControlDbContext context,
        [FromServices] BrokerService brokerService)
    {
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Date = request.Date,
            IsRecurrent = request.Recurrent,
            Type = request.Type,
            Value = request.Value
        };
        context.Add(expense);
        await context.SaveChangesAsync();
        await brokerService.SendMessageAsync(new ExpenseAddedEvent { Value = expense.Value }, Consts.ExpensesAddedTopicName);

        return Created("expenses", expense);
    }
}
