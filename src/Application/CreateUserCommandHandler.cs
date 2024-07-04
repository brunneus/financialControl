using FinanceControl.Domain;
using FinanceControl.Infra;
using FinanceControl.Infra.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FinanceControl.Application;

public record struct CreateUserRequest(
    string Name,
    string Email,
    string Password,
    string ConfirmPassword) : IRequest<ResultResponse<CreateUserResponse>>;

public record struct CreateUserResponse(
    string Id,
    string Email);

public class CreateUserCommandHandler(
    FinanceControlDbContext context,
    UserManager<UserAccount> userManager) : IRequestHandler<CreateUserRequest, ResultResponse<CreateUserResponse>>
{

    public async Task<ResultResponse<CreateUserResponse>> Handle(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = new User(request.Name, request.Email);
        var userAccount = new UserAccount(request.Email, request.Name);

        var result = await userManager.CreateAsync(userAccount, request.Password);

        if (!result.Succeeded)
        {
            var identityResults = result
                .Errors
                .Select(err => new ErrorMessage(err.Code, err.Description));

            return new ResultResponse<CreateUserResponse>(identityResults, CommandResultStatus.InvalidInput);
        }

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateUserResponse(user.Id, user.Email);
    }
}
