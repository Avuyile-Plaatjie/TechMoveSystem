using System.Net.Http.Json;

namespace TechMoveSystem.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRate()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>("https://open.er-api.com/v6/latest/USD");
                return response?.rates != null && response.rates.ContainsKey("ZAR")
                    ? response.rates["ZAR"]
                    : 18.50m;
            }
            catch
            {
                return 18.50m; 
            }
        }
    }

    public class ExchangeRateResponse
    {
        public Dictionary<string, decimal> rates { get; set; } = new();
    }
}