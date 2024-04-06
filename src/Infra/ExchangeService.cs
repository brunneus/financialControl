using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace FinanceControl.Infra
{
    public class ExchangeServiceOptions
    {
        public string ApiUrl { get; set; }
    };

    public record CotationResponse(decimal High);

    public class ExchangeService(IOptions<ExchangeServiceOptions> options)
    {
        private readonly IOptions<ExchangeServiceOptions> _options = options;

        public async Task<decimal> GetUsdValueAsync(decimal referenceValue)
        {
            var response = await _options.Value.ApiUrl
                    .AppendPathSegment("json/last/USD")
                    .AllowAnyHttpStatus()
                    .GetJsonAsync<Dictionary<string, CotationResponse>>();

            return referenceValue / response.First().Value.High;
        }
    }
}
