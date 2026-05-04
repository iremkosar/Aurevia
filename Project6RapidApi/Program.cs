using Project6RapidApi.Services.CryptoPriceServices;
using Project6RapidApi.Services.Destination;
using Project6RapidApi.Services.ExchangeRateService;
using Project6RapidApi.Services.GasPriceServices;
using Project6RapidApi.Services.HotelServices;
using Project6RapidApi.Services.ReviewServices;
using Project6RapidApi.Services.RoomListServices;
using Project6RapidApi.Services.WeatherForecastServices;
using Project6RapidApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<RapidApiSettings>(
    builder.Configuration.GetSection("RapidApi"));

// HttpClient
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Servisler
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<IHotelSearchService, HotelSearchService>();
builder.Services.AddScoped<IHotelDetailService, HotelDetailService>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IGasPriceService, GasPriceService>();
builder.Services.AddScoped<ICryptoPriceService, CryptoPriceService>();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<IRoomListService, RoomListService>();
builder.Services.AddScoped<IHotelReviewService, HotelReviewService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
