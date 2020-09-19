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
    public class OrderData
    {
        private readonly IConfiguration _config;

        public OrderData(IConfiguration config)
        {
            _config = config;
        }

        public int InsertOrder(OrderModel order)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);
            int newOrderId = sql.CreateOrder("dbo.spOrder_Insert", order, "TulipData");

            return newOrderId;

        }

        //public void InsertOrderDetail(OrderDetailModel orderDetail)
        //{
        //    SqlDataAccess sql = new SqlDataAccess();
        //    sql.SaveData("dbo.spOrderDetail_Insert", orderDetail, "TulipData");

        //}


        public void InsertOrderDetails(List<OrderDetailModel> orderDetailModels)
        {
            using (SqlDataAccess sql = new SqlDataAccess(_config))
            {
                try
                {

                    sql.StartTransaction("TulipData");

                    foreach (var orderDetailModel in orderDetailModels)
                    {
                        sql.SaveDataInTransaction("dbo.spOrderDetail_Insert", orderDetailModel);
                    }

                    sql.CommitTransaction();  // important
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }

        }

        public void DeleteOrderById(int orderId)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);
            sql.SaveData<dynamic>("dbo.spDeleteOrderById", new { Id = orderId } , "TulipData");
        }

        public List<OrdersReportModel> GetOrdersReport()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData<OrdersReportModel, dynamic>("dbo.spOrder_OrdersReport",
                new { }, "TulipData");

            return output;
        }
    }
}
