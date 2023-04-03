namespace BlazorTodoApp.Client.Services
{
    public class FileUploadClientService
    {
        private readonly HttpClient _http;

        public FileUploadClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task PostFile()
        {
            
        }
    }
}
