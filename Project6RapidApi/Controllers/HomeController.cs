using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project6RapidApi.Models;
using Project6RapidApi.Services.CryptoPriceServices;
using Project6RapidApi.Services.Destination;
using Project6RapidApi.Services.ExchangeRateService;
using Project6RapidApi.Services.GasPriceServices;
using Project6RapidApi.Services.HotelServices;
using Project6RapidApi.Services.WeatherForecastServices;
using Project6RapidApi.ViewModels;

namespace Project6RapidApi.Controllers
{
    public class HomeController : Controller
    {

        private readonly IDestinationService _destinationService;
        private readonly IHotelSearchService _hotelSearchService;
        private readonly IHotelDetailService _hotelDetailService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IGasPriceService _gasPriceService;
        private readonly ICryptoPriceService _cryptoPriceService;
        private readonly IWeatherForecastService _weatherForecastService;

        public HomeController(
            IDestinationService destinationService,
            IHotelSearchService hotelSearchService,
            IHotelDetailService hotelDetailService,
            IExchangeRateService exchangeRateService,
            IGasPriceService gasPriceService,
            ICryptoPriceService cryptoPriceService,
            IWeatherForecastService weatherForecastService)
        {
            _destinationService = destinationService;
            _hotelSearchService = hotelSearchService;
            _hotelDetailService = hotelDetailService;
            _exchangeRateService = exchangeRateService;
            _gasPriceService = gasPriceService;
            _cryptoPriceService = cryptoPriceService;
            _weatherForecastService = weatherForecastService;
        }
       
        public async Task<IActionResult> Results(
            string city,
            DateTime checkin,
            DateTime checkout,
            int adults,
            int children,
            int rooms)
        {
            // Önce şehrin dest_id'sini bul
            var destination = await _destinationService.SearchDestinationAsync(city);
            var first = destination.data[0];
            var destId = first.dest_id;
            var searchType = first.dest_type.ToUpperInvariant();

            // Otel ara
            var hotels = await _hotelSearchService.SearchHotelsAsync(
                destId, searchType, checkin, checkout, adults, children, rooms);

            var vm = new StaySearchResultVm
            {
                City = city,
                DestinationLabel = first.label,
                Checkin = checkin,
                Checkout = checkout,
                Adults = adults,
                Children = children,
                Rooms = rooms,
                Properties = hotels,
            };

            return View(vm);
        }


        public async Task<IActionResult> Index()
        {
            var exchangeTask = _exchangeRateService.GetExchangeRatesAsync();
            var gasTask = _gasPriceService.GetTurkeyGasPriceAsync();
            var cryptoTask = _cryptoPriceService.GetCryptoPricesAsync();
            var weatherTask = _weatherForecastService.GetWeatherAsync();

            await Task.WhenAll(exchangeTask, gasTask, cryptoTask, weatherTask);

            var vm = new IndexVm
            {
                ExchangeRates = exchangeTask.Result,
                GasPrice = gasTask.Result,
                CryptoPrices = cryptoTask.Result,
                Weather = weatherTask.Result,
            };

            return View(vm);
        }

      
        public async Task<IActionResult> Detail(
            int hotelId,
            DateTime checkin,
            DateTime checkout,
            int adults,
            int children,
            int rooms)
        {
            var vm = await _hotelDetailService.GetHotelDetailAsync(
                hotelId, checkin, checkout, adults, children, rooms);

            return View(vm);
        }
    }
    }

