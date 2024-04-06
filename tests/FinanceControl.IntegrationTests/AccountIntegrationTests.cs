using FinanceControl.Controllers;
using FinanceControl.Domain;
using FinanceControl.Infra;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinanceControl.IntegrationTests
{
    public class CreatedResourceDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class AccountIntegrationTests : IClassFixture<FinanceControlWebApplicationFactory>
    {
        private readonly FinanceControlWebApplicationFactory _financeControlWebApplicationFactory;

        public AccountIntegrationTests(FinanceControlWebApplicationFactory financeControlWebApplicationFactory)
        {
            _financeControlWebApplicationFactory = financeControlWebApplicationFactory;
        }

        [Fact]
        public async void Account_Add_Transaction()
        {
            var dbContext = _financeControlWebApplicationFactory
                .Services
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<FinanceControlDbContext>();

            var account = new Account("Teste");
            var transactionCategory = new TransactionCategory("Expenses", "Expenses description", TransactionType.Expense);
            var budgetAlert = new BudgetAlert(account, transactionCategory, 200, 0.7m);
            account.AddBudgetAlert(budgetAlert);

            dbContext.Add(transactionCategory);
            dbContext.Add(account);
            await dbContext.SaveChangesAsync();

            var httpClient = _financeControlWebApplicationFactory.CreateClient();
            var accountRequest = new CreateTransactionRequest(150, "Grocery", transactionCategory.Id);
            var createAccountRequest = CreateRequest(account, accountRequest);

            var response = await httpClient.SendAsync(createAccountRequest);
            await response.Content.ReadAsStringAsync();

            var createdTransaction = dbContext.Transactions.First();
            createdTransaction.Value.Should().Be(150);
            createdTransaction.Description.Should().Be("Grocery");
            createdTransaction.Type.Should().Be(TransactionType.Expense);
            createdTransaction.AccountId.Should().Be(account.Id);
            createdTransaction.CategoryId.Should().Be(transactionCategory.Id);
        }

        private static HttpRequestMessage CreateRequest(Account account, CreateTransactionRequest accountRequest)
        {
            var json = JsonSerializer.Serialize(accountRequest);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var createAccountRequest = new HttpRequestMessage(HttpMethod.Post, $"api/accounts/transactions/{account.Id}")
            {
                Content = body
            };
            return createAccountRequest;
        }
    }
}