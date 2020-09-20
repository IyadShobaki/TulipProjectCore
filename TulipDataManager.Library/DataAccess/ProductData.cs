using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TulipDataManager.Library.Internal.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public class ProductData : IProductData
    {
        private readonly ISqlDataAccess _sql;

        public ProductData(ISqlDataAccess sql)
        {
            _sql = sql;
        }
        public List<ProductModel> GetProducts()
        {
            var output = _sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "TulipData");

            return output;
        }

        public void InsertProductInventory(ProductModel product, InventoryModel inventory)
        {
            try
            {
                _sql.StartTransaction("TulipData");
                int newProductId = _sql.CreateProductTransaction("dbo.spProduct_Insert", product);

                inventory.ProductId = newProductId;

                _sql.SaveDataInTransaction("dbo.spInventory_Insert", inventory);
                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }

        }

        public void UpdateProductQuantityInStock(int productId, int newQuantity)
        {
            _sql.SaveData<dynamic>("dbo.spProduct_UpdateQuantity",
                new { Id = productId, @QuantityInStock = newQuantity }, "TulipData");
        }
    }
}
