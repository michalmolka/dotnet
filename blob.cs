using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;

// dotnet add package Azure.Storage.Blobs 
// dotnet add package Azure.Identity

namespace blob;
class Program
{
    static async Task Main(string[] args)
    {
        // Create a BlobServiceClient
        var credential = new ClientSecretCredential(AzureVariables.tenantId, AzureVariables.clientId, AzureVariables.clientSecret);
        Uri blobEndpoint = new Uri(AzureVariables.blobEndpoint);
        BlobServiceClient blobServiceClient = new BlobServiceClient(blobEndpoint, credential);

        // Create a BlobContainerClient
        string containerName = "iowa-json";
        BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

        // Upload a file
        // Specify a file to upload
        string fileName00 = "05.json";
        string fileToUpload = Path.Combine("json_files", fileName00);
        
        // Create a BlobClient for the file
        BlobClient blobClient00 = container.GetBlobClient(fileName00);

        // Upload the file
        var uploadBlob = await blobClient00.UploadAsync(fileToUpload, true);
        System.Console.WriteLine($"Status: {uploadBlob.GetRawResponse().Status}, ReasonPhrase: {uploadBlob.GetRawResponse().ReasonPhrase}");
        
        // List files in a container
        Azure.AsyncPageable<BlobItem> blobContainerList = container.GetBlobsAsync();

        await foreach (BlobItem  item in blobContainerList)
        {
            System.Console.WriteLine($"BLOB name: {item.Name}, Size: {item.Properties.ContentLength} bytes");
        }

        // Download a file from a container
        // Create a BlobClient for a downloaded file
        string fileName01 = "08.json";
        BlobClient blobClient01 = container.GetBlobClient(fileName01);
        // Specify a location for the downloaded file
        string downloadFile = Path.Combine("json_Files", fileName01);
        var downloadBlob = await blobClient01.DownloadToAsync(downloadFile);
        
        // Copy blob inside a container
        BlobClient blobClient02_source = container.GetBlobClient(fileName01);
        BlobClient blobClient02_destination = container.GetBlobClient("this_is_a_copy_" + fileName01);
        await blobClient02_destination.StartCopyFromUriAsync(blobClient02_source.Uri);

        // Remove the file
        // var removeBlob = await blobClient01.DeleteAsync();
    }
}
