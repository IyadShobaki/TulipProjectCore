using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TulipWpfUI.EventModels;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class ReviewOrderViewModel : Screen
    {

        private readonly IProductEndPoint _productEndPoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUserModel _loggedInUserModel;
        private readonly IOrderEndPoint _orderEndPoint;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public ReviewOrderViewModel(IProductEndPoint productEndPoint,
            IEventAggregator events, ILoggedInUserModel loggedInUserModel,
            IOrderEndPoint orderEndPoint, StatusInfoViewModel status,
            IWindowManager window)
        {
            _productEndPoint = productEndPoint;
            _events = events;
            _loggedInUserModel = loggedInUserModel;
            _orderEndPoint = orderEndPoint;
            _status = status;
            _window = window;
        }
        public static BindingList<ProductViewModel> CartCopy;

        private BindingList<ProductViewModel> _cartTest;

        public BindingList<ProductViewModel> CartTest
        {
            get { return _cartTest; }
            set
            {
                _cartTest = value;
                CartCopy = new BindingList<ProductViewModel>(CartTest);
                NotifyOfPropertyChange(() => CartTest);
            }
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Cart = new BindingList<ProductViewModel>(CartCopy);
            NotifyOfPropertyChange(() => Cart);
        }
        private BindingList<ProductViewModel> _cart;

        public BindingList<ProductViewModel> Cart
        {
            get { return _cart; }
            set 
            { 
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
                NotifyOfPropertyChange(() => TotalSubTotal);
                NotifyOfPropertyChange(() => TotalTax);
                NotifyOfPropertyChange(() => TotalTotal);
            }
        }



        public decimal TotalSubTotal
        {
            get
            {
                return CalculateTotalSubTotal();
            }
        }

        private decimal CalculateTotalSubTotal()
        {
            decimal subTotal = 0;
            if (Cart != null)
            {
                foreach (var item in Cart)
                {
                    subTotal += item.SubTotal;
                }
            }

            return subTotal;
        }
        public decimal TotalTax
        {
            get
            {
                return CalculateTotalTax();
            }

        }

        private decimal CalculateTotalTax()
        {
            decimal taxAmount = 0;
            if (Cart != null)
            {
                foreach (var item in Cart)
                {
                    taxAmount += item.Tax;
                }
            }
           

            return taxAmount;
        }

        public decimal TotalTotal
        {
            get
            {
                decimal total = TotalSubTotal + TotalTax;

                return total;
            }
        }


        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocationLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;


            try
            {

                OrderModel orderModel = new OrderModel
                {
                    UserId = _loggedInUserModel.Id,
                    SubTotal = TotalSubTotal,
                    Tax = TotalTax,
                    Total = TotalTotal
                };

                int orderId = await _orderEndPoint.PostOrderInfo(orderModel);

                List<OrderDetailModel> orderDetailModels = new List<OrderDetailModel>();

                foreach (var item in Cart)
                {
                    OrderDetailModel orderDetailModel = new OrderDetailModel
                    {
                        OrderId = orderId,
                        ProductId = item.Id,
                        Quantity = item.ItemQuantity,
                        PurchasePrice = item.SubTotal,
                        Tax = item.Tax
                    };

                    orderDetailModels.Add(orderDetailModel);
                }
                if (await _orderEndPoint.PostOrderDetailsInfo(orderDetailModels))
                {

                    foreach (var item in Cart)
                    {
                        await _productEndPoint.UpdateProductQuantity(item.Id, (item.QuantityInStock - item.ItemQuantity));
                    }

                    settings.Title = "System Message";
                    _status.UpdateMessage("Thank you for shopping with us!", $"{_loggedInUserModel.FirstName}, your order Submitted Successfully");
                    await _window.ShowDialogAsync(_status, null, settings);

                }
                else
                {
                    await _orderEndPoint.DeleteOrder(orderId);
                    settings.Title = "System Error";
                    _status.UpdateMessage("Error!!!", "Something went wrong! Please try again later");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                // For testing
                // Comment the following line inside OrderDetail table and publish
                // CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY (OrderId) REFERENCES [Order](Id)
                // await _orderEndPoint.DeleteOrder(orderId); // Worked well

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CancelOrder()
        {
            _events.PublishOnUIThreadAsync(new LogOnEvent());
        }
    }
}
