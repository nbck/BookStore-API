using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;

namespace BookStore_UI.WASM.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;

        public BaseRepository(HttpClient client, ILocalStorageService localStorageService)
        {
            this._client = client;
            this._localStorageService = localStorageService;
        }

        public async Task<T> Get(string url, int id)
        {
            HttpClient client = await this.GetAuthenticatedHttpClient();
            T response = await client.GetFromJsonAsync<T>(url + id);

            return response;
        }

        public async Task<IList<T>> Get(string url)
        {
            HttpClient client = await this.GetAuthenticatedHttpClient();
            IList<T> response = await client.GetFromJsonAsync<IList<T>>(url);
            return response;
        }

        public async Task<bool> Create(string url, T obj)
        {
            if (obj == null)
                return false;

            var client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response =await client.PostAsJsonAsync<T>(url, obj);
            
            if (response.StatusCode == HttpStatusCode.Created)
                return true;
            
            return false;
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            if (obj == null)
                return false;

            var client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response = await client.PutAsJsonAsync<T>(url + id, obj);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return true;

            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
                return false;

            var client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response = await client.DeleteAsync(url + id);

            if (response.StatusCode == HttpStatusCode.NoContent)
                return true;
            
            return false;
        }

        private async Task<string> GetBearerToken()
        {
            return await this._localStorageService.GetItemAsync<string>("authToken");
        }

        private async Task<HttpClient> GetAuthenticatedHttpClient()
        {
            var client = this._client;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await this.GetBearerToken());
            return client;
        }
    }
}