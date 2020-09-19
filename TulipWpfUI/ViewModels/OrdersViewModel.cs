using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TulipWpfUI.EventModels;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class OrdersViewModel : Screen
    {
        private readonly IOrderEndPoint _orderEndPoint;
        private readonly IEventAggregator _events;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public OrdersViewModel(IOrderEndPoint orderEndPoint, IEventAggregator events,
            StatusInfoViewModel status, IWindowManager window)
        {
            _orderEndPoint = orderEndPoint;
            _events = events;
            _status = status;
            _window = window;
        }

        protected override async void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            try
            {
             
                await LoadReports();
                
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocationLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Order Form.");
                    await _window.ShowDialogAsync(_status, null, settings);
                    await _events.PublishOnUIThreadAsync(new LogOnEvent());
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                    await _events.PublishOnUIThreadAsync(new LogOnEvent());
                }
            }

        }

        public bool IsAdmin
        {
            get
            {
                if (Orders?.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return !IsAdmin;
                //if (Orders?.Count > 0)
                //{
                //    return false;
                //}
                //return true;
            }
        }
        

        private async Task LoadReports()
        {

            var ordersReportList = await _orderEndPoint.GetOrdersReport();
            Orders = ordersReportList;
        }

        private List<OrdersReportModel> _orders;

        public List<OrdersReportModel> Orders
        {
            get { return _orders; }
            set 
            {
                _orders = value;
                NotifyOfPropertyChange(() => Orders);
                NotifyOfPropertyChange(() => IsAdmin);
                NotifyOfPropertyChange(() => IsEmpty);
            }
        }

       public void BackToProduct()
        {
            _events.PublishOnUIThreadAsync(new LogOnEvent());
        }
    }
}
