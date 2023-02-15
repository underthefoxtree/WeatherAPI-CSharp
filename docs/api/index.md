# WeatherAPI CSharp

## Getting Started
To get started, you can take a look at the minimal setup down below.

```csharp
// Program.cs
using WeatherAPI_CSharp;

var client = new APIClient("YOUR-API-KEY", true);

var weather = await client.GetWeatherCurrentAsync("Berlin");
Console.WriteLine($"The temperature in Berlin is {weather.TemperatureCelsius}C at a wind speed of {weather.WindKph}km/h");
```
Information about APIClient can be found [here](https://underthefoxtree.github.io/WeatherAPI-CSharp/api/WeatherAPI_CSharp.APIClient.html).