using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TulipDataManager.Library.Internal.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Library.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        public List<UserModel> GetUserById(string Id)
        {
            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "TulipData");

            return output;
        }

        public void InsertUser(UserModel user)
        {
            _sql.SaveData("dbo.spUser_Insert", user, "TulipData");
        }
    }
}
