using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperProductStock : ApiObject
    {
        [JsonProperty("price")]
        public float Price { get; set; }
        [JsonProperty("stock")]
        public float Stock { get; set; }
    }
}
