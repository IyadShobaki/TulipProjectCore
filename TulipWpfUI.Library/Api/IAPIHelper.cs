using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }

        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggendInUserInfo(string token);
        Task<List<string>> GetUserId(string token);
        Task PostUserInfo(UserModel user);
        Task<string> RegisterUser(string email, string password, string confirmPassword);
        void LogOffUser();
    }
}