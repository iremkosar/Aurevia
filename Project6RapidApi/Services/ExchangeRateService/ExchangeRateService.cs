using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.ExchangeRateDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.ExchangeRateService
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;

        public ExchangeRateService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/latest?base=TRY&symbols=EUR%2CUSD%2CRUB"),
                Headers =
                {
                    { "x-rapidapi-key", _apiSettings.Key },
                    { "x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com" },
                },
            };

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ExchangeRateResponseDto>(json)!;

            
            var converted = result.Rates.ToDictionary(
                r => r.Key,
                r => r.Value > 0 ? Math.Round(1 / r.Value, 4) : 0
            );

            return converted;
        }
    }
}