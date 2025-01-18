using System.Net;
using Newtonsoft.Json;
using WeatherAPI_CSharp.Utils;

/// <summary>
/// Holds all classes needed to make API requests
/// </summary>
namespace WeatherAPI_CSharp;

/// <summary>
/// Client to make requests to weather api
/// </summary>
/// <remarks>
/// Create APIClient with optional <paramref name="useHttps"/> parameter
/// </remarks>
/// <param name="apiKey">Your API key</param>
/// <param name="useHttps"><c>true</c>: Use https, <c>false</c>: Use http</param>
public class APIClient(string apiKey, bool useHttps = false)
{
	private readonly string _apiKey = apiKey;
	private readonly bool _useHttps = useHttps;

	/// <summary>
	/// Get Current weather at <paramref name="query" /> location
	/// </summary>
	/// <param name="query">Weather location</param>
	/// <param name="getAirData">Wether or not to get air quality data</param>
	/// <returns><see cref="Forecast"/> object with data from location</returns>
	/// <remarks>Returns default on http error. In this case, Forecast.Valid will be false.</remarks>
	public async Task<Forecast> GetWeatherCurrentAsync(string query, bool getAirData = false)
	{
		var uri = UrlConstructor.GetCurrentWeatherUri(_apiKey, _useHttps, query, getAirData);

		using var client = new HttpClient();

		try
		{
			var jsonResponse = await client.GetStringAsync(uri);
			dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse)! ?? throw new NullReferenceException();
			return new Forecast(jsonData.current, getAirData);
		}
		catch (HttpRequestException e)
		{
			System.Diagnostics.Debug.WriteLine(e.StatusCode switch
			{
				HttpStatusCode.BadRequest => "Error 400 - Bad Request. Possible query error?",
				HttpStatusCode.Unauthorized => "Error 401 - Unauthorized. Possible API key error?",
				HttpStatusCode.Forbidden => "Error 403 - Forbidden. Possible API key error?",
				HttpStatusCode.NotFound => "Error 404 - Not Found.",
				_ => $"Error {e.StatusCode}"
			});
			return default;
		}
	}

	/// <summary>
	/// Get weather forecast for the next <paramref name="days" /> amount of days at <paramref name="query" /> location
	/// </summary>
	/// <param name="query">Weather location</param>
	/// <param name="days">Amount of days to get the forecast for</param>
	/// <returns>Array of <see cref="ForecastDaily"/> classes</returns>
	/// <remarks>Returns default on http error. In this case, ForecastDaily[0].Valid will be false.</remarks>
	public async Task<ForecastDaily[]> GetWeatherForecastDailyAsync(string query, int days = 3)
	{
		var uri = UrlConstructor.GetForecastWeatherUri(_apiKey, _useHttps, query, days, false);

		using var client = new HttpClient();

		try
		{
			var jsonResponse = await client.GetStringAsync(uri);
			dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse)! ?? throw new NullReferenceException();
			var forecasts = new ForecastDaily[days];
			var index = 0;

			foreach (var dailyData in jsonData.forecast.forecastday)
			{
				forecasts[index] = new ForecastDaily(dailyData);
				index++;
			}

			return forecasts;
		}
		catch (HttpRequestException e)
		{
			System.Diagnostics.Debug.WriteLine(e.StatusCode switch
			{
				HttpStatusCode.BadRequest => "Error 400 - Bad Request. Possible query error?",
				HttpStatusCode.Unauthorized => "Error 401 - Unauthorized. Possible API key error?",
				HttpStatusCode.Forbidden => "Error 403 - Forbidden. Possible API key error?",
				HttpStatusCode.NotFound => "Error 404 - Not Found.",
				_ => $"Error {e.StatusCode}"
			});
			return [default];
		}
	}

	/// <summary>
	/// Get weather forecast for the next <paramref name="hours" /> amount of hours at <paramref name="query" /> location
	/// </summary>
	/// <param name="query">Weather location</param>
	/// <param name="hours">Amount of hours to get the forecast for</param>
	/// <returns>Array of <see cref="ForecastHourly"/> classes</returns>
	/// <remarks>Returns default on http error. In this case, ForecastHourly[0].Valid will be false.</remarks>
	public async Task<ForecastHourly[]> GetWeatherForecastHourlyAsync(string query, int hours = 24)
	{
		var uri = UrlConstructor.GetForecastWeatherUri(_apiKey, _useHttps, query, (int)Math.Ceiling(hours / 24d), false);

		using var client = new HttpClient();

		try
		{
			var jsonResponse = await client.GetStringAsync(uri);
			dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse)! ?? throw new NullReferenceException();
			var forecasts = new ForecastHourly[hours];
			var index = 0;

			foreach (var dailyData in jsonData.forecast.forecastday)
			{
				foreach (var hourlyData in dailyData.hour)
				{
					forecasts[index] = new ForecastHourly(hourlyData);
					index++;
					if (index == hours)
						return forecasts;
				}
			}

			return forecasts;
		}
		catch (HttpRequestException e)
		{
			System.Diagnostics.Debug.WriteLine(e.StatusCode switch
			{
				HttpStatusCode.BadRequest => "Error 400 - Bad Request. Possible query error?",
				HttpStatusCode.Unauthorized => "Error 401 - Unauthorized. Possible API key error?",
				HttpStatusCode.Forbidden => "Error 403 - Forbidden. Possible API key error?",
				HttpStatusCode.NotFound => "Error 404 - Not Found.",
				_ => $"Error {e.StatusCode}"
			});
			return [default];
		}
	}

	/// <summary>
	/// Get the location via the IP endpoint
	/// </summary>
	/// <returns><see cref="LocationData"/> object containing the location</returns>
	public async Task<LocationData> GetLocationDataByIpAsync()
	{
		var uri = UrlConstructor.GetIpLocationUri(_apiKey, _useHttps);

		using var client = new HttpClient();

		try
		{
			var jsonResponse = await client.GetStringAsync(uri);
			dynamic jsonData = JsonConvert.DeserializeObject(jsonResponse)! ?? throw new NullReferenceException();
			return new LocationData(jsonData);
		}
		catch (HttpRequestException e)
		{
			System.Diagnostics.Debug.WriteLine(e.StatusCode switch
			{
				HttpStatusCode.BadRequest => "Error 400 - Bad Request. Possible query error?",
				HttpStatusCode.Unauthorized => "Error 401 - Unauthorized. Possible API key error?",
				HttpStatusCode.Forbidden => "Error 403 - Forbidden. Possible API key error?",
				HttpStatusCode.NotFound => "Error 404 - Not Found.",
				_ => $"Error {e.StatusCode}"
			});
			return default;
		}
	}
}
