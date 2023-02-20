using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperCategoryTranslations
    {
        [JsonProperty("pl_PL")]
        public ShoperCategoryLocaleTranslation Polish { get; set; }
    }
}
