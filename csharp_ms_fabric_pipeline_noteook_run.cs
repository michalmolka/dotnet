using System.Text.Json;
using System.Text;
using Microsoft.Data.SqlClient;

namespace CsharpObjects;
class Program
{
    static void Main(string[] args)
    {
        AzureCredentialFabric acf = new AzureCredentialFabric(false, "3*****7", "e*****4", "Z*****J");
        HttpClient client = acf.CreateClient();

        string workspaceId = "7*****a";
        string pipelineId = "e*****4";
        string notebookId = "1*****c";

        var runPipeline = client.PostAsync($"https://api.fabric.microsoft.com/v1/workspaces/{workspaceId}/items/{pipelineId}/jobs/instances?jobType=Pipeline", null);
        Console.WriteLine(runPipeline.Result);

        var pipelineRunStatus = client.GetStringAsync($"https://api.fabric.microsoft.com/v1/workspaces/{workspaceId}/items/{pipelineId}/jobs/instances/9*****f");
        Console.WriteLine(pipelineRunStatus.Result);

        var runNoteook = client.PostAsync($"https://api.fabric.microsoft.com/v1/workspaces/{workspaceId}/items/{notebookId}/jobs/instances?jobType=RunNotebook", null);
        Console.WriteLine(runNoteook.Result);

        var workspacesResponse = client.GetStringAsync($"https://api.fabric.microsoft.com/v1/workspaces/{workspaceId}/items/{notebookId}/jobs/instances/7*****1");
        Console.WriteLine(workspacesResponse.Result);
    }
}
