using Project6RapidApi.Dtos.CryptoPriceDtos;

namespace Project6RapidApi.Services.CryptoPriceServices
{
    public interface ICryptoPriceService
    {
        Task<CryptoPriceResponseDto> GetCryptoPricesAsync();
    }
}
