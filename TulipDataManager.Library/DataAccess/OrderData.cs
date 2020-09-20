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
    public class OrderData : IOrderData
    {
        private readonly ISqlDataAccess _sql;

        public OrderData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        public int InsertOrder(OrderModel order)
        {
            int newOrderId = _sql.CreateOrder("dbo.spOrder_Insert", order, "TulipData");
            return newOrderId;
        }


        public void InsertOrderDetails(List<OrderDetailModel> orderDetailModels)
        {
            try
            {
                _sql.StartTransaction("TulipData");

                foreach (var orderDetailModel in orderDetailModels)
                {
                    _sql.SaveDataInTransaction("dbo.spOrderDetail_Insert", orderDetailModel);
                }

                _sql.CommitTransaction();  // important
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }


        }

        public void DeleteOrderById(int orderId)
        {
            _sql.SaveData<dynamic>("dbo.spDeleteOrderById", new { Id = orderId }, "TulipData");
        }

        public List<OrdersReportModel> GetOrdersReport()
        {
            var output = _sql.LoadData<OrdersReportModel, dynamic>("dbo.spOrder_OrdersReport",
                new { }, "TulipData");

            return output;
        }
    }
}
