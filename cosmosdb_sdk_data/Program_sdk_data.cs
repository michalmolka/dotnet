using System;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string endpointUrl = "https://your-databse-name.documents.azure.com:443/";
            string authorizationKey = "your-authorization-key";
            string databaseName = "iowa_db";
            string containerName = "iowa_sales";

            CosmosClient cosmosClient = new CosmosClient(endpointUrl, authorizationKey);
            Database database = cosmosClient.GetDatabase(databaseName);
            Container container = database.GetContainer(containerName);

            // Point read
            ItemResponse<IowaSales> iowaResponsePointRead = await container.ReadItemAsync<IowaSales>
                (id: "1f34e239-d179-42ea-a1cd-56d131955eac", partitionKey: new PartitionKey("Lyon"));
            IowaSales itemPointRead = iowaResponsePointRead.Resource;
            Console.WriteLine($"Point read: {itemPointRead.City}, {itemPointRead.Address}");

            // SQL query
            var parameterizedQuery = new QueryDefinition(
            query: "SELECT * FROM c where c.CategoryName = @categoryName and c.City = 'West Des Moines'")
            .WithParameter("@categoryName", "Canadian Whiskies"
            );
            using FeedIterator<IowaSales> feed = container.GetItemQueryIterator<IowaSales>(
                parameterizedQuery,
                requestOptions: new QueryRequestOptions { PopulateIndexMetrics = true }
            );

            while (feed.HasMoreResults)
            {
                FeedResponse<IowaSales> response = await feed.ReadNextAsync();
                foreach (IowaSales item in response)
                {
                    Console.WriteLine($"Found item (sql query):\t{item.City}, id: {item.id}");
                }
            }

            // Aggregated query
            string sqlQuery01 = "SELECT count(1) as cnt FROM i where i.CategoryName = 'Canadian Whiskies'";
            QueryDefinition query01 = new QueryDefinition(sqlQuery01);
            using var feed01 = container.GetItemQueryIterator<dynamic>(
                queryDefinition: query01
            );
            var response01 = await feed01.ReadNextAsync();
            dynamic itemCount = new JObject();
            itemCount.cnt = 0;
            itemCount = response01.FirstOrDefault() ?? 0;
            System.Console.WriteLine($"Items count: {itemCount["cnt"]}");

            // Insert
            string jsonText = "{'CountyName': 'Polk','City': 'Lyon', 'StoreNumber': 3762,'VolumeSoldLiters': 15,'id': '88e6b236-6770-4d77-1238-f16f115d60b3'}";
            var iowaJsonObject = JsonConvert.DeserializeObject<IowaSales>(jsonText);
            await container.CreateItemAsync<IowaSales>(item: iowaJsonObject, partitionKey: new PartitionKey(iowaJsonObject.CountyName));

            // Upsert
            ItemResponse<IowaSales> iowaResponseUpsert = await container.ReadItemAsync<IowaSales>(
                id: "88e6b236-6770-4d77-1238-f16f115d60b3", partitionKey: new PartitionKey("Polk")
            );
            IowaSales itemUpsert = iowaResponseUpsert.Resource;
            Console.WriteLine($"Read before change - upsert: {itemUpsert.City}");
            itemUpsert.City = "Lyon-after-upsert";
            await container.UpsertItemAsync<IowaSales>(itemUpsert);
            Console.WriteLine($"Read after change - upsert: {itemUpsert.City}");

            // Replace
            ItemResponse<IowaSales> iowaResponseReplace = await container.ReadItemAsync<IowaSales>(
                id: "88e6b236-6770-4d77-1238-f16f115d60b3", partitionKey: new PartitionKey("Polk")
            );
            IowaSales itemReplace = iowaResponseReplace.Resource;
            string jsonText01 = "{'CountyName': 'Polk', 'City': 'Lyon-replaced', 'StoreNumber': 3762,'VolumeSoldLiters': 15,'id': '88e6b236-6770-4d77-1238-f16f115d60b3',}";
            var iowaJsonObject01 = JsonConvert.DeserializeObject<IowaSales>(jsonText01);
            await container.ReplaceItemAsync(partitionKey: new PartitionKey(iowaJsonObject01.CountyName), id: iowaJsonObject01.id, item: iowaJsonObject01);

            // Remove
            await container.DeleteItemAsync<IowaSales>(id: "88e6b236-6770-4d77-1238-f16f115d60b3", partitionKey: new PartitionKey("Polk"));
            System.Console.WriteLine("Record removed.");
        }
    }
}