using FinanceControl.Application.Balance;
using FinanceControl.Application.Expenses;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FinanceControl.IntegrationTests
{
    public class ExpenseControllerTests : IClassFixture<FinanceControlApplicationFactory>
    {
        private readonly FinanceControlApplicationFactory _webApplicationFactory;

        public ExpenseControllerTests(FinanceControlApplicationFactory webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task ShouldAddExpenseCorrectly()
        {
            var client = _webApplicationFactory.CreateClient();
            var request = new ExpenseRequest(150, DateTime.UtcNow, ExpenseType.Food, false);
            var body = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("expenses", stringContent);
            var balanceResponse = await client.GetFromJsonAsync<BalanceResponse>("balance");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(-150, balanceResponse!.Balance);
            await _webApplicationFactory.ClearDatabaseAsync();
        }
    }
}