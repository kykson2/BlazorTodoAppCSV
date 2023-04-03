using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
namespace BlazorTodoApp.Shared
{
    public class TodoItem
    {

        public string ClassType => nameof(TodoItem);
        public string? title { get; set; }
        public bool isDone { get; set; } = false;

        public string? id { get; set; } = Guid.NewGuid().ToString();

        // public PartitionKey pK { get; set; } = new PartitionKey("todoId");
        public string todoId { get; set; } = Guid.NewGuid().ToString();

        public string? nextTodoId { get; set; }
    }
}
