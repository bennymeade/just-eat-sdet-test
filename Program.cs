using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JustEatApi
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformCheck("https://uk.api.just-eat.io/restaurants/bypostcode/e113db").GetAwaiter().GetResult();
        }

        static async Task PerformCheck(string url)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(content);
                JArray restaurants = (JArray)obj["Restaurants"];
                foreach(var rest in restaurants)
                {
                    JObject address = (JObject)rest["Address"];
                    string city = address["City"]?.ToString();
                    if(string.IsNullOrEmpty(city))
                    {
                        Console.WriteLine($"City for {rest["Id"]} is empty");
                    }

                    string firstLine = address["FirstLine"]?.ToString();
                    if (string.IsNullOrEmpty(firstLine))
                    {
                        Console.WriteLine($"FirstLine for {rest["Id"]} is empty");
                    }

                    string postCode = address["Postcode"]?.ToString();
                    if (string.IsNullOrEmpty(postCode))
                    {
                        Console.WriteLine($"Postcode for {rest["Id"]} is empty");
                    }

                    string latitude = address["Latitude"]?.ToString();
                    if (string.IsNullOrEmpty(latitude))
                    {
                        Console.WriteLine($"Latitude for {rest["Id"]} is empty");
                        if(!double.TryParse(latitude, out var lat))
                        {
                            Console.WriteLine($"Latitude {latitude} for {rest["Id"]} is not in the right format");
                        }
                    }

                    string longitude = address["Longitude"]?.ToString();
                    if (string.IsNullOrEmpty(longitude))
                    {
                        Console.WriteLine($"Longitude for {rest["Id"]} is empty");
                        if (!double.TryParse(longitude, out var lat))
                        {
                            Console.WriteLine($"Longitude {longitude} for {rest["Id"]} is not in the right format");
                        }
                    }

                    Console.WriteLine($"Validated {rest["Id"]}");
                }
            }
        }
    }
}