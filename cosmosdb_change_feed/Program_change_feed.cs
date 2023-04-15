using Microsoft.Azure.Cosmos;

namespace CosmosDB1;
class Program
{
    static async Task Main(string[] args)
    {
        string endpointUrl = "your-cosmos-db-endpoint";
        string authorizationKey = "your-authorization-key";

        CosmosClient cosmosClient = new CosmosClient(endpointUrl, authorizationKey);
        Database database = cosmosClient.GetDatabase("cosmos_db_iowa");
        Container sourceContainer = database.GetContainer("cosmos_c_iowa");
        Container leaseContainer = database.GetContainer("cosmos_c_lease");
        Container backUpContainer = database.GetContainer("cosmos_c_iowa_backup00");

        ChangeFeedProcessor processor = await StartChangeFeedProcessorAsync();
        Console.ReadKey();
        await processor.StopAsync();

        async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync()
        {
            ChangeFeedProcessor changeFeedProcessor = sourceContainer
                .GetChangeFeedProcessorBuilder<IowaSales>("cosmosBackup", HandleChangesAsync)
                .WithInstanceName("cosmosBackup-01")
                .WithLeaseContainer(leaseContainer)
                .Build();

            Console.WriteLine("Starting Change Feed Processor...");
            await changeFeedProcessor.StartAsync();
            return changeFeedProcessor;
        }

        async Task HandleChangesAsync(
            ChangeFeedProcessorContext context,
            IReadOnlyCollection<IowaSales> changes,
            CancellationToken cancellationToken)
        {
            foreach (IowaSales item in changes)
            {
                await backUpContainer.CreateItemAsync(
                    item,
                    partitionKey: new PartitionKey(item.CountyName)
                );
                Console.WriteLine($"Item added to backup container: {item.id}");
                await Task.Delay(10);
            }
        }
    }
}
