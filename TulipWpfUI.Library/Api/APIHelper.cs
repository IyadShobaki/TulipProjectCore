using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace TulipWpfUI.Library.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IConfiguration _config;

        public APIHelper(ILoggedInUserModel loggedInUser, IConfiguration config)
        {
            _config = config;
            InitializeClient();
            _loggedInUser = loggedInUser;

        }

        public HttpClient ApiClient
        {
            get
            {
                return _apiClient;
            }
        }

        private void InitializeClient()
        {
            //string api = _config.GetValue<string>("api");
            string api = ConfigurationManager.AppSettings["api"]; // This is if WPF the UI
            if (api == null)
            {
                api = _config.GetValue<string>("api"); // This is if Blazor the UI
            }


            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await _apiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }
        public async Task GetLoggendInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    _loggedInUser.CreatedDate = result.CreatedDate;
                    _loggedInUser.EmailAddress = result.EmailAddress;
                    _loggedInUser.FirstName = result.FirstName;
                    _loggedInUser.LastName = result.LastName;
                    _loggedInUser.Id = result.Id;
                    _loggedInUser.Token = token;

                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<string> RegisterUser(RegisterModel registerModel)
        {

            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync("/api/account/register", registerModel))
            {
                if (response.IsSuccessStatusCode)
                {
                    return "success";
                }
                else
                {
                    string res = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(res);
                    var errors = values.SelectMany(v => v.Values).ToList();
                    StringBuilder result = new StringBuilder();

                    for (int i = 1; i < errors.Count; i+=2)
                    {
                        //errors[i] = errors[i].Substring(errors[i].IndexOf(' ') + 1);
                        result.Append(errors[i]);
                    }   
                    
                    throw new Exception(result.ToString());
                }
            }
        }
        public async Task PostUserInfo(UserModel user)
        {
            using (HttpResponseMessage response = await _apiClient.PostAsJsonAsync("/api/User", user))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }


        public async Task<List<string>> GetUserId(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/values"))
            {
                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadAsAsync<List<string>>();

                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }



    }
}
