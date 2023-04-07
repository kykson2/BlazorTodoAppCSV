using Azure.Storage.Blobs;
using BlazorTodoApp.Shared;
using CsvHelper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using System.IO;
using System.IO.Pipes;
using Microsoft.Azure.Cosmos.Core;
using Microsoft.Extensions.Azure;

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

        public async Task<CSVInfo> GetCSVResult(string fileName)
        {
            CSVInfo csvItem = new();

            IOrderedQueryable<CSVInfo> queryable = container.GetItemLinqQueryable<CSVInfo>();
            var matches = queryable
                .Where(x => x.fileName == fileName).Select(x => x);

            using FeedIterator<CSVInfo> linqFeed = matches.ToFeedIterator();


            while (linqFeed.HasMoreResults)
            {
                FeedResponse<CSVInfo> response = await linqFeed.ReadNextAsync();

                // Iterate query results
                foreach (CSVInfo item in response)
                {
                    csvItem = item;
                }
            }
            return csvItem;
        }


        public async Task UploadCosmosCSV(CSVInfo item, BlobClient blobClient)
        {
            using MemoryStream ms = new();
            await blobClient.DownloadToAsync(ms);
            // ms.Seek(0, SeekOrigin.Begin);
            ms.Position = 0;

            // using StreamReader sr = new(ms);
            using StreamReader reader = new(ms, Encoding.UTF8);
            List<CSVResultInfo> csvResult = new();
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                var records = csv.GetRecords<CSVResultInfo>();
                foreach (var record in records)
                {
                    csvResult.Add(record);
                }
            };
            item.results = csvResult;


            var searchCSV = await GetCSVResult(item.fileName);
            // var target = await container.ReadItemAsync<CSVInfo>(item.Id, new PartitionKey(item.Id));

            if(searchCSV.fileName != null)
            {
                searchCSV.results = csvResult;
                await container.UpsertItemAsync<CSVInfo>(searchCSV, new PartitionKey(searchCSV.Id));
            }
            else
            {
                await container.CreateItemAsync(item, new PartitionKey(item.Id));
            }

            //await blobClient.DownloadToAsync(fileStream);
            //FileInfo fi = new FileInfo(@"C:\Users\Public\Documents\backup.csv");
            //using FileStream fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
            //using StreamReader sr = new StreamReader(fs);
            //int idx = 0;
            //// using StreamReader reader = new(fs, Encoding.UTF8);
            //List<CSVResultInfo> csvResult = new();
            //using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            //{

            //    var records = csv.GetRecords<CSVResultInfo>();
            //    foreach (var record in records)
            //    {
            //        csvResult.Add(record);
            //        idx++;
            //    }
            //};
            //item.results = csvResult;
            //await container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        //public async Task DownloadBlob(CSVInfo item, BlobClient blobClient)
        //{
        //    //FileStream fileStream = File.OpenWrite("/");
        //    // using var fileStream = System.IO.File.OpenWrite(@"C:\Users\Public\Documents\backup.csv");
        //    using MemoryStream ms = new();
        //    await blobClient.DownloadToAsync(ms);
        //    ms.Seek(0, SeekOrigin.Begin);

        //    // using StreamReader sr = new(ms);
        //    using StreamReader reader = new(ms, Encoding.UTF8);
        //    List<CSVResultInfo> csvResult = new();
        //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //    {

        //        var records = csv.GetRecords<CSVResultInfo>();
        //        foreach (var record in records)
        //        {
        //            csvResult.Add(record);
        //        }
        //    };
        //    item.results = csvResult;
        //    await container.CreateItemAsync(item, new PartitionKey(item.Id));
        //    // await blobClient.DownloadToAsync(fileStream);
        //}
    }
}
