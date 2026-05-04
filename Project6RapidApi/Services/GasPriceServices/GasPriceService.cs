using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.GasPriceDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.GasPriceServices
{
    public class GasPriceService : IGasPriceService
    {
        IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;

        public GasPriceService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
        }

        public async Task<GasPriceItemDto> GetTurkeyGasPriceAsync()
        {
            var client=_httpClientFactory.CreateClient();
           
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://gas-price.p.rapidapi.com/europeanCountries"),
                Headers =
                 {
                       { "x-rapidapi-key", _apiSettings.Key },
                        { "x-rapidapi-host", "gas-price.p.rapidapi.com" },
                },
                };

            using var response=await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json=await response.Content.ReadAsStringAsync();
            var result=JsonConvert.DeserializeObject<GasPriceResponseDto>(json)!;

            // Sadece Türkiye verisi

            var turkey = result.result
                .FirstOrDefault(x => x.country.ToLower() == "turkey");

            return turkey ?? new GasPriceItemDto();
        }
    }
}
