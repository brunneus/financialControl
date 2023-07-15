using Confluent.Kafka;
using FinanceControl.Application.Balance;
using FinanceControl.Application.Expenses;
using FinanceControl.Domain.Incomes;
using FinanceControl.Infra;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FinanceControl.IntegrationTests
{
    public class IncomeControllerTests : IClassFixture<FinanceControlApplicationFactory>
    {
        private readonly FinanceControlApplicationFactory _webApplicationFactory;

        public IncomeControllerTests(FinanceControlApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task ShouldAddIncomeCorrectly()
        {
            var client = _webApplicationFactory.CreateClient();
            var request = new IncomeRequest(150, DateTime.UtcNow, IncomeType.Bonus, false);
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("incomes", stringContent);
            var balanceResponse = await client.GetFromJsonAsync<BalanceResponse>("balance");

            var config = new ConsumerConfig
            {
                BootstrapServers = _webApplicationFactory.GetBootstrapAddress(),
                GroupId = "integrationTestsConsumer",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(Consts.IncomesAddedTopicName);

            var kafkaMessage = consumer.Consume();
            var incomeAddedProducedEvent = JsonSerializer.Deserialize<IncomeAddedEvent>(kafkaMessage.Message.Value);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(150, balanceResponse!.Balance);
            Assert.Equal(150, incomeAddedProducedEvent!.Value);
            Assert.Equal("IncomeAddedEvent", incomeAddedProducedEvent.EventName);
        }
    }
}