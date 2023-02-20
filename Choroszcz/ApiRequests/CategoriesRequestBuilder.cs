using Choroszcz.ApiObjects;

namespace Choroszcz.ApiRequests
{
    internal class CategoriesRequestBuilder : RequestBuilder
    {
        public override string Endpoint => "/webapi/rest/categories";
        public CategoriesRequestBuilder(string shopName) : base(shopName) { }

        public async Task<ShoperCategory> GetAsync(int id) => await base.GetAsync<ShoperCategory>(id);
        public void Insert(ShoperCategory source) => base.Insert(source);
        public async Task<string> InsertAsync(ShoperCategory source) => await base.InsertAsync(source);
        public void Update(int id, ShoperCategory source) => base.Update(id, source);
        public async Task<string> UpdateAsync(int id, ShoperCategory source) => await base.UpdateAsync(id, source);
        public async Task<ShoperCategory[]> ListAsync(int page) => await base.ListAsync<ShoperCategory>(page);
    }
}
