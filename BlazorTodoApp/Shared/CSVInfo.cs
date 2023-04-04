using Azure.Storage.Blobs;
using BlazorTodoApp.Shared.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodoApp.Shared
{
    public class CSVInfo : CommonModel
    {
        public string? fileName { get; set; }

        public List<CSVResultInfo>? results { get; set; }
    }


    public class CSVResultInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
