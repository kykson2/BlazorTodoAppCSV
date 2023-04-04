using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using BlazorTodoApp.Shared;
using System.Text;
using System.Security.Policy;

namespace BlazorTodoApp.Client.Services
{
    public class BlobClientService
    {
        private readonly HttpClient _http;
        private readonly long maxFileSize = long.MaxValue;
        private readonly int maxAllowedFiles = int.MaxValue;
        public BlobClientService(HttpClient http)
        {
            _http = http;
        }

        // cosmos에서 이미지 불러오기
        public async Task<List<BlazorTodoApp.Shared.BlobInfo>> GetBlob()
        {
            var BlobList = await _http.GetFromJsonAsync<List<BlazorTodoApp.Shared.BlobInfo>>("api/BlobCosmos/Get");

            return BlobList;
        }

        // 업로드
        public async Task Upload(InputFileChangeEventArgs e)
        {
            string? containerSAS = await _http.GetStringAsync("api/Blob/SASKey");
            BlobContainerClient container = new(new Uri(containerSAS));

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                // SAS로 토큰을 이용해서 Blob 컨테이너에 업로드
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                await container.UploadBlobAsync(file.Name, fileContent.ReadAsStream());

                var blobObj = new BlazorTodoApp.Shared.BlobInfo() { name = e.File.Name };
                await _http.PostAsJsonAsync("api/BlobCosmos/Post", blobObj);
            }
        }

        public async Task Delete(BlazorTodoApp.Shared.BlobInfo blob)
        {
            await _http.PostAsJsonAsync("api/Blob/Delete", blob);
            await _http.PostAsJsonAsync("api/BlobCosmos/Delete", blob);
        }

        public async Task Update(InputFileChangeEventArgs e, BlazorTodoApp.Shared.BlobInfo blob)
        {
            await _http.DeleteAsync($"api/Blob/Delete/{blob.name}");

            string? containerSAS = await _http.GetStringAsync("api/Blob/SASKey");
            BlobContainerClient container = new(new Uri(containerSAS));
            BlobClient blobClient = container.GetBlobClient(e.File.Name);
            var fileContent = new StreamContent(e.File.OpenReadStream(maxFileSize));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(e.File.ContentType);
            await blobClient.UploadAsync(fileContent.ReadAsStream(), overwrite: true);


            string encodingFileName = System.Web.HttpUtility.UrlEncode($"{e.File.Name}");
            blob.name = encodingFileName;
            await _http.PutAsJsonAsync("api/BlobCosmos/Put", blob);


            //var blobObj = new BlazorTodoApp.Shared.BlobInfo() { uri = e.File.Name, name = encodingFileName };
            // await container.UploadBlobAsync(encodingFileName, fileContent.ReadAsStream());


            //var BlobURL = await _http.GetStringAsync($"api/Blob/GetBlobUrl?blobName={System.Web.HttpUtility.UrlEncode(encodingFileName)}");
        }

        public async Task CSVUpload(InputFileChangeEventArgs e)
        {
            string? containerCSVSAS = await _http.GetStringAsync("api/Blob/CSVSASKey/csv");
            BlobContainerClient container = new(new Uri(containerCSVSAS));
            // BlobClient blobClient = container.GetBlobClient(e.File.Name);
            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                // SAS로 토큰을 이용해서 Blob 컨테이너에 업로드
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                await container.UploadBlobAsync(file.Name, fileContent.ReadAsStream());

                string encodingFileName = System.Web.HttpUtility.UrlEncode($"{file.Name}");
                var csvInfo = new CSVInfo()
                {
                    fileName = encodingFileName
                };
                await _http.PostAsJsonAsync("api/Blob/CSVPost", csvInfo);
            }
        }
    }
}