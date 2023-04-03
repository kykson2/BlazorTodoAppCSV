using BlazorTodoApp.Server.Services;
using BlazorTodoApp.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTodoApp.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoServerService _TodoItems;
        private readonly CosmosService _CosmosService;

        //public TodoController(TodoServerService todoItems)
        //{
        //    _TodoItems = todoItems;
        //}

        public TodoController(CosmosService cosmosService)
        {
            _CosmosService = cosmosService;
        }

        [HttpGet]
        public async Task<TodoItem> GetTodoList()
        {
            var result = await _CosmosService.Get();
            return result;
        }

        [HttpGet("{nextTodoItemId}")]
        public async Task<TodoItem> GetTodoList(string nextTodoItemId)
        {
            var result = await _CosmosService.Get(nextTodoItemId);
            return result;
        }

        // [HttpPut]
        //public void Put(TodoItem item)
        //{
        //    try
        //    {
        //        _TodoItems.put(item, _TodoItems.TodoItems);
        //        // _TodoItems.TodoItems[item.Id].Title = item.Title;
        //    } catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        [HttpPost] 
        public void Post(TodoItem item)
        {
            // _TodoItems.Post(item, _TodoItems.TodoItems);
            _CosmosService.Add(item);
            //_TodoItems.TodoItems.Add(new TodoItem { Id = _TodoItems.TodoItems.Count + 1, Title= item.Title }); ;
            
        }

        // [HttpDelete("{id}")] 
        //public List<TodoItem> Delete(int id)
        //{
        //    _TodoItems.Delete(id, _TodoItems.TodoItems);
        //    //_TodoItems.TodoItems.RemoveAll(x=>x.Id == id);
        //    return _TodoItems.TodoItems;
        //}
    }
}
