using Newtonsoft.Json;

namespace Choroszcz.ApiRequests
{
    public class BulkPostBuilder : IDisposable
    {
        public List<string> ApiResponses;
        public HttpClient HttpClient;

        public const string Endpoint = "/webapi/rest/bulk";
        public const string Method = "POST";
        public readonly string AbsoluteEndpoint;

        private int Count;
        private string PostData;
        public BulkPostBuilder(string shopName, bool print = false)
        {
            ApiResponses = new();

            AbsoluteEndpoint = Descriptors.GetShopUri(shopName) + Endpoint;
            HttpClient = new();
            HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Descriptors.GetAccessToken(shopName));
            Count = 0;
            PostData = "[";

            Program.Print(PrintType.Debug, "Initializing BulkPostBuilder for " + shopName);
        }
        public void Add(BulkItem source)
        {
            PostData += JsonConvert.SerializeObject(source) + ",";
            if (++Count == 10) // max items in bulk is 15
                SendAsync().Wait();
        }
        public async Task SendAsync()
        {
            Program.Print(PrintType.Debug, "Sending bulk request...");
            Count = 0;
            string jsonResponse = await (await HttpClient.PostAsync(AbsoluteEndpoint, new StringContent(PostData.Remove(PostData.Length - 1) + "]"))).Content.ReadAsStringAsync();
            PostData = "[";
            Program.Print(PrintType.Debug, "Successfully sent bulk request");

            if (jsonResponse.Contains("{\"errors\":true"))
                Program.Print(PrintType.Error, $"Failure: {jsonResponse}");
            else
                Program.Print(PrintType.Success, "Success");
        }
        public void Dispose()
        {
            if (Count > 0)
                SendAsync().Wait();
            HttpClient.Dispose();
            HttpClient = null;
            Count = default;
            PostData = default;
            Program.Print(PrintType.Debug, "BulkPostBuilder was disposed");
        }

        public async Task Print()
        {
            if (Count > 0)
                SendAsync().Wait();
            foreach (string jsonResponse in ApiResponses)
            {
                if (jsonResponse.Contains("{\"errors\":true"))
                    Program.Print(PrintType.Error, jsonResponse);
                else
                    Program.Print(PrintType.Debug, jsonResponse);
            }

        }

    }
}
