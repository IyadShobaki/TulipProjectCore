using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TulipWpfUI.Library.Models
{
    public class ApplicationUserModel
    {

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; }


        public string RoleList
        {
            get
            {
                if (Roles.Count > 0)
                {
                    return string.Join(", ", Roles.Select(x => x.Value));
                }
                return "User is a customer";
            }

        }

    }
}
