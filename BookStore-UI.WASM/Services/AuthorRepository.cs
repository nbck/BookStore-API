using System.Net.Http;
using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;

namespace BookStore_UI.WASM.Services
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;

        public AuthorRepository(HttpClient client, ILocalStorageService localStorageService) : base(client, localStorageService)
        {
            this._client = client;
            this._localStorageService = localStorageService;
        }
    }
}