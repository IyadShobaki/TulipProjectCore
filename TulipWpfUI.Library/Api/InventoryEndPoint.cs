using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public class InventoryEndPoint : IInventoryEndPoint
    {
        private IAPIHelper _apiHelper;

        public InventoryEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<InventoryDisplayModel>> GetInventoryReport()
        {

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/Inventory"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<InventoryDisplayModel>>();
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
