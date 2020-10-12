using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipBlazorUI.Models
{
    public class CartModel
    {
        public int Quantity { get; set; }
        public ProductModel Product { get; set; }
    }
}
