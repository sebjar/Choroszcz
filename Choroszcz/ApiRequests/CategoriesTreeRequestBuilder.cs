using Choroszcz.ApiObjects;

namespace Choroszcz.ApiRequests
{
    internal class CategoriesTreeRequestBuilder : RequestBuilder
    {
        public override string Endpoint => "/webapi/rest/categories-tree";
        public CategoriesTreeRequestBuilder(string shopName) : base(shopName) { }

        public async Task<ShoperCategoryTree> GetAsync(int id) => await base.GetAsync<ShoperCategoryTree>(id);
        public void Insert(ShoperCategoryTree source) => base.Insert(source);
        public async Task<string> InsertAsync(ShoperCategoryTree source) => await base.InsertAsync(source);
        public void Update(int id, ShoperCategoryTree source) => base.Update(id, source);
        public async Task<string> UpdateAsync(int id, ShoperCategoryTree source) => await base.UpdateAsync(id, source);
        public async Task<ShoperCategoryTree[]> ListAsync(int page) => await base.ListAsync<ShoperCategoryTree>(page);
    }
}
