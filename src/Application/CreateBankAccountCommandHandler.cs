using FinanceControl.Domain;
using FinanceControl.Infra;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Application;

public record CreateBankAccountRequest([Required] string Name) : IRequest<ResultResponse<CreateBankAccountReponse>>;
public record CreateBankAccountReponse(string Id, string Name);

public class CreateBankAccountCommandHandler(FinanceControlDbContext context, IdentityUserService identityUserService)
    : IRequestHandler<CreateBankAccountRequest, ResultResponse<CreateBankAccountReponse>>
{
    public async Task<ResultResponse<CreateBankAccountReponse>> Handle(
        CreateBankAccountRequest command,
        CancellationToken cancellationToken)
    {
        var account = new BankAccount(command.Name);
        var user = await identityUserService.GetCurrentUserAsync();
        user.AddBankAccount(account);

        await context.SaveChangesAsync(cancellationToken);

        return new CreateBankAccountReponse(account.Id, account.Name);
    }
}
