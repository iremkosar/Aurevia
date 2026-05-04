using Newtonsoft.Json;

namespace Project6RapidApi.Dtos.ExchangeRateDtos
{
    public class ExchangeRateResponseDto
    {
        [JsonProperty("base")]
        public string BaseCurrency { get; set; } = "";

        [JsonProperty("date")]
        public string Date { get; set; } = "";

        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new();

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}
