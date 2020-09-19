using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TulipDataManager.Data;
using TulipDataManager.Library.DataAccess;
using TulipDataManager.Library.Models;

namespace TulipDataManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration config) // .NET CORE
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }
        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//RequestContext.Principal.Identity.GetUserId();

            UserData data = new UserData(_config);

            return data.GetUserById(userId).First();

        }
        [HttpPost]
        public void Post(UserModel user)
        {

            UserData data = new UserData(_config);
            data.InsertUser(user);

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            //using (var context = new ApplicationDbContext()) // Framework
            //{
            //var userStore = new UserStore<ApplicationUser>(context);
            //var userManager = new UserManager<ApplicationUser>(userStore);

            //var users = userManager.Users.ToList();
            //var roles = context.Roles.ToList();

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            foreach (var user in users)
            {
                UserData data = new UserData(_config);

                var userInfo = data.GetUserById(user.Id).First();

                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id)
                    .ToDictionary(key => key.RoleId, val => val.Name);

                //foreach (var r in user.Roles)
                //{
                //    u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                //}
                output.Add(u);
            }
            //}

            return output;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            //using (var context = new ApplicationDbContext())
            //{
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            return roles;

            //}
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddRole")]
        public async Task AddARole(UserRolePairModel pairing)
        {
            //using (var context = new ApplicationDbContext())
            //{
            //var userStore = new UserStore<ApplicationUser>(context);
            //var userManager = new UserManager<ApplicationUser>(userStore);
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.AddToRoleAsync(user, pairing.RoleName);
            //}
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);


            //using (var context = new ApplicationDbContext())
            //{
            //    var userStore = new UserStore<ApplicationUser>(context);
            //    var userManager = new UserManager<ApplicationUser>(userStore);

            //    userManager.RemoveFromRole(pairing.UserId, pairing.RoleName);
            //}
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("CreateRole")]
        public async Task CreateARole(object roleName)
        {
            string role = roleName.ToString();

            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist == false)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }



            ///string role = (string)roleName[0];

            //string role = JsonConvert.DeserializeObject<string>((string)roleName[0]);// (string)roleName;

            // _roleManager.CreateAsync(new IdentityRole(role));
            //_roleManager.CreateAsync(new IdentityRole { Name = $"{role}" });

            //using (var context = new ApplicationDbContext())
            //{
            //    var roleStore = new RoleStore<IdentityRole>(context);
            //    var roleManager = new RoleManager<IdentityRole>(roleStore);
            //    roleManager.Create(new IdentityRole { Name = $"{role}" });
            //}
        }
    }
}
