using FinanceControl.Domain;
using FinanceControl.Infra;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Application;

public record CreateTransactionRequest(
    decimal Value,
    string Description,
    string CategoryId);

public record CreateTransactionCommand(
    CreateTransactionRequest Request,
    string AccountId) : IRequest<ResultResponse<CreateTransactionResponse>>;

public record CreateTransactionResponse(
    string Id,
    string Description,
    decimal Value,
    string CategoryName);

public class CreateIncomeTransactionCommandHandler(
    FinanceControlDbContext context,
    IMessageBroker brokerService,
    ExchangeService exchangeService,
    IdentityUserService identityUserService) : IRequestHandler<CreateTransactionCommand, ResultResponse<CreateTransactionResponse>>
{
    public async Task<ResultResponse<CreateTransactionResponse>> Handle(
        CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var currentUser = await identityUserService.GetCurrentUserAsync();

        var account = currentUser
            .BankAccounts
            .FirstOrDefault(ba => ba.Id == command.AccountId);

        if (account == null)
        {
            return new ResultResponse<CreateTransactionResponse>(ValidationErrors.Account.AccountDoesNotExists, CommandResultStatus.InvalidInput);
        }

        var category = await context.TransactionCategories.FirstOrDefaultAsync(tc =>
            tc.Id == command.Request.CategoryId, cancellationToken: cancellationToken);

        if (category == null)
        {
            return new ResultResponse<CreateTransactionResponse>(ValidationErrors.Category.CategoryDoesNotExists, CommandResultStatus.InvalidInput);
        }

        var usdValueResult = await exchangeService.GetUsdValueAsync(command.Request.Value);
        if (!usdValueResult.Success)
        {
            return new ResultResponse<CreateTransactionResponse>(usdValueResult.Errors, usdValueResult.Status);
        }

        var transaction = new Transaction(command.Request.Description, command.Request.Value, usdValueResult.Result, category, account);

        account.AddTransaction(transaction);

        await context.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in account.Events)
        {
            await brokerService.ProduceMessageAsync(domainEvent);
        }

        return new CreateTransactionResponse(
            transaction.Id,
            transaction.Description,
            transaction.Value,
            category.Name
        );
    }
}
