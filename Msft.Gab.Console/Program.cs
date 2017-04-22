using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;

namespace Msft.Gab.ConsoleApp {
    class Program {
        static void Main(string[] args) {

            string url = ConfigurationManager.AppSettings["Url"];

            if (string.IsNullOrEmpty(url)) {
                Console.WriteLine("Please fill in your URL:");
                url = Console.ReadLine();
            }

            Console.WriteLine("Calling url: " + url);

            TestApi(url);
            Console.WriteLine("Done processing, press any key to close....");
            Console.ReadKey();
        }

        private static void TestApi(string url) {

            var authResult = GetToken();
            string token = authResult.AccessToken;
            if (token != null) {

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = client.GetAsync(new Uri(url));

                string content = response.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);
            }
        }

        private static AuthenticationResult GetToken() {

            string aadInstance = "https://login.windows.net/{0}";
            string ResourceId = ConfigurationManager.AppSettings["ResourceId"];
            string tenantId = ConfigurationManager.AppSettings["TenantId"];
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string replyAddress = ConfigurationManager.AppSettings["ReplyAddressConfigured"];
            AuthenticationContext authenticationContext =
              new AuthenticationContext(string.Format(aadInstance, tenantId));

            AuthenticationResult authenticationResult = authenticationContext.AcquireToken(ResourceId, clientId, new Uri(replyAddress), PromptBehavior.RefreshSession);

            return authenticationResult;
        }
    }
}
