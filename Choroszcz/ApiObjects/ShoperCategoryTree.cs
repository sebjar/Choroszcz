using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperCategoryTree : ApiObject
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("children")]
        public ShoperCategoryTree[] Children { get; set; }
    }
}
