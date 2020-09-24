using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TulipWpfUI.EventModels;
using TulipWpfUI.Library.Api;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>, IHandle<RegisterEvent>,
        IHandle<LogInEvent>, IHandle<InsertProductsEvent>, IHandle<OrdersReportEvent>,
        IHandle<DisplayInventoryEvent>, IHandle<UserManagementEvent>,
        IHandle<ReviewOrderEvent>
    {

        private IEventAggregator _events;
        private readonly ILoggedInUserModel _user;
        private readonly IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events,
                              ILoggedInUserModel user,
                              IAPIHelper apiHelper)
        {
            _events = events;
            _events.SubscribeOnPublishedThread(this);

            _user = user;
            _apiHelper = apiHelper;
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<ProductsViewModel>(), cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task HandleAsync(RegisterEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<RegisterViewModel>(), cancellationToken);
        }

        public async Task HandleAsync(LogInEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), cancellationToken);
        }

        public async Task HandleAsync(InsertProductsEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<InsertProductsViewModel>(), cancellationToken);
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task HandleAsync(OrdersReportEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<OrdersViewModel>(), cancellationToken);
        }


        public async Task HandleAsync(DisplayInventoryEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<DisplayInventoryViewModel>(), cancellationToken);
        }

        public async Task HandleAsync(UserManagementEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), cancellationToken);
        }

        public async Task HandleAsync(ReviewOrderEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<ReviewOrderViewModel>(), cancellationToken);
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }
                return output;
            }

        }


    }
}
