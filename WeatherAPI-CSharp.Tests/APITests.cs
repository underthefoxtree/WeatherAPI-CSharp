using Xunit.Abstractions;

namespace WeatherAPI_CSharp.Tests;

public class APITests
{
	private readonly ITestOutputHelper output;
	private const string apiKey = "YOUR-API-KEY";

	public APITests(ITestOutputHelper output) => this.output = output;

	[Fact]
	public async Task TestGetWeatherCurrentAsync()
	{
		var client = new APIClient(apiKey, true);
		var weather = await client.GetWeatherCurrentAsync("Berlin", true);

		output.WriteLine(weather.ToString());

		Assert.True(weather.Valid);
		Assert.True(weather.AirQuality.Valid);
		Assert.InRange(weather.TemperatureCelsius, -100, 100);
		Assert.NotEmpty(weather.ConditionText);
		Assert.InRange(weather.WindKph, 0, 200);
	}

	[Fact]
	public async Task TestGetWeatherCurrentAsyncFail()
	{
		var client = new APIClient(apiKey, true);
		var weather = await client.GetWeatherCurrentAsync("qwerpoiuqweropiuwqer9872345987");

		output.WriteLine(weather.ToString());

		Assert.False(weather.Valid);
		Assert.False(weather.AirQuality.Valid);
	}

	[Fact]
	public async Task TestGetWeatherForecastDailyAsync()
	{
		const int days = 3;
		var client = new APIClient(apiKey, true);
		var weather = await client.GetWeatherForecastDailyAsync("Berlin", days);

		Assert.Equal(days, weather.Length);

		int index = 0;
		foreach (var forecast in weather)
		{
			output.WriteLine(forecast.ToString());

			Assert.True(forecast.Valid);
			Assert.InRange(forecast.AvgTemperatureCelsius, -100, 100);
			Assert.NotEmpty(forecast.ConditionText);
			Assert.InRange(forecast.MaxWindKph, 0, 200);
			index++;
		}
	}

	[Fact]
	public async Task TestGetWeatherForecastDailyAsyncFail()
	{
		var client = new APIClient(apiKey, true);
		var weather = await client.GetWeatherForecastDailyAsync("qwerpoiuqweropiuwqer9872345987");

		foreach (var forecast in weather)
		{
			output.WriteLine(forecast.ToString());

			Assert.False(forecast.Valid);
		}
	}

	[Fact]
	public async Task TestGetWeatherForecastHourlyAsync()
	{
		const int hours = 24 * 3;
		var client = new APIClient(apiKey, true);
		var weather = await client.GetWeatherForecastHourlyAsync("Berlin", hours);

		Assert.Equal(hours, weather.Length);

		int index = 0;
		foreach (var forecast in weather)
		{
			output.WriteLine(forecast.ToString());

			Assert.True(forecast.Valid);
			Assert.InRange(forecast.TemperatureCelsius, -100, 100);
			Assert.NotEmpty(forecast.ConditionText);
			Assert.InRange(forecast.WindKph, 0, 200);
			index++;
		}
	}

	[Fact]
	public async Task TestGetWeatherForecastHourlyAsyncFail()
	{
		var client = new APIClient(apiKey, true);
		var weather = await client.GetWeatherForecastHourlyAsync("qwerpoiuqweropiuwqer9872345987");

		foreach (var forecast in weather)
		{
			output.WriteLine(forecast.ToString());

			Assert.False(forecast.Valid);
		}
	}

	[Fact]
	public async Task TestGetLocationDataByIp()
	{
		var client = new APIClient(apiKey, true);
		var location = await client.GetLocationDataByIpAsync();

		output.WriteLine(location.ToString());
		Assert.True(location.Valid);
	}

	[Fact]
	public async Task TestGetCurrentDataWithIpLookup()
	{
		var client = new APIClient(apiKey, true);
		var location = await client.GetLocationDataByIpAsync();

		Assert.True(location.Valid);

		var weather = await client.GetWeatherCurrentAsync($"{location.Latitude},{location.Longitude}", true);

		output.WriteLine(weather.ToString());

		Assert.True(weather.Valid);
		Assert.True(weather.AirQuality.Valid);
		Assert.InRange(weather.TemperatureCelsius, -100, 100);
		Assert.NotEmpty(weather.ConditionText);
		Assert.InRange(weather.WindKph, 0, 200);
	}
}