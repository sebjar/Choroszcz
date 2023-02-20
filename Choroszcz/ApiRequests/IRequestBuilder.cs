using Choroszcz.ApiObjects;

namespace Choroszcz.ApiRequests
{
    public interface IRequestBuilder
    {
        public string Endpoint { get; }
        public void Delete(int id);
        public Task<string> DeleteAsync(int id);
        public Task<T> GetAsync<T>(int id);
        public void Insert(ApiObject source);
        public Task<string> InsertAsync(ApiObject source);
        public void Update(int id, ApiObject source);
        public Task<string> UpdateAsync(int id, ApiObject source);
        public Task<T[]> ListAsync<T>(int page);
    }
}
