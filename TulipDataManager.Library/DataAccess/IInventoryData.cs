using System.Collections.Generic;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryDisplayModel> GetInventory();
    }
}