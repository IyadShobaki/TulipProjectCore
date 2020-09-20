using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public class ProductEndPoint : IProductEndPoint
    {
        private IAPIHelper _apiHelper;

        public ProductEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<ProductModel>> GetAll()
        {

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Product"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<ProductModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }


        public async Task<bool> PostProductInventory(ProductModel product, InventoryModel inventory)
        {
            List<object> productInventory = new List<object>
            {
                product,
                inventory
            };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Product", productInventory))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //public async Task UpdateProductQuantity(UpdatedQtyProductModel updatedQtyProduct)
        public async Task UpdateProductQuantity(int productId, int newQuantity)
        {
            int[] product = { productId, newQuantity };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Product/UpdateProductQuantity", product))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

    }
}
