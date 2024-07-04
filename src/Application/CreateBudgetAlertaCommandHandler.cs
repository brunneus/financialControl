using FinanceControl.Domain;
using FinanceControl.Infra;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Application;

public record CreateBudgetAlertRequest(
    decimal Value,
    decimal Threshold,
    string CategoryId,
    string AccountId) : IRequest<ResultResponse<CreateBudgetAlertResponse>>;

public record CreateBudgetAlertResponse(
    string Id,
    decimal Value,
    decimal Threshold,
    string CategoryId,
    string AccountId);

public class CreateBudgetAlertaCommandHandler(FinanceControlDbContext financeControlDbContext) :
    IRequestHandler<CreateBudgetAlertRequest, ResultResponse<CreateBudgetAlertResponse>>
{

    public async Task<ResultResponse<CreateBudgetAlertResponse>> Handle(
        CreateBudgetAlertRequest request,
        CancellationToken cancellationToken)
    {

        var account = await financeControlDbContext
            .BankAccounts
            .FirstOrDefaultAsync(ac => ac.Id == request.AccountId, cancellationToken: cancellationToken);
        
        var category = await financeControlDbContext
            .TransactionCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken: cancellationToken);

        if (account == null)
        {
            return ValidationErrors.Account.AccountDoesNotExists;
        }

        if (category == null)
        {
            return ValidationErrors.Category.CategoryDoesNotExists;
        }

        var budgetAlert = new BudgetAlert(account, category, request.Value, request.Threshold);

        account.AddBudgetAlert(budgetAlert);

        await financeControlDbContext.SaveChangesAsync(cancellationToken);

        return new CreateBudgetAlertResponse(
            budgetAlert.Id,
            budgetAlert.Value,
            budgetAlert.Threshold,
            budgetAlert.CategoryId,
            budgetAlert.AccountId
        );
    }
}
