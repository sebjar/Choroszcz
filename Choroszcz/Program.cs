using Choroszcz.ApiObjects;
using Choroszcz.ApiRequests;
using System.Text;

namespace Choroszcz
{
    public class Program
    {
        public static string CategoriesTreeFilePath;
        public static string CategoriesFilePath;
        public static string ProductsFilePath;

        private static ApiTools ApiTools;
        private static ShoperCategoryTree[] CategoriesTree;
        private static ShoperCategory[] Categories;
        private static ShoperProduct[] Products;
        public static void Main()
        {
            Run("debug", @"C:\Users\admin\Downloads\plik.csv");
        }
        public static void AddNamesToTree(ShoperCategoryTree[] tree)
        {
            foreach(ShoperCategoryTree node in tree)
            {
                node.Name = Categories.Where(x => x.CategoryId == node.Id).Select(x => x.Translations.Polish.Name).FirstOrDefault();

                if (node.Children.Length > 0)
                {
                    foreach(ShoperCategoryTree child in node.Children)
                    {
                        child.Name = Categories.Where(x => x.CategoryId == child.Id).Select(x => x.Translations.Polish.Name).FirstOrDefault();
                        AddNamesToTree(child.Children);
                    }
                }
            }
        }

        public static void Run(string shopName, string csvPath)
        {
            Setup();
            using CsvParser csvParser = new(csvPath);

            ApiTools = new ApiTools();
            Categories = ApiTools.GetExistingCategories("szkolne").Result;
            CategoriesTree = ApiTools.GetExistingCategoriesTree("szkolne").Result;
            Products = ApiTools.GetExistingProducts("szkolne").Result;
            AddNamesToTree(CategoriesTree);

            int productCount = ApiTools.GetLatestProductId(shopName).Result + 1;
            Scan("szkolne", csvParser);
            List<string> Images = new();
            List<int> categories = new();
            StringBuilder categoriesCsvFile = new();
            categoriesCsvFile.AppendLine("name;categoryId");

            using (BulkPostBuilder bulkPostBuilder = new(shopName, true))
            {
                int randomInt = new Random().Next();
                while (!csvParser.EndOfData)
                {
                    IEnumerable<string> categoriesList = csvParser["category"].Trim().Split(" > ");
                    categories.Add(ApiTools.FindCategoryId(categoriesList, CategoriesTree));
                    int categoryId = categories.Last();
                    string newline = string.Format($"{categoriesList.Last()};{categoryId}");
                    categoriesCsvFile.AppendLine(newline);


                    int producerId = int.Parse(csvParser["producer"]);
                    int categoryIdCsv = int.Parse(csvParser["categoryId"]);
                    string code = csvParser["code"];

                    if(shopName == "debug")
                    {
                        producerId = 1;
                        categoryIdCsv = 1;
                        code += randomInt;
                    }

                    bulkPostBuilder.Add(new BulkItem("/webapi/rest/products", "POST", new ShoperProduct()
                    {
                        ProducerId = producerId,
                        CategoryId = categoryIdCsv,
                        Ean = csvParser["ean"],
                        Code = code,
                        Translations = new ShoperProductTranslations()
                        {
                            Polish = new ShoperProductLocaleTranslation()
                            {
                                Name = csvParser["name"],
                                Description = csvParser["description"].Replace("\n", "<br />"),
                            },
                        },
                        Stock = new ShoperProductStock()
                        {
                            Price = float.Parse(csvParser["price"]),
                        },
                    }));
                    //Images.AddRange(EnumerateImages(@"C:\Users\knut\Downloads\zdjecia", csvParser["code"], csvParser["ean"]));
                    //Images.AddRange(GetAveryImages(csvParser["code"], csvParser["ean"]));
                }
            }
            File.WriteAllText(@"C:\Users\admin\AppData\Roaming\Choroszcz\Categories.csv", categoriesCsvFile.ToString());

            csvParser.Reset();
            using (BulkPostBuilder bulkPostBuilder = new(shopName))
            {
                while (!csvParser.EndOfData)
                {
                    List<ShoperProductImage> productImages = new();
                    foreach (var image in GetAveryImages(csvParser["code"], csvParser["ean"]))
                        productImages.Add(ShoperProductImage.FromSource(productCount + csvParser.Pointer, image));

                    foreach (var productImage in productImages.Where(x => x is not null))
                        bulkPostBuilder.Add(new BulkItem("/webapi/rest/product-images", "POST", productImage));

                    if (productImages.Count < 1)
                        Program.Print(PrintType.Error, $"{csvParser["ean"]} has no image");
                    Program.Print(PrintType.Info, $"{csvParser.Pointer} /  {csvParser.ItemsCount}");
                }
                bulkPostBuilder.Print().Wait();
            }
        }

        private static string[] EnumerateImages(string directory, string code, string ean)
        {
            var childrenDirectories = Directory.EnumerateDirectories(directory).ToArray();
            string[] images = { };
            if(childrenDirectories.Length > 0)
            {
                foreach (var dir in childrenDirectories)
                {
                    EnumerateImages(dir, code, ean);
                }
            }
            else
            {
                var files = Directory.EnumerateFiles(directory).ToArray();

                images = files.Where(x => (x.Contains(directory + @"\" + code.ToLower()) && x.EndsWith(".jpg")) || (x.Contains(directory + @"\" + ean.ToLower()) && x.EndsWith(".png"))).ToArray();
            }
              
            return images;
        }

        private static string[] GetAveryImages(string code, string ean)
        {
            if (Directory.Exists(@"C:\Users\admin\Downloads\zdjecia\" + code))
                return Directory.EnumerateFiles(@"C:\Users\admin\Downloads\zdjecia\" + code).Where(x => x.Contains(ean)).ToArray();
            return Array.Empty<string>();
        }

        private static void Scan(string shopName, CsvParser csvParser)
        {
            List<string> duplicates = new();
            while (!csvParser.EndOfData)
            {
                if (Products.Any(x => x.Code == csvParser["code"]))
                {
                    duplicates.Add(csvParser["code"]);
                    Program.Print(PrintType.Error, csvParser["code"] + " code already exists!");
                    continue;
                }
                if (Products.Any(x => x.Ean == csvParser["ean"]))
                {
                    duplicates.Add(csvParser["ean"]);
                    Program.Print(PrintType.Error, csvParser["ean"] + " ean already exists!");
                    continue;
                }
            }
            csvParser.Reset();         
        }
        private static void Setup()
        {
            Print(PrintType.Info, "Starting settings loading proccess...");
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Choroszcz";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            CategoriesTreeFilePath = directory + "/CategoriesTree.choroszcz";
            CategoriesFilePath = directory + "/Categories.choroszcz";
            ProductsFilePath = directory + "/Products.choroszcz";
            Print(PrintType.Success, "Finished loading settings");
        }
        public static void Print(PrintType type, object data)
        {
            string message = $"<{DateTime.Now.ToShortTimeString()}> {data}";
            switch (type)
            {
                case PrintType.None:
                    break;
                case PrintType.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case PrintType.Info:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case PrintType.Error:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                case PrintType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine();
        
        }
    }

    public enum PrintType
    {
        None,
        Debug,
        Info,
        Error,
        Success,
    }
}