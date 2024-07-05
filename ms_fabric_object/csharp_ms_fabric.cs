namespace CsharpObjects;
class Program
{
    static void Main(string[] args)
    {
        AzureCredentialFabric acf = new AzureCredentialFabric(true, "3*****7", "e*****4", "Z*****J");
        string workspaceList = acf.GetWorkspaces();
        Console.WriteLine(workspaceList);
    }
}
