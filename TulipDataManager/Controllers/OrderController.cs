using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TulipDataManager.Library.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost]
        public int Post(OrderModel order)
        {

            OrderData data = new OrderData(_config);
            return data.InsertOrder(order);

        }

        //[HttpPost]
        //[Route("api/OrderDetail")]
        //public void Post(OrderDetailModel orderDetail)
        //{
        //    OrderData data = new OrderData();
        //    data.InsertOrderDetail(orderDetail);
        //}

        [HttpPost]
        [Route("OrderDetails")]
        public void Post(List<OrderDetailModel> orderDetailModels)
        {

            OrderData data = new OrderData(_config);
            data.InsertOrderDetails(orderDetailModels);

        }

        [HttpPost]
        [Route("DeleteOrder")]
        public void Post(object orderId)
        {
            string _id = orderId.ToString();
            int id = int.Parse(_id);
            OrderData data = new OrderData(_config);
            data.DeleteOrderById(id);

        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetOrdersReport")]
        public List<OrdersReportModel> GetOrdersReport()
        {
            OrderData data = new OrderData(_config);
            return data.GetOrdersReport();
        }
    }
}
