using Project6RapidApi.Dtos.CryptoPriceDtos;
using Project6RapidApi.Dtos.GasPriceDtos;
using Project6RapidApi.Dtos.WeatherDtos;

namespace Project6RapidApi.ViewModels
{
    public class IndexVm
    {
        public Dictionary<string, decimal> ExchangeRates { get; set; } = new();
        public GasPriceItemDto GasPrice { get; set; } = new();
        public CryptoPriceResponseDto CryptoPrices { get; set; } = new();
        public WeatherResponseDto Weather { get; set; } = new();
    }
}
