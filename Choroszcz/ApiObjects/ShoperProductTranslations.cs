using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperProductTranslations : ApiObject
    {
        [JsonProperty("pl_PL")]
        public ShoperProductLocaleTranslation Polish { get; set; }
    }
}
