using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public class UserEndPoint : IUserEndPoint
    {
        private readonly IAPIHelper _apiHelper;

        public UserEndPoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public async Task<List<ApplicationUserModel>> GetAllUsersInfo()
        {

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/GetAllUsers"))
            {
                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadAsAsync<List<ApplicationUserModel>>();

                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<Dictionary<string, string>> GetAllRolesInfo()
        {

            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/GetAllRoles"))
            {
                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadAsAsync<Dictionary<string, string>>();

                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task AddUserToRole(string userId, string roleName)
        {
            var data = new { userId, roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/AddRole", data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task RemoveUserFromRole(string userId, string roleName)
        {
            var data = new { userId, roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/RemoveRole", data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task CreateNewRole(string roleName)
        {
            var data = new { roleName };
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/CreateRole", roleName))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
