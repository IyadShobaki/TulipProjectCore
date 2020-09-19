using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TulipDataManager.Library.Models
{
    public class InventoryDisplayModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int QuantityInStock { get; set; }
        public decimal RetailPrice { get; set; }
    }
}
