using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TulipWpfUI.Library.Helpers;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class ProductViewModel : Screen
    {
        private ProductModel _product;
        private readonly IConfigHelper _configHelper;

        public ProductViewModel(ProductModel product, IConfigHelper configHelper)
        {
            _product = product;
            _configHelper = configHelper;
        }

        public int Id => _product.Id;
        public string ProductName => _product.ProductName;
        public string Description => _product.Description;
        public string ProductImage => _product.ProductImage;
        public decimal RetailPrice => _product.RetailPrice;
        public int QuantityInStock => _product.QuantityInStock;
        public bool IsTaxable => _product.IsTaxable;
        public bool Sex => _product.Sex;

        private int _itemQuantity = 1;

        public int ItemQuantity 
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);

            }
        }

        public string DisplayTaxable
        {
            get
            {
                if (IsTaxable)
                {
                    return "Taxable";
                }
                return "Tax-Free";
            }
        }


        public decimal SubTotal
        {
            get
            {
                return CalculateSubTotal();
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            subTotal += (RetailPrice * ItemQuantity);

            return subTotal;
        }

        public decimal Tax
        {
            get
            {
                if (IsTaxable)
                {
                    return CalculateTax();
                }
                return 0;
            }
        }

        private decimal CalculateTax()
        {
            
            decimal taxRate = _configHelper.GetTaxRate() / 100;

            decimal taxAmount = RetailPrice * ItemQuantity * taxRate;

            return taxAmount;
        }

        public decimal Total
        {
            get
            {
                //decimal total = CalculateSubTotal() + CalculateTax();
                decimal total = SubTotal + Tax;

                return total;
            }
        }

        public event EventHandler AddTCart;

        public event EventHandler RemoveFCart;

        private bool _isAdded = true;

        public bool IsAdded
        {
            get { return _isAdded; }
            set 
            {
                _isAdded = value;
                NotifyOfPropertyChange(() => IsAdded);
                NotifyOfPropertyChange(() => CanAddToCart);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        public bool CanAddToCart
        {          
            get
            {

                bool output = false;
               
                if (ItemQuantity > 0 && ItemQuantity <= QuantityInStock && IsAdded)
                {
                    output = true;
                }

                return output;
            }
        }

        public void AddToCart()
        {
            AddTCart?.Invoke(this, EventArgs.Empty);
            IsAdded = false;
        }

        public bool CanRemoveFromCart
        {
            get
            {

                bool output = false;

                if (IsAdded == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public void RemoveFromCart()
        {
            RemoveFCart?.Invoke(this, EventArgs.Empty);
            IsAdded = true;
        }
    }
}
