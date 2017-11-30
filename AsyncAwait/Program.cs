using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        private static HttpClient _client = new HttpClient();
        private static Dictionary<string, long> _cities = new Dictionary<string, long>();
        private static string APIKEY = "821209c9d22877b5ee22ec35a8614642";
        static Program()
        {
            SetUpCities();
        }

        private static void SetUpCities()
        {
            _cities.Add("Mompos", 3674597);
            _cities.Add("Malaga", 3675605);
            _cities.Add("Mariquita", 3675252);
            _cities.Add("Puerto Chigüiro", 3688452);
            _cities.Add("Leticia", 3676623);
            _cities.Add("Mocoa", 3674654);
        }

        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            Stopwatch s = Stopwatch.StartNew();
            DoSync();
            Console.WriteLine("Elapsed Time: {0} ms", s.ElapsedMilliseconds);
            s.Restart();
            await DoAsync1();
            Console.WriteLine("Elapsed Time: {0} ms", s.ElapsedMilliseconds);
            s.Restart();
            await DoAsync2().ConfigureAwait(false);
            Console.WriteLine("Elapsed Time: {0} ms", s.ElapsedMilliseconds);
            s.Restart();
            Console.Read();
        }

        private static async Task DoAsync2()
        {
            Console.WriteLine("---- ASYNC #2----");
            var weatherData = await Task.WhenAll(_cities.Select(city => GetWeather(city.Value)));
            for (int i = 0; i < weatherData.Length; i++)
            {
                PrintCityWeather(weatherData[i]);
            }
        }

        private static async Task DoAsync1()
        {
            Console.WriteLine("---- ASYNC #1----");
            foreach (var city in _cities)
            {
                var weatherData = await GetWeather(city.Value);
                PrintCityWeather(weatherData);
            }
        }

        private static void DoSync()
        {
            Console.WriteLine("---- SYNC ----");
            foreach (var city in _cities)
            {
                var weatherData = GetWeather(city.Value).Result;
                PrintCityWeather(weatherData);
            }
        }

        private static void PrintCityWeather(WeatherApi.WeatherData weatherData)
        {
            Console.WriteLine($"El clima en: {weatherData.name} es {weatherData.main.temp - 272.15}");
        }


        public static async Task<WeatherApi.WeatherData> GetWeather(long city)
        {
            HttpResponseMessage response = await _client.GetAsync($"http://api.openweathermap.org/data/2.5/weather?id={city}&APPID={APIKEY} ");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<WeatherApi.WeatherData>();
            }
            return null;
        }


        public class WeatherApi
        {
            public class Coord
            {
                public double lon { get; set; }
                public double lat { get; set; }
            }

            public class Weather
            {
                public double id { get; set; }
                public string main { get; set; }
                public string description { get; set; }
                public string icon { get; set; }
            }

            public class Main
            {
                public double temp { get; set; }
                public double pressure { get; set; }
                public double humidity { get; set; }
                public double temp_min { get; set; }
                public double temp_max { get; set; }
            }

            public class Wind
            {
                public double speed { get; set; }
                public double deg { get; set; }
            }

            public class Clouds
            {
                public double all { get; set; }
            }

            public class Sys
            {
                public int type { get; set; }
                public int id { get; set; }
                public double message { get; set; }
                public string country { get; set; }
                public int sunrise { get; set; }
                public int sunset { get; set; }
            }

            public class WeatherData
            {
                public Coord coord { get; set; }
                public List<Weather> weather { get; set; }
                public string @base { get; set; }
                public Main main { get; set; }
                public double visibility { get; set; }
                public Wind wind { get; set; }
                public Clouds clouds { get; set; }
                public int dt { get; set; }
                public Sys sys { get; set; }
                public int id { get; set; }
                public string name { get; set; }
                public int cod { get; set; }
            }
        }
    }
}
