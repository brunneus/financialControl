using Asp.Versioning;
using FinanceControl.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Controllers;

public record CreateTransactionCategoryRequest(string Name, string Description, TransactionType CategoryType);

[ApiController]
[ApiVersion(1.0)]
[Route("api/transaction-categories")]
public class TransactionCategoryController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTransactionCategory(
        [FromBody] CreateTransactionCategoryRequest transactionRequest,
        [FromServices] FinanceControlDbContext context)
    {
        var transactionCategory = new TransactionCategory(
            transactionRequest.Name,
            transactionRequest.Description,
            transactionRequest.CategoryType
        );

        context.TransactionCategories.Add(transactionCategory);

        await context.SaveChangesAsync();

        return Ok(transactionCategory);
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionCategories([FromServices] FinanceControlDbContext context) =>
        Ok(await context.TransactionCategories.ToListAsync());
}
