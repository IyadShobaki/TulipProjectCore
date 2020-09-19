using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public class OrderEndPoint : IOrderEndPoint
    {
        private IAPIHelper _apiHelper;

        public OrderEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<int> PostOrderInfo(OrderModel order)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Order", order))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    return int.Parse(result);
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //public async Task PostOrderDetailInfos(OrderDetailModel orderDetail)
        //{
        //    using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/OrderDetails", orderDetail))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {

        //        }
        //        else
        //        {
        //            throw new Exception(response.ReasonPhrase);
        //        }
        //    }
        //}

        public async Task<bool> PostOrderDetailsInfo(List<OrderDetailModel> orderDetailModels)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Order/OrderDetails", orderDetailModels))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                    //throw new Exception(response.ReasonPhrase);
                }
            }
        }
        
        public async Task DeleteOrder(int orderId)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Order/DeleteOrder", orderId))
            {
                if (response.IsSuccessStatusCode == false)
                {

                    throw new Exception(response.ReasonPhrase);
                }
            }
        }


        public async Task<List<OrdersReportModel>> GetOrdersReport()
        {

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/Order/GetOrdersReport"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<OrdersReportModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

    }
}
