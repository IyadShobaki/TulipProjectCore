using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TulipWpfUI.Library.Models;

namespace TulipBlazorUI.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var email = await _sessionStorage.GetItemAsync<string>("email");

            ClaimsIdentity identity;
            if (email != null)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),

                }, "apiauth_type");
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(user));
        }

        //public void MarkUserAsAuthenticated(string emailAddress)
        public async Task MarkUserAsAuthenticated(ILoggedInUserModel loggedInUser)
        {

           // await _sessionStorage.SetItemAsync("email", user.Email);
            await _sessionStorage.SetItemAsync("token", loggedInUser.Token);
            //var identity = new ClaimsIdentity(new[]
            //{
            //    new Claim(ClaimTypes.Name, emailAddress),

            //}, "apiauth_type");

            var identity = GetClaimsIdentity(loggedInUser);

            var claimPrincipl = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimPrincipl)));
        }
        public void MarkUserAsLoggedOut()
        {
            //_sessionStorage.RemoveItemAsync("email");
            _sessionStorage.RemoveItemAsync("token");

            var identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        private ClaimsIdentity GetClaimsIdentity(ILoggedInUserModel user)
        {
            var claimsIdentity = new ClaimsIdentity();

            if (user.EmailAddress != null)
            {
                claimsIdentity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, user.EmailAddress),
                                    new Claim(ClaimTypes.Role, user.Role)
                                    
                                }, "apiauth_type");
            }

            return claimsIdentity;
        }

    }
}
