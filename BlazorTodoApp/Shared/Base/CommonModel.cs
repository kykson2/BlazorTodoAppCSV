using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodoApp.Shared.Base
{
    public class CommonModel
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("pk")]
        public string Pk { get; set; } = string.Empty;

        public void SetupPk()
        {
            Pk = Id;
        }
    }
}
