using Choroszcz.ApiObjects;
using Choroszcz.ApiRequests;
using Newtonsoft.Json;

namespace Choroszcz
{
    public class ApiTools
    {
        public static int FindCategoryId(IEnumerable<string> categoriesList, ShoperCategoryTree[] CategoriesTree)
        {
            if (categoriesList.Count() == 1)
            {
                return Convert.ToInt32(CategoriesTree.Where(x => x.Name == categoriesList.ElementAt(0)).Select(x => x.Id).FirstOrDefault());
            }
            else
            {
                CategoriesTree = CategoriesTree.Where(x => x.Name == categoriesList.ElementAt(0)).Select(x => x.Children).FirstOrDefault();
                return FindCategoryId(categoriesList.Skip(1), CategoriesTree);
            }
        }

        public static async Task<int> GetLatestProductId(string shopName)
        {
            Program.Print(PrintType.Info, "Getting latest id...");
            ProductsRequestBuilder productsRequestBuilder = new(shopName);

            string response = await (await productsRequestBuilder.HttpClient.GetAsync($"{Descriptors.GetShopUri(shopName)}/webapi/rest/products?limit=1&order=product_id")).Content.ReadAsStringAsync();
            string productId = response[(response.IndexOf("product_id") + 12)..];
            productId = productId[..productId.IndexOf(',')][1..^1];

            Program.Print(PrintType.Success, $"Latest id found: {productId}");

            return Convert.ToInt32(productId);
        }

        public static async Task<ShoperCategoryTree[]> GetExistingCategoriesTree(string shopName)
        {
            Program.Print(PrintType.Info, "Starting categories fetching process...");
            CategoriesTreeRequestBuilder categoriesBuilder = new(shopName);

            if (File.Exists(Program.CategoriesTreeFilePath))
            {
                ShoperCategoryTree[] fileTreeCategories = JsonConvert.DeserializeObject<ShoperCategoryTree[]>(File.ReadAllText(Program.CategoriesTreeFilePath));
                Program.Print(PrintType.Success, "Finished fetching categories from files");
                return fileTreeCategories;
            }

            string response = await (await categoriesBuilder.HttpClient.GetAsync($"{Descriptors.GetShopUri(shopName)}/webapi/rest/categories-tree?limit=50")).Content.ReadAsStringAsync();

            ShoperCategoryTree[] tree = JsonConvert.DeserializeObject<ShoperCategoryTree[]>(response);
            File.WriteAllText(Program.CategoriesTreeFilePath, JsonConvert.SerializeObject(tree));

            Program.Print(PrintType.Success, "Finished fetching categories from files");
            return tree;
        }

        public static async Task<ShoperCategory[]> GetExistingCategories(string shopName)
        {
            Program.Print(PrintType.Info, "Starting categories fetching process...");
            CategoriesRequestBuilder categoriesBuilder = new CategoriesRequestBuilder(shopName);
            List<ShoperCategory> categories = new List<ShoperCategory>();
            string response = await (await categoriesBuilder.HttpClient.GetAsync($"{Descriptors.GetShopUri(shopName)}/webapi/rest/categories?limit=50")).Content.ReadAsStringAsync();

            if (File.Exists(Program.CategoriesFilePath))
            {
                string countJsonPiece = response[(response.IndexOf("count") + 7)..];
                countJsonPiece = countJsonPiece[..countJsonPiece.IndexOf(',')][1..^1];
                ShoperCategory[] fileCategories = JsonConvert.DeserializeObject<ShoperCategory[]>(File.ReadAllText(Program.CategoriesFilePath));
                if (int.Parse(countJsonPiece) == fileCategories.Length)
                {
                    Program.Print(PrintType.Success, "Finished fetching categories from files");
                    return fileCategories;
                }

                fileCategories = null;
            }
            Program.Print(PrintType.Info, "Starting to fetch categories from API");
            response = response[(response.IndexOf("pages") + 7)..];
            for (int i = 1; i <= int.Parse(response[..response.IndexOf(",")]); i++)
            {
                categories.AddRange(await categoriesBuilder.ListAsync(i));
                Thread.Sleep(500);
            }
            
            File.WriteAllText(Program.CategoriesFilePath, JsonConvert.SerializeObject(categories));
            Program.Print(PrintType.Success, "Finished fetching categories from files");
            return categories.ToArray();
        }

        public static async Task<ShoperProduct[]> GetExistingProducts(string shopName)
        {
            Program.Print(PrintType.Info, "Starting products fetching process...");
            ProductsRequestBuilder productsRequestBuilder = new ProductsRequestBuilder(shopName);
            List<ShoperProduct> products = new List<ShoperProduct>();
            string response = await (await productsRequestBuilder.HttpClient.GetAsync($"{Descriptors.GetShopUri(shopName)}/webapi/rest/products?limit=50")).Content.ReadAsStringAsync();

            if (File.Exists(Program.ProductsFilePath))
            {
                string countJsonPiece = response[(response.IndexOf("count") + 7)..];
                countJsonPiece = countJsonPiece[..countJsonPiece.IndexOf(',')][1..^1];
                ShoperProduct[] fileProducts = JsonConvert.DeserializeObject<ShoperProduct[]>(File.ReadAllText(Program.ProductsFilePath));
                if (int.Parse(countJsonPiece) == fileProducts.Length)
                {
                    Program.Print(PrintType.Success, "Finished fetching products from files");
                    return fileProducts;
                }
                fileProducts = null;
            }
            Program.Print(PrintType.Info, "Starting to fetch products from API");
            response = response[(response.IndexOf("pages") + 7)..];
            int c = int.Parse(response[..response.IndexOf(",")]);
            for (int i = 1; i <= c; i++)
            {
                products.AddRange(await productsRequestBuilder.ListAsync(i));
                Program.Print(PrintType.Info, $"{i}/{c}");
            }
                
                
            File.WriteAllText(Program.ProductsFilePath, JsonConvert.SerializeObject(products));
            Program.Print(PrintType.Success, "Finished fetching products from API");
            return products.ToArray();
        }
    }
}
