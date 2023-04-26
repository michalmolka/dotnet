using Azure.Monitor.Query;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Monitor.Query.Models;

namespace log_analytics;
class Program
{

    public class RUConsumption
    {
        public string? TimeGenerated { get; set; }
        public string? CollectionName { get; set; }
        public string? OperationName { get; set; }
        public long RequestCharge { get; set; }
    }

    static async Task Main(string[] args)
    {
        // AzureVariables... is a separate class where tenantId, clientId, clientSecret are saved as strings
        // Create a credential to aunthenticate to Log Analytics, a Service Principal is used in this case
        var credential = new ClientSecretCredential(AzureVariables.tenantId, AzureVariables.clientId, AzureVariables.clientSecret);
        var logAnalyticsClient = new LogsQueryClient(credential);

        string kqlQuery = """
                CDBPartitionKeyRUConsumption 
            | where DatabaseName  == 'iowa_db' 
            | summarize RequestCharge = sum(RequestCharge) by OperationName, CollectionName, format_datetime(TimeGenerated, 'yyyy-MM-dd') 
            | project TimeGenerated, CollectionName, OperationName, RequestCharge 
            | order by TimeGenerated desc , CollectionName asc, OperationName asc, RequestCharge desc
        """;

        // Built-in type
        Azure.Response<IReadOnlyList<int>> response = await logAnalyticsClient.QueryWorkspaceAsync<int>(
            AzureVariables.logAnalyticsWorkspaceId,
            "CDBControlPlaneRequests | summarize count()",
            new QueryTimeRange(TimeSpan.FromDays(14)));

        System.Console.WriteLine("Response value: {0}", response.Value.First());

        // Response query at class object
        Azure.Response<IReadOnlyList<RUConsumption>> response00 = await logAnalyticsClient.QueryWorkspaceAsync<RUConsumption>(
            AzureVariables.logAnalyticsWorkspaceId,
            kqlQuery,
            new QueryTimeRange(TimeSpan.FromDays(7)));

        foreach (var item1 in response00.Value)
        {
            System.Console.WriteLine(
                $@"Date: {item1.TimeGenerated.ToString()}, Container: {item1.CollectionName}, Operation: {item1.OperationName}, RU: {item1.RequestCharge}");
        }

        Azure.Response<LogsQueryResult> response02 = await logAnalyticsClient.QueryWorkspaceAsync(
            AzureVariables.logAnalyticsWorkspaceId,
            kqlQuery,
            new QueryTimeRange(TimeSpan.FromDays(7)));

        LogsTable table = response02.Value.Table;

        foreach (var row in table.Rows)
        {
            Console.WriteLine(row["TimeGenerated"] + " " + row["CollectionName"] + " " + row["OperationName"] + " " + row["RequestCharge"]);
        }
    }
}
