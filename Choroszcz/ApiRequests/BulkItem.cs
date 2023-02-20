using Choroszcz.ApiObjects;
using Newtonsoft.Json;

namespace Choroszcz.ApiRequests
{
    public class BulkItem
    {
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("body")]
        public ApiObject Body { get; set; }

        public BulkItem(string path, string method, ApiObject body)
        {
            Path = path;
            Method = method;
            Body = body;
        }
    }
}
