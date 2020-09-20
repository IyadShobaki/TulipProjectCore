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
        private readonly IUserData _userData;

        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserData userData)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _userData = userData;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _userData.GetUserById(userId).First();
        }

        [HttpPost]
        public void Post(UserModel user)
        {
            _userData.InsertUser(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            foreach (var user in users)
            {
                var userInfo = _userData.GetUserById(user.Id).First();

                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id)
                    .ToDictionary(key => key.RoleId, val => val.Name);
                output.Add(u);
            }

            return output;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            return roles;

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddRole")]
        public async Task AddARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.AddToRoleAsync(user, pairing.RoleName);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);

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
        }
    }
}
