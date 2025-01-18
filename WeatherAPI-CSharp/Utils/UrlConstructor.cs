namespace WeatherAPI_CSharp.Utils;

static internal class UrlConstructor
{
	internal static Uri GetCurrentWeatherUri(string apiKey, bool https, string query, bool getAirData)
	{
		return new Uri($"{(https ? "https" : "http")}://api.weatherapi.com/v1/current.json?key={apiKey}&q={query}&aqi={(getAirData ? "yes" : "no")}");
	}

	internal static Uri GetForecastWeatherUri(string apiKey, bool https, string query, int days, bool getAirData)
	{
		return new Uri($"{(https ? "https" : "http")}://api.weatherapi.com/v1/forecast.json?key={apiKey}&q={query}&days={days}&aqi={(getAirData ? "yes" : "no")}");
	}

	internal static Uri GetIpLocationUri(string apiKey, bool https)
	{
		return new Uri($"{(https ? "https" : "http")}://api.weatherapi.com/v1/ip.json?key={apiKey}&q=auto:ip");
	}
}