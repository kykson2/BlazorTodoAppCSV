using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Cosmos;
using System.Security.Policy;
using BlazorTodoApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTodoApp.Server.Services
{
    public class BlobServerService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly BlobContainerClient _blobCsvContainerClient;
        readonly string connectionstring = "DefaultEndpointsProtocol=https;AccountName=gwkimlangcode;AccountKey=oEAuQyeTctbqpKg3n9NR3gTGJQfnkIJSO7nAdWMJPv5vbHWzp23c8/4XK9Gmzo8DScYjMPlnaAxP+AStE4beww==;EndpointSuffix=core.windows.net";
        readonly string containerName = "image";
        readonly string csvcontainerName = "csv";

        public BlobServerService()
        {
            _blobServiceClient = new(connectionstring);
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            _blobCsvContainerClient = _blobServiceClient.GetBlobContainerClient(csvcontainerName);
        }

        public string GetSAS()
        {
            DateTimeOffset expireTime = DateTimeOffset.UtcNow.AddHours(1);
            var sasuri = _blobContainerClient.GenerateSasUri(BlobContainerSasPermissions.All, expireTime);
            return sasuri.ToString();
        }
        public string CSVGetSAS()
        {
            DateTimeOffset expireTime = DateTimeOffset.UtcNow.AddHours(1);
            var sasuri = _blobCsvContainerClient.GenerateSasUri(BlobContainerSasPermissions.All, expireTime);
            return sasuri.ToString();
        }

        public string SASKey()
        {
            string containerSAS = GetSAS();

            //BlobContainerClient tempbc = new(new Uri(containerSAS));
            //var blob = tempbc.GetBlobClient("LesserPanda.jpg");

            return containerSAS;
        }
        public string CSVSASKey()
        {
            string containerSAS = CSVGetSAS();

            //BlobContainerClient tempbc = new(new Uri(containerSAS));
            //var blob = tempbc.GetBlobClient("LesserPanda.jpg");
            
            return containerSAS;
        }

        public async Task<List<string>> CSVGet()
        {
            List<string> blobnames = new();
            var blobs = _blobCsvContainerClient.GetBlobsAsync();
            await foreach (var blob in blobs)
            {
                blobnames.Add(blob.Name);
            }

            return blobnames;
        }

        public BlobClient GetCSVBlob(string blobName)
        {
            return _blobCsvContainerClient.GetBlobClient(blobName);
        }

        public string GetBlobUrl(string blobName)
        {
            //_blobContainerClient.GetBlobClient(blobName).Uri.AbsoluteUri;
            // _blobContainerClient.GetBlobClient(blobName).Uri.AbsoluteUri;
            var blob = _blobContainerClient.GetBlobClient(blobName);
            return blob.Uri.ToString();
            //BlobClient blobclient = new(connectionstring, containerName, blobName);
            //_blobContainerClient.GetBlobClient
            //return blobclient.Uri.AbsoluteUri;
        }

        public void Delete(BlobInfo blob)
        {
            _blobContainerClient.DeleteBlob(blob.name);
        }

        public void Delete(string name)
        {
            _blobContainerClient.DeleteBlob(name);
        }

        public async Task CSVBlobDownload(string fileName)
        {
            //BlobClient blobClient = _blobCsvContainerClient.GetBlobClient(fileName);
            //string path = Environment.SpecialFolder.UserProfile + @"\Downloads";

            //using (var stream = new MemoryStream())
            //{
            //    blobClient.DownloadToAsync(stream);
            //    stream.Position = 0;

            //    var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
            //    return File(stream.ToString(), );
            //}
        }
    }
}
