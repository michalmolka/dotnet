using Azure;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using Azure.Identity;

// dotnet add package Azure.Storage.Files.DataLake
// dotnet add package Azure.Identity

namespace adls;
class Program
{
    static async Task Main(string[] args)
    {
        // Create a dataLakeServiceClient
        var credential = new ClientSecretCredential(AzureVariables.tenantId, AzureVariables.clientId, AzureVariables.clientSecret);
        Uri dataLakeEndpoint = new Uri(AzureVariables.adlsEndpoint);
        DataLakeServiceClient dataLakeServiceClient = new DataLakeServiceClient(dataLakeEndpoint, credential);

        // Create a dataLakeFileSystemClient
        string containerName = "iowa-json";
        DataLakeFileSystemClient dataLakeFileSystemClient = dataLakeServiceClient.GetFileSystemClient(containerName);

        // Create a new folder
        DataLakeDirectoryClient dataLakeDirectoryClient00 = await dataLakeFileSystemClient.CreateDirectoryAsync("adls_json_upload");

        System.Console.WriteLine($"Directory: {dataLakeDirectoryClient00.Name} created");

        // Rename a folder
        DataLakeDirectoryClient dataLakeDirectoryClient01 = dataLakeFileSystemClient.GetDirectoryClient("adls_json_upload");
        DataLakeDirectoryClient dataLakeDirectoryClient02 = await dataLakeDirectoryClient01.RenameAsync("adls-json_upload-01");
        System.Console.WriteLine($"Directory {dataLakeDirectoryClient01.Name} has been renamed. New name: {dataLakeDirectoryClient02.Name}");

        // // Delete the directory
        // await dataLakeDirectoryClient02.DeleteAsync();

        // Upload a file
        string fileName = "09.json";
        string fileToUpload = Path.Combine("json_files", fileName);

        DataLakeFileClient fileToUploadClient = await dataLakeDirectoryClient02.CreateFileAsync(fileName);

        FileStream fileStream00 = File.OpenRead(fileToUpload);
        await fileToUploadClient.AppendAsync(fileStream00, offset: 0);
        await fileToUploadClient.FlushAsync(position: fileStream00.Length);

        // List files in a directory
        await foreach (PathItem pathItem in dataLakeDirectoryClient02.GetPathsAsync(recursive: true))
        {
            System.Console.WriteLine($"File: {pathItem.Name}");
        }

        // Download/read a file
        DataLakeFileClient fileToDownload = dataLakeDirectoryClient02.GetFileClient("09.json");
        Response<FileDownloadInfo> downloadResponse  = await fileToDownload.ReadAsync();
        BinaryReader reader = new BinaryReader(downloadResponse.Value.Content);
        FileStream fileStream01 = File.OpenWrite("json_files/09-01.json");

        byte[] buffer = new byte[4096];
        int count;

        while((count = reader.Read(buffer, 0, buffer.Length)) != 0)
        {
            fileStream01.Write(buffer, 0, count);
        }

        await fileStream01.FlushAsync();
        fileStream01.Close();

        // Rename a file and move to another folder - the new folder must exist
        await fileToDownload.RenameAsync("adls-json_upload-02/09-03.json");

        // Delete a file
        // await dataLakeFileSystemClient.DeleteFileAsync("adls-json_upload-01/09.json");
    }
}
