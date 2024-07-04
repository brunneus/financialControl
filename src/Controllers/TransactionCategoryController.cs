using Asp.Versioning;
using FinanceControl.Application;
using FinanceControl.Infra;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/transaction-categories")]
public class TransactionCategoryController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CreateTransactionCategory(
        [FromServices] IMediator mediator,
        [FromBody] CreateTransactionCategoryRequest transactionRequest)
    {
        var result = await mediator.Send(transactionRequest);
        return result.ToActionResult(this);
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionCategories([FromServices] FinanceControlDbContext context) =>
        Ok(await context.TransactionCategories.ToListAsync());
}
