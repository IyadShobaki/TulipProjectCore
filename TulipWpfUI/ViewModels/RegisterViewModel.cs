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
    public class RegisterViewModel : Screen
    {
        private readonly IAPIHelper _apiHelper;
        private readonly IEventAggregator _events;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public RegisterViewModel(IAPIHelper apiHelper, IEventAggregator events,
            StatusInfoViewModel status, IWindowManager window)
        {
            _apiHelper = apiHelper;
            _events = events;
            _status = status;
            _window = window;
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }


        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanSubmit);
            }
        }

        private string _confirmPassword;

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                _confirmPassword = value;
                NotifyOfPropertyChange(() => ConfirmPassword);
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

                if (Email?.Length > 0 && Password?.Length > 0 && ConfirmPassword?.Length > 0)
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

                RegisterModel registerModel = new RegisterModel
                {
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword
                };

                //string createSuccess = await _apiHelper.RegisterUser(Email, Password, ConfirmPassword);
                string createSuccess = await _apiHelper.RegisterUser(registerModel);

                if (createSuccess == "success")
                {
                    var result = await _apiHelper.Authenticate(Email, Password);

                    UserModel user = new UserModel();
                    List<string> UInfor = await _apiHelper.GetUserId(result.Access_Token);
                    user.Id = UInfor[0];
                    user.FirstName = FirstName;
                    user.LastName = LastName;
                    user.EmailAddress = Email;

                    await _apiHelper.PostUserInfo(user);


                    settings.Title = "System Message";
                    _status.UpdateMessage("Account Created Successfully", $"{FirstName} Thank you for joining us!");
                    await _window.ShowDialogAsync(_status, null, settings);

                    await _events.PublishOnUIThreadAsync(new LogInEvent());
                }

            }
            catch (Exception ex)
            {
                settings.Title = "System Error";
                _status.UpdateMessage("Fatal Exception", ex.Message);
                _window.ShowDialogAsync(_status, null, settings);
            }
        }




        public void LogIn()
        {
            _events.PublishOnUIThreadAsync(new LogInEvent());
        }

        public void ResetFields()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            Password = "";
            ConfirmPassword = "";
        }
    }
}
