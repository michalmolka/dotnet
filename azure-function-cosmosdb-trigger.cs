using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Comosdb.Function
{
    
    public class IowaSales
    {
        public string County { get; set; }
        public string State { get; set; }
        public int Quantity { get; set; }
        public string FileName { get; set; }
        public string OperationId { get; set; }
        public string id { get; set; }
        public bool isIowa { get; set; }
        public string OperationType { get; set; }
    };

    public static class azure_function_cosmosdb_trigger
    {
        [FunctionName("azure_function_cosmosdb_trigger")]
        public static async Task Run(
            [CosmosDBTrigger(
                databaseName: "iowa_db",
                collectionName: "iowa_population",
                ConnectionStringSetting = "cosmosdb_DOCUMENTDB",
                LeaseCollectionName = "leases")]
                IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "iowa_db",
                collectionName:"iowa_sales",
                ConnectionStringSetting = "cosmosdb_DOCUMENTDB")] 
                IAsyncCollector<IowaSales> output,
            ILogger log)
        {
            string endpointUrl = System.Environment.GetEnvironmentVariable("endpointUrl");
            string authorizationKey = System.Environment.GetEnvironmentVariable("authorizationKey");
            string databaseName = System.Environment.GetEnvironmentVariable("databaseName");
            string containerName = System.Environment.GetEnvironmentVariable("containerName");

            CosmosClient cosmosClient = new CosmosClient(endpointUrl, authorizationKey);
            Microsoft.Azure.Cosmos.Database database = cosmosClient.GetDatabase(databaseName);
            Container container = database.GetContainer(containerName);
            
            if (input != null && input.Count > 0)
            {
                foreach (var item in input)
                {
                    var jsonDocument = JsonConvert.DeserializeObject<IowaSales>(item.ToString());
                    if (jsonDocument.OperationType is null)
                    {
                        log.LogInformation("The OperationType property is null");
                        jsonDocument.OperationType = "Upsert";
                        await container.UpsertItemAsync(jsonDocument, new Microsoft.Azure.Cosmos.PartitionKey(jsonDocument.County));
                        await output.AddAsync(jsonDocument);
                        log.LogInformation($"Document ID: {jsonDocument.id}");
                    }
                    else
                    {
                        log.LogInformation("Nothing to do.");
                    }
                }
            }
        }
    }
}
