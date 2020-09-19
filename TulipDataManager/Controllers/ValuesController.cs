using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TulipDataManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        // GET api/values
        public IEnumerable<string> Get()
        {
            // Using Postman to display the user id
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //RequestContext.Principal.Identity.GetUserId(); //old .NET Framework
                                                                            //var roles = new Dictionary<string, string>();

            //using (var context = new ApplicationDbContext())
            //{

            //    roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

            //}

            //string result = "Not Admin";

            //if (roles.Keys.Contains(userId))
            //{
            //    result = "Admin";
            //}


            string result = "Not Admin";
            //if (RequestContext.Principal.IsInRole("Admin")) //.NET Framework
            if (User.IsInRole("Admin")) // CORE
            {
                result = "Admin";
            }

            //return new string[] { "value1", "value2", userId };
            return new string[] { userId, result };

        }
    }
}
