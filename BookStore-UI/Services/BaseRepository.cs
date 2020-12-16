using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using Newtonsoft.Json;

namespace BookStore_UI.Services
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILocalStorageService localStorageService;

        public BaseRepository(IHttpClientFactory clientFactory, ILocalStorageService localStorageService)
        {
            this.clientFactory = clientFactory;
            this.localStorageService = localStorageService;
        }

        public async Task<T> Get(string url, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url + id);

            HttpClient client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }

            return null;
        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpClient client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }

            return null;
        }

        public async Task<bool> Create(string url, T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (obj == null)
                return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            HttpClient client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Created)
                return true;
            
            return false;
        }

        public async Task<bool> Update(string url, T obj, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url + id);
            if (obj == null)
                return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            HttpClient client = await this.GetAuthenticatedHttpClient();

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
                return true;

            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
                return false;

            var request = new HttpRequestMessage(HttpMethod.Delete, url + id);

            HttpClient client = await this.GetAuthenticatedHttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.NoContent)
                return true;
            
            return false;
        }

        private async Task<string> GetBearerToke()
        {
            return await this.localStorageService.GetItemAsync<string>("authToken");
        }

        private async Task<HttpClient> GetAuthenticatedHttpClient()
        {
            var client = this.clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await this.GetBearerToke());
            return client;
        }
    }
}