using FinanceControl.Infra;
using FinanceControl.Infra.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;

namespace FinanceControl.IntegrationTests
{
    public class FinanceControlApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder().Build();
        private readonly KafkaContainer _kafkaContainer = new KafkaBuilder().Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextOptionsDescriptor = services.SingleOrDefault(
                   d => d.ServiceType == typeof(DbContextOptions<FinanceControlDbContext>)
                );

                var brokerOptionsDescriptor = services.SingleOrDefault(
                   d => d.ServiceType == typeof(BrokerOptions)
                );

                services.Remove(dbContextOptionsDescriptor!);
                services.Remove(brokerOptionsDescriptor!);

                services.AddDbContext<FinanceControlDbContext>(options =>
                {
                    options.UseNpgsql(_postgresContainer.GetConnectionString());
                });

                services.Configure<BrokerOptions>(options =>
                {
                    options.BootstrapServers = _kafkaContainer.GetBootstrapAddress();
                });
            });
        }

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();
            await _kafkaContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _postgresContainer.StopAsync();
            await _kafkaContainer.StopAsync();
        }

        public async Task ClearDatabaseAsync()
        {
            using var connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
            await connection.OpenAsync();
            var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres
            });

            await respawner.ResetAsync(connection);
        }

        public string GetBootstrapAddress() => _kafkaContainer.GetBootstrapAddress();
    }
}
