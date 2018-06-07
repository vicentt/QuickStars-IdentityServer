using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        //static void Main(string[] args) => roMainAsync().GetAwaiter().GetResult();
        static void Main(string[] args) => Mai nAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error); Console.ReadLine();
                return;
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error); Console.ReadLine();
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.ReadLine();


            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            { Console.WriteLine(response.StatusCode); }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
              
            }

            Console.ReadLine();
        }

        private static async Task roMainAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000/");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);Console.ReadLine();
                return;
            }
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.Client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("bob", "password", "openid api1 profile");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);Console.ReadLine();
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.ReadLine();

            var HttpClient = new HttpClient();
            HttpClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await HttpClient.GetAsync("http://localhost:5001/identity");
            if(!response.IsSuccessStatusCode)
            { Console.WriteLine(response.StatusCode); }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));

                await GetClaims(tokenResponse.AccessToken);


                Console.ReadLine();
            }
        }

       public static async Task GetClaims(string token)
        {
            var client = new UserInfoClient("http://localhost:5000/connect/userinfo");

            var response = await client.GetAsync(token);
            var identity = response.Claims;

            foreach (var claim in response.Claims)
            {
                Console.WriteLine("{0}\n {1}", claim.Type, claim.Value);
            }
        }
    }
}
