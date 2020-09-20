using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TulipWpfUI.EventModels;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Helpers;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class ProductsViewModel : Screen
    {
        private readonly IProductEndPoint _productEndPoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUserModel _loggedInUserModel;
        private readonly IConfigHelper _configHelper;
        private readonly IOrderEndPoint _orderEndPoint;
        private readonly IAPIHelper _apiHelper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public ProductsViewModel(IProductEndPoint productEndPoint, IEventAggregator events,
            ILoggedInUserModel loggedInUserModel, IConfigHelper configHelper,
            IOrderEndPoint orderEndPoint, IAPIHelper apiHelper, StatusInfoViewModel status,
            IWindowManager window)
        {
            _productEndPoint = productEndPoint;
            _events = events;
            _loggedInUserModel = loggedInUserModel;
            _configHelper = configHelper;
            _orderEndPoint = orderEndPoint;
            _apiHelper = apiHelper;
            _status = status;
            _window = window;
        }

        protected override async void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            await LoadProducts();
            await GetUserRole();

        }

        private async Task LoadProducts()
        {

            var productList = await _productEndPoint.GetAll();
            Products = new BindableCollection<ProductViewModel>(productList.Select(x => CreateProductViewModel(x)));
            NotifyOfPropertyChange(() => Products);
        }

        private ProductViewModel CreateProductViewModel(ProductModel product)
        {

            var productViewModel = new ProductViewModel(product, _configHelper);
            productViewModel.AddTCart += OnProductAdd;
            productViewModel.RemoveFCart += OnProductRemove;
            return productViewModel;
        }

        private void OnProductRemove(object sender, EventArgs e)
        {
            var productToAdd = (ProductViewModel)sender;

            Cart.Remove(productToAdd);
            NotifyOfPropertyChange(() => TotalSubTotal);
            NotifyOfPropertyChange(() => TotalTax);
            NotifyOfPropertyChange(() => TotalTotal);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        private void OnProductAdd(object sender, EventArgs e)
        {
            var productToAdd = (ProductViewModel)sender;

            Cart.Add(productToAdd);
            NotifyOfPropertyChange(() => TotalSubTotal);
            NotifyOfPropertyChange(() => TotalTax);
            NotifyOfPropertyChange(() => TotalTotal);
            NotifyOfPropertyChange(() => CanCheckOut);
        }



        public BindableCollection<ProductViewModel> Products { get; set; } = new BindableCollection<ProductViewModel>();

        private BindingList<ProductViewModel> _cart = new BindingList<ProductViewModel>();

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
                NotifyOfPropertyChange(() => CanCheckOut);
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

            foreach (var item in Cart)
            {
                subTotal += item.SubTotal;
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
            foreach (var item in Cart)
            {
                taxAmount += item.Tax;
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

                OrderModel orderModel = new OrderModel();
                orderModel.UserId = _loggedInUserModel.Id;
                orderModel.SubTotal = TotalSubTotal;
                orderModel.Tax = TotalTax;
                orderModel.Total = TotalTotal;

                int orderId = await _orderEndPoint.PostOrderInfo(orderModel);

                List<OrderDetailModel> orderDetailModels = new List<OrderDetailModel>();

                foreach (var item in Cart)
                {
                    OrderDetailModel orderDetailModel = new OrderDetailModel();
                    orderDetailModel.OrderId = orderId;
                    orderDetailModel.ProductId = item.Id;
                    orderDetailModel.Quantity = item.ItemQuantity;
                    orderDetailModel.PurchasePrice = item.SubTotal;
                    orderDetailModel.Tax = item.Tax;

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

                    await ResetCart();
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

        private async Task ResetCart()
        {
            await LoadProducts();
            Cart = new BindingList<ProductViewModel>();
            NotifyOfPropertyChange(() => Cart);


        }

        public string LoggedInUser
        {
            get
            {
                return _loggedInUserModel.FirstName;
            }
        }

        private async Task GetUserRole()
        {
            List<string> userRole = await _apiHelper.GetUserId(_loggedInUserModel.Token);

            _loggedInUserModel.Role = userRole[1];
            NotifyOfPropertyChange(() => IsAdmin);
        }


        public bool IsAdmin
        {
            get
            {

                bool output = false;
                if (_loggedInUserModel.Role == "Admin")
                {
                    output = true;
                }
                return output;
                //return true; // For testing

            }

        }

        public void AddNewProduct()
        {

            _events.PublishOnUIThreadAsync(new InsertProductsEvent());
        }
        public void OrdersReport()
        {

            _events.PublishOnUIThreadAsync(new OrdersReportEvent());
        }
        public void InventoryReport()
        {

            _events.PublishOnUIThreadAsync(new DisplayInventoryEvent());
        }

        public void UserManagement()
        {

            _events.PublishOnUIThreadAsync(new UserManagementEvent());
        }
    }
}
