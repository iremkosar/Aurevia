using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project6RapidApi.Dtos;

namespace Project6RapidApi.Controllers
{
    public class CurrencyController : Controller
    {
        public async Task<IActionResult> Index()
        {
          
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://currency-conversion-and-exchange-rates.p.rapidapi.com/convert?from=USD&to=TRY&amount=1"),
                Headers =
            {
                { "x-rapidapi-key", "47751142damsh49a735a0ef5a185p1f08afjsn13716bb0e823" },
                { "x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com" },
            },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();
                var value=JsonConvert.DeserializeObject<CurrencyDto>(jsonData);
                ViewBag.Currency = value.result;             
            }
            return View();
        }
    }
}
