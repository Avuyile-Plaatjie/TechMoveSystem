using System.Text.Json;

namespace TechMoveSystem.Api.Services
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
                using var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();

                JsonElement json = JsonSerializer.Deserialize<JsonElement>(jsonString);

                return json
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();
            }
            catch
            {
                // Fallback exchange rate if API is unavailable
                return 18.50m;
            }
        }
    }
}