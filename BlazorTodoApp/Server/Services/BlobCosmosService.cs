using BlazorTodoApp.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.ComponentModel;

namespace BlazorTodoApp.Server.Services
{
    public class BlobCosmosService 
    {
        Microsoft.Azure.Cosmos.Container container;
        public BlobCosmosService()
        {
            Connection();
        }

        public async Task Connection()
        {
            string key = "AccountEndpoint=https://blazortodoapp.documents.azure.com:443/;AccountKey=i9ecIL5bGOz19rnN0kBSxderd2oPFRDBVYOzVh2PhuPbCBBB4o2Zix8xUmwGKieiP10xiWsbyFgnACDbqL0FsQ==;";
            string databaseId = "BlazorTodoApp";
            string containerId = "Blob";
            CosmosClient client = new(key);
            container = client.GetContainer(databaseId, containerId);
        }

        public async Task<List<BlobInfo>> Get()
        {
            List<BlobInfo> blobItems = new();

            IOrderedQueryable<BlobInfo> queryable = container.GetItemLinqQueryable<BlobInfo>();
            var matches = queryable
                .Select(c => c);

            using FeedIterator<BlobInfo> linqFeed = matches.ToFeedIterator();

            while (linqFeed.HasMoreResults)
            {
                FeedResponse<BlobInfo> response = await linqFeed.ReadNextAsync();

                // Iterate query results
                foreach (BlobInfo item in response)
                {
                    blobItems.Add(item);
            
                }
            }

            return blobItems;
        }

        public async Task Post(BlobInfo item) 
        {
            item.id = Guid.NewGuid().ToString();
            item.uri = $"https://gwkimlangcode.blob.core.windows.net/image/{item.name}";

            await container.CreateItemAsync(item, new PartitionKey(item.id));
        }

        public async Task Delete(string id)
        {
            await container.DeleteItemAsync<BlobInfo>(id, new PartitionKey(id));
        }

        public async Task Update(BlobInfo item)
        {
            var target = await container.ReadItemAsync<BlobInfo>(item.id, new PartitionKey(item.id));

            if(target == null)
            {
                BlobInfo blobInfo = new()
                {
                    name = item.name,
                };
                await Post(blobInfo);
            }
            else
            {
                var orgblob = target.Resource;
                orgblob.name = item.name;
                orgblob.uri = $"https://gwkimlangcode.blob.core.windows.net/image/{item.name}";

                await container.UpsertItemAsync(orgblob, new PartitionKey(orgblob.id));
            }

        }
    }
}
