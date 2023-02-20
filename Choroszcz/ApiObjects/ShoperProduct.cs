using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperProduct : ApiObject
    {
        [JsonProperty("product_id")]
        public int Id { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("producer_id")]
        public int? ProducerId { get; set; }
        [JsonProperty("category_id")]
        public int CategoryId { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("ean")]
        public string Ean { get; set; }
        [JsonProperty("related")]
        public int[] Related { get; set; }
        [JsonProperty("options")]
        public int[] Options { get; set; }
        [JsonProperty("stock")]
        public ShoperProductStock Stock { get; set; }
        [JsonProperty("translations")]
        public ShoperProductTranslations Translations { get; set; }
    }
}
