using Choroszcz.ApiObjects;
using Newtonsoft.Json;

namespace Choroszcz.ApiRequests
{
    public class RequestBuilder : IRequestBuilder
    {
        public virtual string Endpoint => "";

        internal readonly string AbsoluteEndpoint;
        internal readonly HttpClient HttpClient;
        public RequestBuilder(string shopName)
        {
            this.AbsoluteEndpoint = Descriptors.GetShopUri(shopName) + Endpoint;
            this.HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Descriptors.GetAccessToken(shopName));
        }
        public void Delete(int id) => HttpClient.DeleteAsync($"{AbsoluteEndpoint}/{id}");
        public async Task<string> DeleteAsync(int id) => await (await HttpClient.DeleteAsync($"{AbsoluteEndpoint}/{id}")).Content.ReadAsStringAsync();
        public async Task<T> GetAsync<T>(int id) => JsonConvert.DeserializeObject<T>(await (await HttpClient.GetAsync($"{AbsoluteEndpoint}/{id}")).Content.ReadAsStringAsync());
        public void Insert(ApiObject source) => HttpClient.PostAsync(AbsoluteEndpoint, new StringContent(JsonConvert.SerializeObject(source)));
        public async Task<string> InsertAsync(ApiObject source) => await (await HttpClient.PostAsync(AbsoluteEndpoint, new StringContent(JsonConvert.SerializeObject(source)))).Content.ReadAsStringAsync();
        public void Update(int id, ApiObject source) => HttpClient.PostAsync($"{AbsoluteEndpoint}/{id}", new StringContent(JsonConvert.SerializeObject(source)));
        public async Task<string> UpdateAsync(int id, ApiObject source) => await (await HttpClient.PostAsync($"{AbsoluteEndpoint}/{id}", new StringContent(JsonConvert.SerializeObject(source)))).Content.ReadAsStringAsync();

        public async Task<T[]> ListAsync<T>(int page = 1)
        {
            string jsonResponse = await (await HttpClient.GetAsync($"{AbsoluteEndpoint}?limit=50&page=" +page)).Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T[]>(jsonResponse[jsonResponse.IndexOf('[')..][..^1]);
        }
    }
}
