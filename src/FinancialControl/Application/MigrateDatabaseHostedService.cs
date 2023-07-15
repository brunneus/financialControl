using FinanceControl.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Application;

public class MigrateDatabaseHostedService : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<MigrateDatabaseHostedService> _logger;

    public MigrateDatabaseHostedService(IServiceProvider sp, ILogger<MigrateDatabaseHostedService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Migrating database...");
        using var scope = _sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceControlDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Migration finished");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}