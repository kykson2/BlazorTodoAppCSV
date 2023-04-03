using BlazorTodoApp.Server.Services;
using BlazorTodoApp.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace BlazorFileUpload.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly BlobServerService _blobService;
        private readonly BlobCosmosService _BlobConsmosService;
        private readonly CSVCosmosService _CSVCosmosService;

        public BlobController(BlobServerService blobService, BlobCosmosService blobConsmosService)
        {
            _blobService = blobService;
            _BlobConsmosService = blobConsmosService;

        }

        [HttpGet]
        public string SASKey()
        {
            return _blobService.SASKey();
        }

        [HttpGet("{fileType}")]
        public string CSVSASKey()
        {
            return _blobService.CSVSASKey();
        }


        public string GetBlobUrl(string blobName)
        {
            var result = _blobService.GetBlobUrl(blobName);
            return result;
        }

        [HttpPost]
        public async Task Post(BlobInfo blobInfo)
        {
            blobInfo.id = Guid.NewGuid().ToString();
            await _BlobConsmosService.Post(blobInfo);
        }

        [HttpPost]
        public async Task CSVPost(CSVInfo blobInfo)
        {
            blobInfo.id = Guid.NewGuid().ToString();
            await _CSVCosmosService.Post(blobInfo);
        }

        [HttpDelete("{blobName}")] 
        public void Delete(string blobName)
        {
            _blobService.Delete(blobName);
        }        

    }
}
