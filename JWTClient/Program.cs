using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JWTClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var tokenEndpoint = new Uri("https://localhost:44329/login");
            var westInitialRequestEndpoint = new Uri("https://localhost:44329/api/values");
            
            var content = "{\"Username\": \"Test@testing.co.uk\", \"Password\": \"Testing1\" }";

            var client = new HttpClient();

            Console.WriteLine($"*** Requesting JWT Token ...");

            var httpResponse = client
                .PostAsync(tokenEndpoint, new StringContent(content,
                    Encoding.UTF8, "application/json")).Result;

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var tokenJson = httpResponse.Content.ReadAsStringAsync().Result;

                dynamic token = JObject.Parse(tokenJson);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"*** Got JWT Token ...");

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", (string)token.token);

                var result = client.GetAsync(westInitialRequestEndpoint).Result;

                switch (result.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unauthorized! Refresh your Bearer token!");

                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Forbidden! You don't have the correct claims");

                        break;
                    case System.Net.HttpStatusCode.OK:
                        var data = result.Content.ReadAsStringAsync().Result;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"*** Data found: {data}");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Not Found! That person was not found in CRM!");

                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"*** Bad Request getting JWT Bearer Token!");
               // Logger.Error($"*** Bad Request getting JWT Bearer Token!");
            }

        }
    }
}
