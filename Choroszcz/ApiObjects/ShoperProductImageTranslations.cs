using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperProductImageTranslations
    {
        [JsonProperty("pl_PL")]
        public ShoperProductImageLocaleTranslation Polish { get; set; }
    }
}
