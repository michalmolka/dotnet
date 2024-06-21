using System;
using System.Text;
using System.Text.Json;
using Azure.Identity;

namespace PbiRestApi
{
    internal class Program
    {
        public static string tenantId = "3*****7";
        public static string clientId = "e*****4";
        public static string secret = "Z*****J";
        static void Main(string[] args)
        {
            bool readMode = false;
            string token;

            string workspaceId = "7*****a";
            string datasetId = "0*****4";
            string dataflowId = "d*****f";

            if (readMode) // Type of a credential we want to use
            {
                var credential = new ClientSecretCredential(tenantId, clientId, secret); // A service principal
                token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://analysis.windows.net/powerbi/api/.default" })).Token.ToString();
            }
            else
            { // Authenticate first: az login --scope https://graph.microsoft.com/.default --allow-no-subscriptions  or use an Azure credential
                var credential = new ChainedTokenCredential(new AzureCliCredential(), new DefaultAzureCredential());
                token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://analysis.windows.net/powerbi/api/.default" })).Token.ToString();
            }

            // An http client with enriched witch a Bearer token.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            // Get workspaces
            var workspacesResponse = client.GetStringAsync("https://api.powerbi.com/v1.0/myorg/groups");
            Console.WriteLine(workspacesResponse.Result);

            // Get dataflow sources
            var dataflowSourcesResponse = client.GetStringAsync($"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/dataflows/{dataflowId}/datasources");
            Console.WriteLine(dataflowSourcesResponse.Result);

            // Reports from a workspace
            var reportsResponse = client.GetStringAsync($"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/reports");
            Console.WriteLine(reportsResponse.Result);

            // Refresh a dataflow
            var values01 = JsonSerializer.Serialize(new { notifyOption = "NoNotification" });
            var requestContent01 = new StringContent(values01, Encoding.UTF8, "application/json");
            var refreshDataset = client.PostAsync($"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/dataflows/{dataflowId}/refreshes", requestContent01);
            Console.WriteLine(refreshDataset.Result);

            // Add an admin user to a workspace
            var values02 = JsonSerializer.Serialize(new { emailAddress = "L***@***.com", groupUserAccessRight = "Admin" });
            var requestContent02 = new StringContent(values02, Encoding.UTF8, "application/json");
            var workspaceAddUser = client.PostAsync($"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/users", requestContent02);
            Console.WriteLine(workspaceAddUser.Result);

            // Change a dataset storage mode
            var values03 = JsonSerializer.Serialize(new { targetStorageMode = "PremiumFiles" }); // PremiumFiles <- a large storage mode, Abf <- small datasets
            var requestContent03 = new StringContent(values03, Encoding.UTF8, "application/json");
            var storageModeDataset = client.PatchAsync($"https://api.powerbi.com/v1.0/myorg/datasets/{datasetId}", requestContent03);
            Console.WriteLine(storageModeDataset.Result);
        }
    }
}
