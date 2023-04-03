using BlazorTodoApp.Shared;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;
using System.Net.Sockets;
using BlazorTodoApp.Client.Pages;

namespace BlazorTodoApp.Client.Services
{
    public class TodoService
    {
        private readonly HttpClient _http;
        public TodoService(HttpClient http) => _http = http;


        public async Task<TodoItem> GetTodoList()
        {
            var TodoItem = await _http.GetFromJsonAsync<TodoItem>("Todo");
            return TodoItem;
        }

        public async Task<TodoItem> GetTodoList(string nextTodoItemId)
        {
            var TodoItem = await _http.GetFromJsonAsync<TodoItem>($"Todo/{nextTodoItemId}");
            return TodoItem;
        }

        public async void PutTodoItem(TodoItem SelectTodo)
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync("Todo", SelectTodo);

            if (response.StatusCode.Equals(200))
            {
                await _http.GetFromJsonAsync<List<TodoItem>>("Todo");
            }
        }

        public async void AddTodoItem(TodoItem NewTodo)
        {
            await _http.PostAsJsonAsync("Todo", NewTodo);
        }

        public async Task<List<TodoItem>> DeleteTodoItem(TodoItem todo)
        {
            HttpResponseMessage response = await _http.DeleteAsync($"Todo/{todo.id}");
            var result = await response.Content.ReadFromJsonAsync<List<TodoItem>>();

            return result;
        }
    }
}
