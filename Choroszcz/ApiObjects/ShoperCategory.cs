using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperCategory : ApiObject
    {
        [JsonProperty("category_id")]
        public int CategoryId { get; set; }
        [JsonProperty("translations")]
        public ShoperCategoryTranslations Translations { get; set; }
    }
}
