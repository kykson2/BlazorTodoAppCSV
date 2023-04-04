using Microsoft.AspNetCore.Mvc;
using BlazorTodoApp.Server.Services;
using BlazorTodoApp.Shared;

namespace BlazorTodoApp.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlobCosmosController : ControllerBase
    {
        private readonly BlobCosmosService _BlobConsmosService;

        public BlobCosmosController(BlobCosmosService blobCosmosService)
        {
            _BlobConsmosService = blobCosmosService;
        }

        [HttpGet]
        public async Task<List<BlobInfo>> Get(string id)
        {
            var result = await _BlobConsmosService.Get();
            return result;
        }

        [HttpPost]
        public async Task Post(BlobInfo item)
        {
           await _BlobConsmosService.Post(item);
        }



        [HttpPut]
        public async Task Put(BlobInfo item)
        {
            await _BlobConsmosService.Update(item);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _BlobConsmosService.Delete(id);
        }

        public async Task Update(BlobInfo blob)
        {
            await _BlobConsmosService.Update(blob);
        }
    }
}
