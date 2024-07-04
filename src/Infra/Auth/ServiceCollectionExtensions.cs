using FinanceControl.Domain;
using FinanceControl.Infra.HostedServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FinanceControl.Infra.Auth;

public static class AuthExtensions
{
    public static IServiceCollection ConfigureAuthorizationAndAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var authenticationSection = configuration.GetSection("Authentication");
        var basicAuthenticationSection = configuration.GetSection("BasicAuthentication");

        serviceCollection.AddOptions<BearerAuthenticationOptions>()
            .Bind(authenticationSection)
            .ValidateOnStart();

        serviceCollection.AddOptions<BasicAuthenticationOptions>()
            .Bind(basicAuthenticationSection)
            .ValidateOnStart();

        serviceCollection.AddOptions<CreateAdminUserOptions>()
            .Bind(configuration.GetSection("AdminCredentials"));

        var authenticationOptions = authenticationSection.Get<BearerAuthenticationOptions>()!;

        serviceCollection
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationOptions.Secret)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authenticationOptions.Issuer,
                    ValidAudience = authenticationOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

        serviceCollection.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();

            options.AddPolicy("Admin", policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim("Role", "Admin");
                policy.RequireAuthenticatedUser();
            });
        });

        serviceCollection.ConfigureIdentity(configuration);

        serviceCollection.AddScoped<ITokenProviderService, TokenProviderService>();

        return serviceCollection;
    }

    public static IServiceCollection ConfigureIdentity(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddIdentity<UserAccount, IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        return serviceCollection;
    }

    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
