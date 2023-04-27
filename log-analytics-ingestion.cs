using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Ingestion;

// dotnet add package Azure.Identity
// dotnet add package Azure.Monitor.Ingestion

namespace log_analytics_ingestion;
class Program
{
    static async Task Main(string[] args)
    {
        var endpointUri = new Uri(AzureVariables.dataCollectionEndpointUrl);
        var credential = new ClientSecretCredential(AzureVariables.tenantId, AzureVariables.clientId, AzureVariables.clientSecret);
        var client = new LogsIngestionClient(endpointUri, credential);

        var log01 = new
        {
            TimeGenerated = DateTimeOffset.UtcNow,
            AccountName = "cosmosdb",
            RegionName = "West Europe",
            PartitionKey = "Iowa",
            SizeKb = 254,
            DatabaseName = "iowa_db",
            CollectionName = "iowa_sales",
            SourceSystem = ".NET",
            MetaDataType = "CDBPartitionKeyStatistics",
            ResourceId = "3"

        };

        var log02 = new
        {
            TimeGenerated = DateTimeOffset.UtcNow,
            AccountName = "cosmosdb",
            RegionName = "West Europe",
            PartitionKey = "Iowa",
            SizeKb = 156,
            DatabaseName = "iowa_population",
            CollectionName = "iowa_sales",
            SourceSystem = ".NET",
            MetaDataType = "CDBPartitionKeyStatistics",
            ResourceId = "3"
        };
        BinaryData data = BinaryData.FromObjectAsJson(new[] { log01, log02 });

        Response response = await client.UploadAsync(
            AzureVariables.dataCollectionRuleId,
            AzureVariables.dataCollectionStreamName,
            RequestContent.Create(data)).ConfigureAwait(false);

        System.Console.WriteLine("Response status: {0}", response.Status);

    }
}
