using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Azure;

namespace BlazorTodoApp.Server.Services
{
    public class BlobServiceOptions
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }

    public class BlobService
    {
        private readonly string connectionString;
        private readonly string _defaultContainerName;
        private readonly BlobServiceClient client;
        private readonly HttpClient _httpClient;

        public BlobService(IOptions<BlobServiceOptions> options, HttpClient httpClient)
        {
            connectionString = options.Value.ConnectionString;
            _defaultContainerName = options.Value.ContainerName;
            client = new BlobServiceClient(connectionString);
            _httpClient = httpClient;
        }

        public async Task CreateContainerAsync(string containerName, bool isPublic = false)
        {
            var accessType = isPublic ? PublicAccessType.Blob : PublicAccessType.None;
            var container = client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(accessType);
        }


        public async Task CreateBlobAsync(Stream stream, string blobName)
        {
            var container = client.GetBlobContainerClient(_defaultContainerName);
            var blob = container.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync();
            await container.UploadBlobAsync(blobName, stream);
        }

        public async Task<bool> DeleteBlobAsync(string blobName)
        {
            var container = client.GetBlobContainerClient(_defaultContainerName);
            var blob = container.GetBlobClient(blobName);
            return await blob.DeleteIfExistsAsync();
        }

        public async Task<string> CreateUserDelegationSAS(string containerName, string storedPolicyName)
        {
            string res = "";
            var container = client.GetBlobContainerClient(containerName);

            if (!container.CanGenerateSasUri)
                throw new Exception("Create SAS Fail");

            //string newFileName = $"{DateTime.Now.Ticks.ToString()}_{fileName}";

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                Resource = "c"
            };

            if (string.IsNullOrEmpty(storedPolicyName))
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Create);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            // SAS result
            Uri sasUri = container.GenerateSasUri(sasBuilder);
            Console.WriteLine("SAS URI for blob container is: {0}", sasUri);



            return res;
        }

        public Uri GetServiceSasUriForContainer(string containerName)
        {
            var container = client.GetBlobContainerClient(containerName);
            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (container.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = container.Name,
                    Resource = "c",
                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1);
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobAccountSasPermissions.All);

                Uri sasUri = container.GenerateSasUri(sasBuilder);
                Console.WriteLine("SAS URI for blob container is: {0}", sasUri);

                return sasUri;
            }
            else
            {
                Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
                return null;
            }
        }

        public async Task<List<string>> GetBlobNames(string containerName)
        {
            List<string> blobnames = new();

            var container = client.GetBlobContainerClient(containerName);

            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (container.CanGenerateSasUri)
            {
                var blobs = container.GetBlobsAsync();

                await foreach (var blob in blobs)
                {
                    blobnames.Add(blob.Name);
                }

                return blobnames;
            }
            else
            {
                Console.WriteLine($"container:{containerName} can not have SAS URI");
                return new List<string>();
            }
        }

        public Uri GetContainerSASUri()
        {
            var container = client.GetBlobContainerClient(_defaultContainerName);


            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (container.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = container.Name,
                    Resource = "c"
                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1);
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.All);

                Uri sasUri = container.GenerateSasUri(sasBuilder);

                return sasUri;
            }
            else
            {
                Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");

                return new Uri("");
            }
        }

        public Uri GetBlobSASUri(string containerName, string blobname)
        {
            var container = client.GetBlobContainerClient(containerName);
            var blobclient = container.GetBlobClient(blobname);

            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (blobclient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = container.Name,
                    Resource = "b"
                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1);
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write | BlobContainerSasPermissions.Create);

                Uri sasUri = blobclient.GenerateSasUri(sasBuilder);

                return sasUri;
            }
            else
            {
                Console.WriteLine(@"BlobContainerClient must be authorized with Shared Key 
                          credentials to create a service SAS.");

                return new Uri("");
            }
        }

        public BlobContainerClient GetBlobContainer(string containerName)
        {
            return client.GetBlobContainerClient(containerName);
        }

        public async Task UploadBlob(string fileName, Stream stream)
        {
            await client.GetBlobContainerClient(_defaultContainerName).UploadBlobAsync(fileName, stream);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            //overwrite!! 확인~~
            try
            {
                await client.GetBlobContainerClient(containerName).UploadBlobAsync(blobName, stream);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task UploadBlobByUrlAsync(string containerName, string blobName, string url)
        {
            using var stream = await _httpClient.GetStreamAsync(url);
            await UploadBlobAsync(containerName, blobName, stream);
        }

        public async Task<Stream> GetBlobClientStream(string blobName)
        {
            var container = client.GetBlobContainerClient(_defaultContainerName);
            var blob = container.GetBlobClient(blobName);
            return await blob.OpenReadAsync();
        }

        public async Task<Stream> GetBlobClientStreamAsync(string containerName, string blobName)
        {
            var container = client.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobName);
            return await blob.OpenReadAsync();
        }

        public Uri GetBlobUri(string containerName, string blobName)
        {
            var container = client.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(blobName);
            return blob.Uri;
        }
    }
}