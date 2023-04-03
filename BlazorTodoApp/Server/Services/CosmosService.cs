using BlazorTodoApp.Client.Pages;
using BlazorTodoApp.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BlazorTodoApp.Server.Services
{
    //public record ToDoActivity
    //(

    //     string id ,
    //     string title ,
    //     string todoId 
    //);
    public class CosmosService
    {
        Microsoft.Azure.Cosmos.Container container;
        public CosmosService()
        {
            Connection();
        }

        public async Task Connection()
        {
            string key = "AccountEndpoint=https://blazortodoapp.documents.azure.com:443/;AccountKey=i9ecIL5bGOz19rnN0kBSxderd2oPFRDBVYOzVh2PhuPbCBBB4o2Zix8xUmwGKieiP10xiWsbyFgnACDbqL0FsQ==;";
            string databaseId = "BlazorTodoApp";
            string containerId = "Todo";
            CosmosClient client = new(key);
            container = client.GetContainer(databaseId, containerId);
        }

        public async void Add(TodoItem item)
        {
            await container.CreateItemAsync<TodoItem>(item, new PartitionKey(item.todoId));
            await Get();
        }

        public async Task<TodoItem> Get()
        {
            container.GetItemLinqQueryable<TodoItem>().Where(x => x.ClassType == nameof(TodoItem));

            var TodoItems = new List<TodoItem>();
            IOrderedQueryable<TodoItem> queryable = container.GetItemLinqQueryable<TodoItem>();
            var matches = queryable
                .Select(c => c);

            using FeedIterator<TodoItem> linqFeed = matches.ToFeedIterator();

            while (linqFeed.HasMoreResults)
            {
                FeedResponse<TodoItem> response = await linqFeed.ReadNextAsync();
                int idx = 0;

                // Iterate query results
                foreach (TodoItem item in response)
                {

                    if (idx > 0)
                    {
                        TodoItems[idx - 1].nextTodoId = item.todoId;
                        await container.UpsertItemAsync<TodoItem>(TodoItems[idx - 1], new PartitionKey(TodoItems[idx - 1].todoId) );
                    }
                    TodoItems.Add(item);
                    idx++;
                }
            }

            return TodoItems[0];
        }

        public async Task<TodoItem> Get(string nextTodoItemId)
        {
            var TodoItem = new TodoItem();
            IOrderedQueryable<TodoItem> queryable = container.GetItemLinqQueryable<TodoItem>();
            var matches = queryable
                .Where(x => x.todoId == nextTodoItemId);

            using FeedIterator<TodoItem> linqFeed = matches.ToFeedIterator();

            while (linqFeed.HasMoreResults)
            {
                FeedResponse<TodoItem> response = await linqFeed.ReadNextAsync();


                // Iterate query results
                foreach (TodoItem item in response)
                {
                    TodoItem = item;
                }
            }

            return TodoItem;
        }
    }
}
