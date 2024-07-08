﻿using FinanceControl.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;

[ApiController]
[Route("budget-alert")]
public class BudgetAlertController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBudgetAlert(
        [FromServices] IMediator mediator,
        [FromBody] CreateBudgetAlertRequest request)
    {
        var result = await mediator.Send(request);
        return result.ToActionResult(this);
    }
}
