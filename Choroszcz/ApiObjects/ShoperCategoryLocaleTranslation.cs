using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperCategoryLocaleTranslation
    {
        [JsonProperty("trans_id")]
        public int Id { get; set; }
        [JsonProperty("category_id")]
        public int CategoryId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
