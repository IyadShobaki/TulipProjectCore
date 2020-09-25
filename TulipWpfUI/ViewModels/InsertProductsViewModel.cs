using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TulipWpfUI.EventModels;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class InsertProductsViewModel : Screen
    {
        private readonly IProductEndPoint _productEndPoint;
        private readonly IEventAggregator _events;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly ILoggedInUserModel _loggedInUser;

        public InsertProductsViewModel(IProductEndPoint productEndPoint, IEventAggregator events,
            StatusInfoViewModel status, IWindowManager window, ILoggedInUserModel loggedInUser)
        {
            _productEndPoint = productEndPoint;
            _events = events;
            _status = status;
            _window = window;
            _loggedInUser = loggedInUser;


        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (IsAdmin == false)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocationLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Product Form.");
                _window.ShowDialogAsync(_status, null, settings);
                _events.PublishOnUIThreadAsync(new LogOnEvent());
            }
        }

        public bool IsAdmin
        {
            get
            {
                bool output = false;
                if (_loggedInUser.Role == "Admin")
                {
                    output = true;
                }
                return output;
            }

        }

        private string _productName;

        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                NotifyOfPropertyChange(() => ProductName);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyOfPropertyChange(() => Description);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        public void SelectImage()
        {

            OpenFileDialog dialog = new OpenFileDialog();
            // If you want to show specific files and all files
            //dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"; 
            // Just for specific files
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (dialog.ShowDialog() == true)
            {
                var currentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                //{D:\C#Projects\TulipProjectCore\TulipWpfUI\bin\Debug\netcoreapp3.1\}
                string projectDirectory = currentDirectory.Parent.Parent.Parent.FullName;
                //D:\\C#Projects\\TulipProjectCore\\TulipWpfUI"
                // Create Images folder if not exists
                string destinationFolder = (Directory.CreateDirectory(@$"{projectDirectory}\Images\").ToString());

                string fileName = dialog.FileName;
                //string destinationFolder = @"D:\C#Projects\TulipProjectCore\TulipWpfUI\Images\";

                File.Copy(fileName, $"{destinationFolder}{Path.GetFileName(fileName)}", true);

                //string[] imageName = fileName.Split('\\');
                //ProductImage = $"/Images/{imageName[imageName.Length - 1]}";
                ProductImage = $"/Images/{Path.GetFileName(fileName)}";
                NotifyOfPropertyChange(() => ProductImage);

            }

        }

        private string _productImage;

        public string ProductImage
        {
            get { return _productImage; }
            set
            {
                _productImage = value;
                NotifyOfPropertyChange(() => ProductImage);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private decimal _retailPrice;

        public decimal RetailPrice
        {
            get { return _retailPrice; }
            set
            {
                _retailPrice = value;
                NotifyOfPropertyChange(() => RetailPrice);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private int _quantityInStock;

        public int QuantityInStock
        {
            get { return _quantityInStock; }
            set
            {
                _quantityInStock = value;
                NotifyOfPropertyChange(() => QuantityInStock);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }
        private bool _sex;

        public bool Sex
        {
            get { return _sex; }
            set
            {
                _sex = value;
                NotifyOfPropertyChange(() => Sex);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private decimal _purchasePrice;

        public decimal PurchasePrice
        {
            get { return _purchasePrice; }
            set
            {
                _purchasePrice = value;
                NotifyOfPropertyChange(() => PurchasePrice);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private DateTime _purchaseDate = DateTime.Now;

        public DateTime PurchaseDate
        {
            get { return _purchaseDate; }
            set
            {
                _purchaseDate = value;
                NotifyOfPropertyChange(() => PurchaseDate);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private int _totalQuantity;

        public int TotalQuantity
        {
            get { return _totalQuantity; }
            set
            {
                _totalQuantity = value;
                NotifyOfPropertyChange(() => TotalQuantity);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }


        public bool IsErrorVisible
        {
            get
            {
                bool output = false;
                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }

        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool CanSubmit
        {
            get
            {

                bool output = false;

                if (ProductName?.Length > 0 && Description?.Length > 0
                    && ProductImage?.Length > 0 && RetailPrice > 0
                    && QuantityInStock > 0 && TotalQuantity > 0
                    && PurchasePrice > 0)
                {
                    output = true;
                }
                return output;

            }
        }
        public async Task Submit()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocationLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;

            try
            {
                // Use transactions to commit the following to database

                ProductModel product = new ProductModel();
                product.ProductName = ProductName;
                product.Description = Description;
                product.ProductImage = ProductImage;
                product.RetailPrice = RetailPrice;
                product.QuantityInStock = QuantityInStock;
                product.Sex = Sex;


                InventoryModel inventory = new InventoryModel();
                inventory.PurchasePrice = PurchasePrice;
                inventory.Quantity = TotalQuantity;
                inventory.PurchaseDate = PurchaseDate;

                if (await _productEndPoint.PostProductInventory(product, inventory))//using transaction
                {
                    settings.Title = "System Message";
                    _status.UpdateMessage($"{ProductName}", "Product Information Inserted successfully!");
                    await _window.ShowDialogAsync(_status, null, settings);

                    ResetFields();
                }
                else
                {
                    settings.Title = "System Error";
                    _status.UpdateMessage("Error Inserting Product", "Something went wrong! Please try again later");
                    await _window.ShowDialogAsync(_status, null, settings);
                }



            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }




        public void MainPage()
        {
            _events.PublishOnUIThreadAsync(new LogOnEvent());
        }
        public void ResetFields()
        {
            ProductName = "";
            Description = "";
            ProductImage = "";
            TotalQuantity = 0;
            PurchasePrice = 0;
            RetailPrice = 0;
            QuantityInStock = 0;
        }
    }
}
