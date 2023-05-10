using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace key_vault;
class Program
{
    static async Task Main(string[] args)
    {
        var credential = new ClientSecretCredential(AzureVariables.tenantId, AzureVariables.clientId, AzureVariables.clientSecret);
        var client = new SecretClient(new Uri(AzureVariables.keyVaultUri), credential);
        var secret = await client.GetSecretAsync("cosmosdb-primary");
        Console.WriteLine($"Your secret is '{secret.Value.Value}'.");
    }
}
