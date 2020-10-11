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
        private readonly IProductData _productData;

        public ProductController(IProductData productData)
        {
            _productData = productData;
        }
        [HttpGet]
        [AllowAnonymous]
        public List<ProductModel> Get()
        {
            return _productData.GetProducts();
        }

        [HttpPost]
        [Route("UpdateProductQuantity")]
        public void PutProduct(int[] product)
        {
            int productId = product[0];
            int newQuantity = product[1];
            _productData.UpdateProductQuantityInStock(productId, newQuantity);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(List<object> paramsList)
        {
            string productString = paramsList[0].ToString();
            string inventoryString = paramsList[1].ToString();

            var product = JsonConvert.DeserializeObject<ProductModel>(productString);
            var inventory = JsonConvert.DeserializeObject<InventoryModel>(inventoryString);

            _productData.InsertProductInventory(product, inventory);

        }
    }
}
