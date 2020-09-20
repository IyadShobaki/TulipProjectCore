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
    public class InventoryData : IInventoryData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sql;

        public InventoryData(IConfiguration config, ISqlDataAccess sql)
        {
            _config = config;
            _sql = sql;
        }
        public List<InventoryDisplayModel> GetInventory()
        {
            var output = _sql.LoadData<InventoryDisplayModel, dynamic>("spInventory_GetAll",
                new { }, "TulipData");

            return output;
        }
    }
}
