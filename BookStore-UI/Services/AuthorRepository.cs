using System.Net.Http;
using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;

namespace BookStore_UI.Services
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILocalStorageService localStorageService;

        public AuthorRepository(IHttpClientFactory clientFactory, ILocalStorageService localStorageService) : base(clientFactory, localStorageService)
        {
            this._clientFactory = clientFactory;
            this.localStorageService = localStorageService;
        }
    }
}