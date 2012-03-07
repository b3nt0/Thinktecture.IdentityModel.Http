using System;
using System.Net.Http;
using Thinktecture.IdentityModel.Http;
using Thinktecture.IdentityModel.Utility;
using Thinktecture.Samples;
using Thinktecture.Samples.Resources.Data;

namespace BasicAuthenticationClient
{
    class Program
    {
        static Uri _baseAddress = new Uri(Constants.ServiceBaseAddressWebHost);

        static void Main(string[] args)
        {
            while (true)
            {
                Helper.Timer(() =>
                {
                    "Calling Service\n".ConsoleYellow();

                    var client = new HttpClient { BaseAddress = _baseAddress };
                    client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue("alice", "alice");

                    var response = client.GetAsync("identity").Result;
                    response.EnsureSuccessStatusCode();

                    var identity = response.Content.ReadAsAsync<Identity>().Result;
                    identity.ShowConsole();
                });

                Console.ReadLine();
            }
        }
    }
}
