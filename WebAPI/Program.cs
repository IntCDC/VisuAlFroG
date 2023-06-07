using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Owin.Hosting;
using System.Net.Http;



/*
 * Web API
 * 
 */
namespace WebAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Random generator = new Random();
            String port = generator.Next(49215, 65535).ToString();
            string baseAddress = "http://localhost:" + port + "/";
            Console.WriteLine("LOG Base Address: " + baseAddress);


            // Start OWIN host 
            using (WebApp.Start<StartUp>(url: baseAddress))
            {
                // Create HttpClient and make a request to api/values 
                HttpClient client = new HttpClient();

                var response = client.GetAsync(baseAddress + "api/request").Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }

        }
    }
}
