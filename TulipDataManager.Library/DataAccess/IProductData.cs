using System.Collections.Generic;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public interface IProductData
    {
        List<ProductModel> GetProducts();
        void InsertProductInventory(ProductModel product, InventoryModel inventory);
        void UpdateProductQuantityInStock(int productId, int newQuantity);
    }
}