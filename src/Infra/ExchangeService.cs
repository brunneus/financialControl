using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace FinanceControl.Infra;

public class ExchangeServiceOptions
{
    public string ApiUrl { get; set; }
};

public record CotationResponse(string High);

public class ExchangeService(IOptions<ExchangeServiceOptions> options)
{
    private readonly IOptions<ExchangeServiceOptions> _options = options;

    public async Task<ResultResponse<decimal>> GetUsdValueAsync(decimal referenceValue)
    {
        var response = await _options.Value.ApiUrl
                .AppendPathSegment("json/last/USD")
                .AllowAnyHttpStatus()
                .GetAsync();

        if (!response.ResponseMessage.IsSuccessStatusCode)
        {
            var responseContent = await response.GetStringAsync();
            var error = ValidationErrors.General.UnknownError(responseContent);

            return new ResultResponse<decimal>(error, CommandResultStatus.Unknown);
        }

        var responseAsJson = await response.GetJsonAsync<Dictionary<string, CotationResponse>>();
        var cotationAsNumber = decimal.Parse(responseAsJson.First().Value.High);

        return referenceValue / cotationAsNumber;
    }
}
