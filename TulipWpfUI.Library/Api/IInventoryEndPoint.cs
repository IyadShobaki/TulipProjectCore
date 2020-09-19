using System.Collections.Generic;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public interface IInventoryEndPoint
    {
        Task<List<InventoryDisplayModel>> GetInventoryReport();
    }
}