using FinanceControl.Domain;
using FinanceControl.Infra;
using FinanceControl.Infra.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FinanceControl.Application;

public record struct LoginRequest(
    string Email,
    string Password) : IRequest<ResultResponse<LoginResponse>>;

public record struct LoginResponse(
    string AccessToken, 
    string RefreshToken);

public class LoginCommandHandler(
    UserManager<UserAccount> userManager,
    SignInManager<UserAccount> signInManager,
    ITokenProviderService tokenProviderService) : IRequestHandler<LoginRequest, ResultResponse<LoginResponse>>
{
    public async Task<ResultResponse<LoginResponse>> Handle(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var signInResult = await signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);

        if (!signInResult.Succeeded)
        {
            return new ResultResponse<LoginResponse>(CommandResultStatus.Unauthorized);
        }

        var user = (await userManager.FindByNameAsync(request.Email))!;

        var accessToken = tokenProviderService.GenerateToken(user);
        var refreshToken = tokenProviderService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(15));
        await userManager.UpdateAsync(user);

        return new LoginResponse(accessToken, refreshToken);
    }
}
