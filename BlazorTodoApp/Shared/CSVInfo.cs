using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodoApp.Shared
{
    public class CSVInfo
    {
        public string id {  get; set; }
        public string fileName { get; set; }
        public BlobClient blobClient { get; set; }
    }

    public class CSVResultInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
