namespace Project6RapidApi.Dtos.GasPriceDtos
{
    public class GasPriceResponseDto
    {
        public bool success { get; set; }
        public List<GasPriceItemDto> result { get; set; } = new();
    }

    public class GasPriceItemDto
    {
        public string country { get; set; } = "";
        public string currency { get; set; } = "";
        public string gasoline { get; set; } = "";
        public string diesel { get; set; } = "";
        public string lpg { get; set; } = "";
    }
}
