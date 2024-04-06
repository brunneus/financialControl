using FinanceControl.Infra;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Testcontainers.MsSql;

namespace FinanceControl.IntegrationTests
{
    public class FinanceControlWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
            .Build();

        private readonly MockacoContainer _httpMockContainer = new();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var messageBroker = Substitute.For<IMessageBroker>();

                services.AddScoped(_ => messageBroker);
            });

            return base.CreateHost(builder);
        }

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();
            await _httpMockContainer.InitializeAsync();

            Environment.SetEnvironmentVariable("ConnectionStrings:Database", _sqlContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ExchangeService:ApiUrl", _httpMockContainer.ContainerUri);
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _sqlContainer.StopAsync();
            await _httpMockContainer.DisposeAsync();
        }
    }
}
