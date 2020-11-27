using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JustEatApi
{
    class Program
    {
        static HttpClient client = new HttpClient();

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
                    try
                    {
                        AddressCheck((JObject)rest);
                        RatingsCheck((JObject)rest);
                        UrlCheck(rest["Id"].ToString(), rest["Url"]?.ToString());

                        // New requirements
                        // 1. Validate that Name Exists
                        if(string.IsNullOrWhiteSpace(rest["Name"]?.ToString()))
                        {
                            Console.WriteLine($"Name is missing for {rest["Id"]}");
                        }

                        // 2. Cuisine Types check
                        CuisineTypeCheck((JObject)rest);

                        // 3. If IsDelivery is true, then DeliveryStartTime should exist
                        if(bool.TryParse(rest["IsDelivery"]?.ToString(), out bool isDelivery) && isDelivery)
                        {
                            if(!DateTime.TryParse(rest["DeliveryStartTime"]?.ToString(), out var deliveryStartTime))
                            {
                                Console.WriteLine($"DeliveryStartTime is missing or not a valid DateTime for {rest["Id"]} even though IsDelivery is true");
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Something failed " + ex);
                    }
                }
            }
        }

        static void CuisineTypeCheck(JObject rest)
        {
            JArray cuisineTypes = rest["CuisineTypes"] as JArray;
            foreach (var ct in cuisineTypes)
            {
                if (ct == null)
                {
                    Console.WriteLine($"Cuisine Types are missing for {rest["Id"]}");
                }

                if (string.IsNullOrEmpty(ct["Id"]?.ToString()))
                {
                    Console.WriteLine($"Id is missing for CuisineType for {rest["Id"]}");
                }

                if (string.IsNullOrEmpty(ct["IsTopCuisine"]?.ToString()))
                {
                    Console.WriteLine($"IsTopCuisine is missing for CuisineType for {rest["Id"]}");
                }

                if (string.IsNullOrEmpty(ct["Name"]?.ToString()))
                {
                    Console.WriteLine($"Name is missing for CuisineType for {rest["Id"]}");
                }

                if (string.IsNullOrEmpty(ct["SeoName"]?.ToString()))
                {
                    Console.WriteLine($"SeoName is missing for CuisineType for {rest["Id"]}");
                }
            }
        }

        static void UrlCheck(string restId, string url)
        {
            if(string.IsNullOrEmpty(url))
            {
                Console.WriteLine($"The url is empty for {restId}");
            }


            try
            {
                var response = client.GetAsync(url).GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"The call for {restId} to {url} failed");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Call failed" + ex);
            }
        }

        static void RatingsCheck(JObject rest)
        {
            if (rest["RatingStars"] == null || rest["NumberOfRatings"] == null)
            {
                Console.WriteLine($"RatingStars or NumberOfRatings is missing for {rest["Id"]}");
            }

            if( int.TryParse(rest["NumberOfRatings"]?.ToString(), out var num) && double.TryParse(rest["RatingStars"]?.ToString(), out var rating))
            {
                if(num > 0 && rating == 0)
                {
                    Console.WriteLine($"Rating Stars for {rest["Id"]} is 0 but number of ratings is greater than 0");
                }
                else if(num == 0 && rating > 0)
                {
                    Console.WriteLine($"Rating Stars for {rest["Id"]} is greater than 0 but number of ratings is 0");
                }
            }
            else
            {
                Console.WriteLine($"Either RatingStars or NumberOfratings could not be parsed for {rest["Id"]}");
            }
        }

        static void AddressCheck(JObject rest)
        {
            JObject address = (JObject)rest["Address"];
            string city = address["City"]?.ToString();
            if (string.IsNullOrEmpty(city))
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
                if (!double.TryParse(latitude, out var lat))
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

            // Console.WriteLine($"Validated {rest["Id"]}"); 
        }
    }
}
