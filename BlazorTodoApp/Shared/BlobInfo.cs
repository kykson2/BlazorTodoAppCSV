using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodoApp.Shared
{
    public class BlobInfo
    {
        public string? uri { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
    }
}
