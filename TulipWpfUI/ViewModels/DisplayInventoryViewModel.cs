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
    public class DisplayInventoryViewModel : Screen
    {
        private readonly IInventoryEndPoint _inventoryEndPoint;
        private readonly IEventAggregator _events;
        private readonly IWindowManager _window;
        private readonly StatusInfoViewModel _status;

        public DisplayInventoryViewModel(IInventoryEndPoint inventoryEndPoint, IEventAggregator events,
            IWindowManager window, StatusInfoViewModel status)
        {
            _inventoryEndPoint = inventoryEndPoint;
            _events = events;
            _window = window;
            _status = status;
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
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Inventory Form.");
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
                if (InventoryReport?.Count > 0)
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

                if (InventoryReport?.Count > 0 )
                {
                    return false;
                }
                return true;
            }
        }


        private async Task LoadReports()
        {

            var inventoryReport = await _inventoryEndPoint.GetInventoryReport();
            InventoryReport = inventoryReport;
        }

        private List<InventoryDisplayModel> _inventoryReport;

        public List<InventoryDisplayModel> InventoryReport
        {
            get { return _inventoryReport; }
            set
            {
                _inventoryReport = value;
                NotifyOfPropertyChange(() => InventoryReport);
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
