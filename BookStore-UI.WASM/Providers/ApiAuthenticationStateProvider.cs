using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookStore_UI.WASM.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public ApiAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            this._localStorageService = localStorageService;
            this._tokenHandler = _tokenHandler;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await this._localStorageService.GetItemAsync<string>("authToken");
                if (string.IsNullOrWhiteSpace(savedToken))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var tokenContent = this._tokenHandler.ReadJwtToken(savedToken);
                var expiry = tokenContent.ValidTo;
                if (expiry < DateTime.Now)
                {
                    await this._localStorageService.RemoveItemAsync("authToken");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                
                //Get claims from token and build auth user object
                var claims = ParseClaim(tokenContent);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                //return authenticated person
                return new AuthenticationState(user);
            }
            catch (Exception)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task LoggedIn()
        {
            var savedToken = await this._localStorageService.GetItemAsync<string>("authToken");
            var tokenContent = this._tokenHandler.ReadJwtToken(savedToken);
            var claims = ParseClaim(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            this.NotifyAuthenticationStateChanged(authState);
        }

        public void LoggedOut()
        {
            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(nobody));
            this.NotifyAuthenticationStateChanged(authState);
        }

        private IList<Claim> ParseClaim(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}