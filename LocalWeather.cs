using System.Collections.Generic;
using System.Linq;
using Oxide.Core.Libraries;
using Newtonsoft.Json;
namespace Oxide.Plugins
{
    [Info("Realistic Weather", "chezhead", "1.0.0", ResourceId = 2519)]
    [Description("Calls weather events with console commands.")]
    class RealisticWeather : RustPlugin
    {

		public class WeatherResponse {
			public List<WeatherStatus> weather;
			public WindStatus wind;
			public CloudStatus clouds;
		}
		public class WeatherStatus {
			public int id { get; set; } 
			public string main { get; set; } 
			public string description { get; set; } 
			public string icon { get; set; } 
		}
		public class WindStatus {
			public string speed {get; set; } // m/s
		}
		public class CloudStatus {
			public string all {get; set; } //percent, ie '90' is 90%
		}

        public Timer weatherTimer;
		public string apiKey = "5ec4eea0ef806348ee3198ea183140ad";
		public string city = "Milwaukee";

		public string weatherApiResponse;

        void Unload()
        {
            if (weatherTimer != null)
                weatherTimer.Destroy();
        }

		void Init()
		{
			Puts("Initialized!");
			StartLoop();
		}

        void StartLoop()
        {
			ProcessWeather();
			Puts("Beginning loop");

            if (weatherTimer != null)
                weatherTimer.Destroy();
            weatherTimer = timer.Every(120, () => ProcessWeather());
        }

        void ProcessWeather()
        {
			Puts("Processing weather...");
			int weatherCode = GetWeatherCode();
			ApplyWeather(weatherCode);
			Puts("Processed weather.");
        }

		int GetWeatherCode()
		{
			string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

			Puts($"trying with url {url}");

			webrequest.Enqueue(url, null, (code, response) =>
			{
				if (code != 200 || response == null)
				{
					Puts($"No response");
					return;
				}
				Puts($"Server answered: {response}");
				weatherApiResponse = response;
				WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);  
				Puts($"Weather Code: {weatherResponse.weather[0].id}, Wind Speed: {weatherResponse.wind.speed}, Cloud Coverage: {weatherResponse.clouds.all}%");
			}, this, RequestMethod.GET);
			return 11;
		}

		void ApplyWeather(int weatherCode)
		{
			Puts($"Applying weather with code {weatherCode}");

			

			
		}

		public Dictionary<int, string[]> conditionDict = new Dictionary<int, string[]>()
		{

			// This is all from https://openweathermap.org/weather-conditions

			{200, new string[] {"thunderstorm with light rain", "StormMild"}},
			{201, new string[] {"thunderstorm with rain", "Storm"}},
			{202, new string[] {"thunderstorm with heavy rain", "StormHeavy"}},
			{210, new string[] {"light thunderstorm", "StormMild"}},
			{211, new string[] {"thunderstorm", "Storm"}},
			{212, new string[] {"heavy thunderstorm", "StormHeavy"}},
			{221, new string[] {"ragged thunderstorm", "StormHeavy"}},
			{230, new string[] {"thunderstorm with light drizzle", "StormMild"}},
			{231, new string[] {"thunderstorm with drizzle", "Storm"}},
			{232, new string[] {"thunderstorm with heavy drizzle", "StormHeavy"}},

			{300, new string[] {"light intensity drizzle", "RainMild"}},
			{301, new string[] {"drizzle", "Rain"}},
			{302, new string[] {"heavy intensity drizzle", "RainHeavy"}},
			{310, new string[] {"light intensity drizzle rain", "RainMild"}},
			{311, new string[] {"drizzle rain", "Rain"}},
			{312, new string[] {"heavy intensity drizzle", "RainHeavy"}},
			{313, new string[] {"shower rain and drizzle", "RainMild"}},
			{314, new string[] {"heavy shower rain and drizzle", "RainHeavy"}},
			{321, new string[] {"shower drizzle", "RainMild"}},

			{500, new string[] {"light rain", "RainMild"}},
			{501, new string[] {"moderate rain", "Rain"}},
			{502, new string[] {"heavy intensity rain", "RainHeavy"}},
			{503, new string[] {"very heavy rain", "Storm"}},
			{504, new string[] {"extreme rain", "StormHeavy"}},
			{511, new string[] {"freezing rain", "RainHeavy"}},
			{520, new string[] {"light intensity shower rain", "RainMild"}},
			{521, new string[] {"shower rain", "Rain"}},
			{522, new string[] {"heavy intensity shower rain", "RainHeavy"}},
			{531, new string[] {"ragged shower rain", "RainHeavy"}},

			{600, new string[] {"light snow", "SnowMild"}},
			{601, new string[] {"snow", "Snow"}},
			{602, new string[] {"Heavy snow", "SnowHeavy"}},
			{611, new string[] {"Sleet", "SnowHeavy"}},
			{612, new string[] {"Light shower sleet", "SnowMild"}},
			{613, new string[] {"Shower sleet", "RainMild"}},
			{615, new string[] {"Light rain and snow", "RainMild"}},
			{616, new string[] {"Rain and snow", "Rain"}},
			{620, new string[] {"Light shower snow", "RainMild"}},
			{621, new string[] {"Shower snow", "Snow"}},
			{622, new string[] {"Heavy shower snow", "SnowHeavy"}},
			
			{701, new string[] {"mist", "Mist"}},
			{711, new string[] {"Smoke", "Smoke"}},
			{721, new string[] {"Haze", "Smoke"}},
			{731, new string[] {"sand/ dust whirls", "Sand"}},
			{741, new string[] {"fog", "FogMild"}},
			{751, new string[] {"sand", "Sand"}},
			{761, new string[] {"dust", "Sand"}},
			{762, new string[] {"volcanic ash", "Ash"}},
			{771, new string[] {"squalls", "Storm"}},
			{781, new string[] {"tornado", "StormHeavy"}},

			{800, new string[] {"clear sky", "Clear"}},
			{801, new string[] {"few clouds", "FewClouds"}},
			{802, new string[] {"scattered clouds", "ScatteredClouds"}},
			{803, new string[] {"broken clouds", "BrokenClouds"}},
			{804, new string[] {"overcast clouds", "OvercastClouds"}}
		};

    }
}