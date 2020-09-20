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
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {

        private readonly IEventAggregator _events;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndPoint _userEndPoint;

        public UserDisplayViewModel(IEventAggregator events,
            StatusInfoViewModel status, IWindowManager window, IUserEndPoint userEndPoint)
        {
            _events = events;
            _status = status;
            _window = window;
            _userEndPoint = userEndPoint;
        }

        protected override async void OnViewLoaded(object view)
        {

            base.OnViewLoaded(view);
            try
            {

                await LoadUsers();
                await LoadRoles();

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
                    _window.ShowDialogAsync(_status, null, settings);
                    await _events.PublishOnUIThreadAsync(new LogOnEvent());
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    _window.ShowDialogAsync(_status, null, settings);
                    await _events.PublishOnUIThreadAsync(new LogOnEvent());
                }
            }

        }


        private async Task LoadUsers()
        {

            var usersList = await _userEndPoint.GetAllUsersInfo();
            Users = new BindingList<ApplicationUserModel>(usersList);
        }

        private BindingList<ApplicationUserModel> _users;

        public BindingList<ApplicationUserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
                NotifyOfPropertyChange(() => IsAdmin);
                NotifyOfPropertyChange(() => IsEmpty);
            }

        }

        private ApplicationUserModel _selectedUser;

        public ApplicationUserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                if (SelectedUser != null)
                {
                    SelectedUserName = value.Email;
                    UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                }
                NotifyOfPropertyChange(() => SelectedUser);
                SelectedUserRole = null;
                NotifyOfPropertyChange(() => SelectedUserRole);
                NotifyOfPropertyChange(() => CanAddSelectedRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);
            }
        }

        private string _selectedUserName;

        public string SelectedUserName
        {
            get
            {
                return _selectedUserName;
            }
            set
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        private BindingList<string> _userRoles = new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<string> _availableRoles;

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }

        private async Task LoadRoles()
        {
            var result = await _userEndPoint.GetAllRolesInfo();
            AvailableRoles = new BindingList<string>(result.Select(x => x.Value).ToList());

        }


        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set
            {
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);
            }
        }

        private string _selectedAvailableRole;
        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);
                NotifyOfPropertyChange(() => CanAddSelectedRole);
            }
        }


        public bool CanAddSelectedRole
        {
            get
            {
                if (SelectedUser != null && SelectedAvailableRole != null)
                {
                    return true;
                }
                return false;
            }
        }
        public async void AddSelectedRole()
        {
            if (UserRoles.IndexOf(SelectedAvailableRole) < 0)
            {
                await _userEndPoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

                UserRoles.Add(SelectedAvailableRole);
                await LoadUsers();
                NotifyOfPropertyChange(() => Users);
                NotifyOfPropertyChange(() => UserRoles);

            }
            else
            {
                MessageBox.Show("Already There!");
            }


        }


        public bool CanRemoveSelectedRole
        {
            get
            {
                if (SelectedUser != null && SelectedUserRole != null)
                {
                    return true;
                }
                return false;
            }
        }

        public async void RemoveSelectedRole()
        {
            await _userEndPoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

            UserRoles.Remove(SelectedUserRole);
            await LoadUsers();
            NotifyOfPropertyChange(() => Users);
            NotifyOfPropertyChange(() => UserRoles);
        }

        public bool IsAdmin
        {
            get
            {
                if (Users?.Count > 0)
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
            }
        }

        public void BackToProduct()
        {
            _events.PublishOnUIThreadAsync(new LogOnEvent());
        }

        private string _newRole;

        public string NewRole
        {
            get { return _newRole; }
            set
            {
                _newRole = value;
                NotifyOfPropertyChange(() => NewRole);
                NotifyOfPropertyChange(() => CanCreateNewRole);
            }
        }

        public bool CanCreateNewRole
        {
            get
            {
                if (NewRole?.Length > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public async Task CreateNewRole()
        {
            await _userEndPoint.CreateNewRole(NewRole);
            NotifyOfPropertyChange(() => AvailableRoles);
        }
    }
}
