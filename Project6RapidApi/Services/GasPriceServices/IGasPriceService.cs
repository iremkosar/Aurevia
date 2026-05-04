using Project6RapidApi.Dtos.GasPriceDtos;

namespace Project6RapidApi.Services.GasPriceServices
{
    public interface IGasPriceService
    {
        Task<GasPriceItemDto> GetTurkeyGasPriceAsync();
    }
}
