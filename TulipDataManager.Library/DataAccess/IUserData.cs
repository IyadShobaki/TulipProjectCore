using System.Collections.Generic;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public interface IUserData
    {
        List<UserModel> GetUserById(string Id);
        void InsertUser(UserModel user);
    }
}