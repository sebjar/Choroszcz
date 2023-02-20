using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperProductLocaleTranslation : ApiObject
    {
        [JsonProperty("translation_id")]
        public int Id { get; set; }
        [JsonProperty("product_id")]
        public int ProductId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("short_description")]
        public string ShortDescription { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
