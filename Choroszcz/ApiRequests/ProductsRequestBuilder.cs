using Choroszcz.ApiObjects;

namespace Choroszcz.ApiRequests
{
    public class ProductsRequestBuilder : RequestBuilder
    {
        public override string Endpoint => "/webapi/rest/products";
        public ProductsRequestBuilder(string shopName) : base(shopName) { }

        public async Task<ShoperProduct> GetAsync(int id) => await base.GetAsync<ShoperProduct>(id);  
        public void Insert(ShoperProduct source) => base.Insert(source);
        public async Task<string> InsertAsync(ShoperProduct source) => await base.InsertAsync(source);
        public void Update(int id, ShoperProduct source) => base.Update(id, source);
        public async Task<string> UpdateAsync(int id, ShoperProduct source) => await base.UpdateAsync(id, source);
        public async Task<ShoperProduct[]> ListAsync(int page) => await base.ListAsync<ShoperProduct>(page);
    }
}
