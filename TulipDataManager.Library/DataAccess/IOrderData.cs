using System.Collections.Generic;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public interface IOrderData
    {
        void DeleteOrderById(int orderId);
        List<OrdersReportModel> GetOrdersReport();
        int InsertOrder(OrderModel order);
        void InsertOrderDetails(List<OrderDetailModel> orderDetailModels);
    }
}