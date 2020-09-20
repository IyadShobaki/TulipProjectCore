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
        private readonly IOrderData _orderData;

        public OrderController(IOrderData orderData)
        {
            _orderData = orderData;
        }

        [HttpPost]
        public int Post(OrderModel order)
        {
            return _orderData.InsertOrder(order);
        }

        [HttpPost]
        [Route("OrderDetails")]
        public void Post(List<OrderDetailModel> orderDetailModels)
        {
            _orderData.InsertOrderDetails(orderDetailModels);
        }

        [HttpPost]
        [Route("DeleteOrder")]
        public void Post(object orderId)
        {
            string _id = orderId.ToString();
            int id = int.Parse(_id);
            _orderData.DeleteOrderById(id);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetOrdersReport")]
        public List<OrdersReportModel> GetOrdersReport()
        {
            return _orderData.GetOrdersReport();
        }
    }
}
