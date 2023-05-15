using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;

namespace Cosmosdb.Http
{
    public static class cosmosdb_func_00
    {
        public class IowaSales
        {
            public string? CountyName { get; set; }
            public string? State { get; set; }
            public int Quantity { get; set; }
            public string? FileName { get; set; }
            public string? OperationId { get; set; }
            public string? id { get; set; }
            public bool isIowa { get; set; }
        };

        [FunctionName("cosmosdb_func_00")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string cn = req.Query["cn"]; // Document county name
            string id = req.Query["id"]; // Document id

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            cn = cn ?? data.cn;
            id = id ?? data.id;

            string endpointUrl = System.Environment.GetEnvironmentVariable("endpointUrl");
            string authorizationKey = System.Environment.GetEnvironmentVariable("authorizationKey");
            string databaseName = System.Environment.GetEnvironmentVariable("databaseName");
            string containerName = System.Environment.GetEnvironmentVariable("containerName");

            CosmosClient cosmosClient = new CosmosClient(endpointUrl, authorizationKey);
            Database database = cosmosClient.GetDatabase(databaseName);
            Container container = database.GetContainer(containerName);

            ItemResponse<IowaSales> iowaResponseUpsert = await container.ReadItemAsync<IowaSales>(
                id: id, partitionKey: new PartitionKey(cn)
            );
            IowaSales itemUpsert = iowaResponseUpsert.Resource;

            if (itemUpsert.CountyName == "Iowa")
            {
                itemUpsert.isIowa = true;
            }
            else
            {
                itemUpsert.isIowa = false;
            }
            await container.UpsertItemAsync<IowaSales>(itemUpsert);

            string responseMessage = string.IsNullOrEmpty(id) || string.IsNullOrEmpty(cn)
                ? "The CountyName and Id value is null"
                : $"Operation completed. CountyName: {cn}, Id: {id}, IsIowa: {itemUpsert.isIowa}";

            return new OkObjectResult(responseMessage);
        }
    }
}
