using Newtonsoft.Json;

namespace Choroszcz.ApiObjects
{
    public class ShoperProductImage : ApiObject
    {
        [JsonProperty("product_id")]
        public int ProductId { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("translations")]
        public ShoperProductImageTranslations Translations { get; set; }

        public static ShoperProductImage FromSource(int productId, string source, ShoperProductImageTranslations translations = null)
        {
            ShoperProductImage image = new()
            {
                ProductId = productId,
            };
            if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
                image.Url = source;
            else
            {
                if (File.Exists(source))
                    image.Content = Convert.ToBase64String(File.ReadAllBytes(source));
                else
                {
                    Program.Print(PrintType.Error, $"File \"{source}\" does not exist");
                    return null;
                }       
            }
            if (translations != null)
                image.Translations = translations;
            else
                image.Translations = new ShoperProductImageTranslations()
                {
                    Polish = new ShoperProductImageLocaleTranslation()
                    {
                        Name = Uri.IsWellFormedUriString(source, UriKind.Absolute) ? Guid.NewGuid().ToString() : source[(source.LastIndexOf('\\') + 1)..],
                    }
                };
                
            return image;
        }
    }
}
