using System.Collections.Generic;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipWpfUI.Library.Api
{
    public interface IUserEndPoint
    {
        Task AddUserToRole(string userId, string roleName);
        Task CreateNewRole(string roleName);
        Task<Dictionary<string, string>> GetAllRolesInfo();
        Task<List<ApplicationUserModel>> GetAllUsersInfo();
        Task RemoveUserFromRole(string userId, string roleName);
    }
}