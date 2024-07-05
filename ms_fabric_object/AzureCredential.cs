using Azure.Identity;

namespace CsharpObjects
{
    public class AzureCredentialFabric
    {
        private bool readMode;
        private string tenantId;
        private string clientId;
        private string secret;
        public string? token;

        public AzureCredentialFabric(bool readmode, string tenantid, string clientid, string secret)
        {
            this.readMode = readmode;
            this.tenantId = tenantid;
            this.clientId = clientid;
            this.secret = secret;
        }
        private HttpClient CreateClient()
        {

            if (readMode) // Type of a credential we want to use
            {
                var credential = new ClientSecretCredential(tenantId, clientId, secret); // A service principal
                this.token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://analysis.windows.net/powerbi/api/.default" })).Token.ToString();
            }
            else
            { // Authenticate first: az login --scope https://graph.microsoft.com/.default --allow-no-subscriptions or use an Azure credential
                var credential = new ChainedTokenCredential(new AzureCliCredential(), new DefaultAzureCredential());
                this.token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://analysis.windows.net/powerbi/api/.default" })).Token.ToString();
            }

            // An http client with enriched witch a Bearer token.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.token);
            return client;
        }

        public string GetWorkspaces(){
            HttpClient client = CreateClient();
            var workspacesResponse = client.GetStringAsync("https://api.powerbi.com/v1.0/myorg/groups");
            return workspacesResponse.Result;
        }
    }
}
