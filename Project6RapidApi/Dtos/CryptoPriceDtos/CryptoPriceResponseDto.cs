namespace Project6RapidApi.Dtos.CryptoPriceDtos
{
    public class CryptoPriceResponseDto
    {
        public BitcoinDto bitcoin { get; set; } = new();
        public EthereumDto ethereum { get; set; } = new();

        public class BitcoinDto
        {
            public decimal usd { get; set; }
        }

        public class EthereumDto
        {
            public decimal usd { get; set; }
        }
    }
}