using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.CryptoPriceDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.CryptoPriceServices
{
    public class CryptoPriceService : ICryptoPriceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;
        

        public CryptoPriceService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
        }

        public async Task<CryptoPriceResponseDto> GetCryptoPricesAsync()
        {
            var client=_httpClientFactory.CreateClient();
           
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://crypto-price-api.p.rapidapi.com/cryptoPrice?symbol=Ethereum%2CBitcoin"),
                Headers =
                    {
                       { "x-rapidapi-key", _apiSettings.Key },
                        { "x-rapidapi-host", "crypto-price-api.p.rapidapi.com" },
                    },
                    };
            using var response=await client.SendAsync(request);          
            response.EnsureSuccessStatusCode();
            var json=await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CryptoPriceResponseDto>(json)!;

        }
    }
}
