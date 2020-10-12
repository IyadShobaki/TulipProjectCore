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
        private readonly IAPIHelper _apiHelper;
        private readonly ReviewOrderViewModel _reviewOrderViewModel;

        public ProductsViewModel(IProductEndPoint productEndPoint, IEventAggregator events,
            ILoggedInUserModel loggedInUserModel, IConfigHelper configHelper,
            IAPIHelper apiHelper, ReviewOrderViewModel reviewOrderViewModel)
        {
            _productEndPoint = productEndPoint;
            _events = events;
            _loggedInUserModel = loggedInUserModel;
            _configHelper = configHelper;
            _apiHelper = apiHelper;
            _reviewOrderViewModel = reviewOrderViewModel;
        }

        protected override async void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            await LoadProducts();
            //await GetUserRole();

        }

        private async Task LoadProducts()
        {

            var productList = await _productEndPoint.GetAll();
            Products = new BindableCollection<ProductViewModel>(productList.Select(x => CreateProductViewModel(x)));
            OriginalList = new List<ProductModel>(productList);
            OriginalProducts = new BindableCollection<ProductViewModel>(Products);
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
            NotifyOfPropertyChange(() => CanReviewOrder);
        }

        private void OnProductAdd(object sender, EventArgs e)
        {
            var productToAdd = (ProductViewModel)sender;

            Cart.Add(productToAdd);
            NotifyOfPropertyChange(() => TotalSubTotal);
            NotifyOfPropertyChange(() => TotalTax);
            NotifyOfPropertyChange(() => TotalTotal);
            NotifyOfPropertyChange(() => CanReviewOrder);
        }

        private string _seacrhProduct;

        public string SearchProduct
        {
            get { return _seacrhProduct; }
            set
            {
                _seacrhProduct = value;

                Products = new BindableCollection<ProductViewModel>(OriginalProducts);

                Products = new BindableCollection<ProductViewModel>(OriginalList.Select(x => CreateProductViewModel(x))
                    .Where(x => x.Description.ToUpper().Contains(value.ToUpper())));


                NotifyOfPropertyChange(() => SearchProduct);
                NotifyOfPropertyChange(() => Products);
            }
        }


        public List<ProductModel> OriginalList { get; set; }
        public BindableCollection<ProductViewModel> OriginalProducts { get; set; }
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
                NotifyOfPropertyChange(() => CanReviewOrder);
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


        public bool CanReviewOrder
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

        public void ReviewOrder()
        {
            _reviewOrderViewModel.CartTest = Cart;
            _events.PublishOnUIThreadAsync(new ReviewOrderEvent());
        }

        public string LoggedInUser
        {
            get
            {
                return _loggedInUserModel.FirstName;
            }
        }

        //private async Task GetUserRole()
        //{
        //    List<string> userRole = await _apiHelper.GetUserId(_loggedInUserModel.Token);

        //    _loggedInUserModel.Role = userRole[1];
        //    NotifyOfPropertyChange(() => IsAdmin);
        //}


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

        public void NotesPage()
        {
            _events.PublishOnUIThreadAsync(new NotesEvent());
        }
    }
}
