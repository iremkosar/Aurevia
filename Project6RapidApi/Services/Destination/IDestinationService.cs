using Project6RapidApi.Dtos.DestinationDtos;

namespace Project6RapidApi.Services.Destination
{
    public interface IDestinationService
    {
        Task<DestinationResponseDto> SearchDestinationAsync(string query);
    }
}
