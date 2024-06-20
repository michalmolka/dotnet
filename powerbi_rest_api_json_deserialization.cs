using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;

namespace PbiRestApi
{
    internal class Program
    {
        public class Datasets
        {
            [JsonPropertyName("@odata.context")]
            public string OdataContext { get; set; }
            [JsonPropertyName("value")]
            public List<Dataset> Dataset { get; set; }
        }
        public class Dataset
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("webUrl")]
            public string WebUrl { get; set; }
            [JsonPropertyName("addRowsAPIEnabled")]
            public bool AddRowsAPIEnabled { get; set; }
            [JsonPropertyName("configuredBy")]
            public string ConfiguredBy { get; set; }
            [JsonPropertyName("isRefreshable")]
            public bool IsRefreshable { get; set; }
            [JsonPropertyName("isEffectiveIdentityRequired")]
            public bool IsEffectiveIdentityRequired { get; set; }
            [JsonPropertyName("isEffectiveIdentityRolesRequired")]
            public bool IsEffectiveIdentityRolesRequired { get; set; }
            [JsonPropertyName("isOnPremGatewayRequired")]
            public bool IsOnPremGatewayRequired { get; set; }
            [JsonPropertyName("targetStorageMode")]
            public string TargetStorageMode { get; set; }
            [JsonPropertyName("createdDate")]
            public DateTime CreatedDate { get; set; }
            [JsonPropertyName("createReportEmbedURL")]
            public string CreateReportEmbedURL { get; set; }
            [JsonPropertyName("qnaEmbedURL")]
            public string QnaEmbedURL { get; set; }
            [JsonPropertyName("upstreamDatasets")]
            public List<int> UpstreamDatasets { get; set; }
            [JsonPropertyName("users")]
            public List<string> Users { get; set; }
            [JsonPropertyName("queryScaleOutSettings")]
            public QueryScaleOutSettings QueryScaleOutSettings { get; set; }
        }

        public class QueryScaleOutSettings
        {
            [JsonPropertyName("autoSyncReadOnlyReplicas")]
            public bool AutoSyncReadOnlyReplicas { get; set; }
            [JsonPropertyName("maxReadOnlyReplicas")]
            public int MaxReadOnlyReplicas { get; set; }
        }
        public static string tenantId = "3*****7";
        public static string clientId = "e*****4";
        public static string secret = "Z*****J";
        static void Main(string[] args)
        {
            string workspaceId = "7*****a";
            var credential = new ClientSecretCredential(tenantId, clientId, secret); // A service principal
            string token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://analysis.windows.net/powerbi/api/.default" })).Token.ToString();
   

            // An http client with enriched witch a Bearer token.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Get datasets from a workspace
            var reportsResponse = client.GetStringAsync($"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/datasets");
            string jsonString = reportsResponse.Result;


            // 1: three classes without string operations
            var datasets01 = JsonSerializer.Deserialize<Datasets>(jsonString);
            var iowaDatasets01 = datasets01.Dataset.Where(
                d => d.Name.Contains("iowa", StringComparison.OrdinalIgnoreCase) ||
                d.Name.Contains("eightfive", StringComparison.OrdinalIgnoreCase)).OrderBy(d => d.TargetStorageMode).Reverse();
            
            Console.WriteLine(datasets01.OdataContext);
            foreach (Dataset dataset in iowaDatasets01)
            {
                Console.WriteLine($"Name: {dataset.Name}, Storage mode: {dataset.TargetStorageMode} : {dataset.QueryScaleOutSettings.AutoSyncReadOnlyReplicas}");
            }
            

            // 2: two classes and string operations
            int first = jsonString.IndexOf(@"value"":") + @"value"":".Length;
            string jsonSubstring = jsonString.Substring(first, jsonString.Length - first - 2);

            var datasets02 = JsonSerializer.Deserialize<List<Dataset>>(jsonSubstring);
            var iowaDatasets02 = datasets02.Where(
                d => d.Name.Contains("iowa", StringComparison.OrdinalIgnoreCase) ||
                d.Name.Contains("eightfive", StringComparison.OrdinalIgnoreCase)).OrderBy(d => d.TargetStorageMode).Reverse();

            foreach (Dataset dataset in iowaDatasets02)
            {
                Console.WriteLine($"Name: {dataset.Name}, Storage mode: {dataset.TargetStorageMode} : {dataset.QueryScaleOutSettings.AutoSyncReadOnlyReplicas}");
            };
        }
    }
}