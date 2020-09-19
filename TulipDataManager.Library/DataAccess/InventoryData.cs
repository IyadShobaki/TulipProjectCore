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
    public class InventoryData
    {
        private readonly IConfiguration _config;

        public InventoryData(IConfiguration config)
        {
            _config = config;
        }
        public List<InventoryDisplayModel> GetInventory()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);
            var output = sql.LoadData<InventoryDisplayModel, dynamic>("spInventory_GetAll",
                new { }, "TulipData");

            return output;
        }
    }
}
