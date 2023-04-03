using Azure.Storage.Blobs;
using BlazorTodoApp.Shared;
using CsvHelper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace BlazorTodoApp.Server.Services
{
    public class CSVCosmosService
    {
        Microsoft.Azure.Cosmos.Container container;
        public CSVCosmosService()
        {
            Connection();
        }

        public async Task Connection()
        {
            string key = "AccountEndpoint=https://blazortodoapp.documents.azure.com:443/;AccountKey=i9ecIL5bGOz19rnN0kBSxderd2oPFRDBVYOzVh2PhuPbCBBB4o2Zix8xUmwGKieiP10xiWsbyFgnACDbqL0FsQ==;";
            string databaseId = "BlazorTodoApp";
            string containerId = "CSV";
            CosmosClient client = new(key);
            container = client.GetContainer(databaseId, containerId);
        }



        public async Task Post(CSVInfo item)
        {
            var result = await DownloadfromStream(item.blobClient);


            item.id = Guid.NewGuid().ToString();
            //item.uri = $"https://gwkimlangcode.blob.core.windows.net/image/{item.name}";

            await container.CreateItemAsync(result, new PartitionKey(item.id));
        }

        public async Task<CSVResultInfo> DownloadfromStream(BlobClient blobClient)
        {
            UTF8Encoding utf8 = new();
            FileStream fileStream = File.OpenWrite("");
            await blobClient.DownloadToAsync(fileStream);

            using var reader = new StreamReader(fileStream, utf8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<CSVResultInfo>();

            return (CSVResultInfo?)records;
        }
    }
}
