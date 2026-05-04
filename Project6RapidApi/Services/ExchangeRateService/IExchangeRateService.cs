namespace Project6RapidApi.Services.ExchangeRateService
{
    public interface IExchangeRateService
    {
        Task<Dictionary<string, decimal>> GetExchangeRatesAsync();
    }
}
