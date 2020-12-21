using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;
using BookStore_UI.WASM.Providers;
using BookStore_UI.WASM.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace BookStore_UI.WASM.Services
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationRepository(HttpClient client, ILocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this._client = client;
            this._localStorageService = localStorageService;
            this._authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.RegisterEndpoint, user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.LoginEndpoint, user);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            // Store Token
            await this._localStorageService.SetItemAsync("authToken", token.Token);

            // Change auth state of app
            await ((ApiAuthenticationStateProvider)this._authenticationStateProvider).LoggedIn();

            // FIXME should be unnecessary because client is not used anymore
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);
            
            return true;
        }

        public async Task Logout()
        {
            await this._localStorageService.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)this._authenticationStateProvider).LoggedOut();
        }
    }
}