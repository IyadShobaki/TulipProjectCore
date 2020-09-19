using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TulipDataManager.Library.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData(_config);
            return data.GetProducts();

        }

        [HttpPost]
        [Route("UpdateProductQuantity")]
        //public void PutProduct(UpdatedQtyProductModel updatedQtyProduct)
        public void PutProduct(int[] product)
        {
            int productId = product[0];
            int newQuantity = product[1];
            ProductData data = new ProductData(_config);
            data.UpdateProductQuantityInStock(productId, newQuantity);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(List<object> paramsList)
        {

            string productString = paramsList[0].ToString();
            string inventoryString = paramsList[1].ToString();
            //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            //var product = (ProductModel)javaScriptSerializer.Deserialize(productString, typeof(ProductModel));
            //var inventory = (InventoryModel)javaScriptSerializer.Deserialize(inventoryString, typeof(InventoryModel));

            var product = JsonConvert.DeserializeObject<ProductModel>(productString); // Core
            var inventory = JsonConvert.DeserializeObject<InventoryModel>(inventoryString);



            ProductData data = new ProductData(_config);
            data.InsertProductInventory(product, inventory);

        }
        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //public int Post(ProductModel product)
        //{

        //    ProductData data = new ProductData();
        //    return data.InsertProduct(product);

        //}

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[Route("api/Inventory")]
        //public void PostInventory(InventoryModel inventory)
        //{
        //    ProductData data = new ProductData();
        //    data.InsertInventory(inventory);
        //}
    }
}
