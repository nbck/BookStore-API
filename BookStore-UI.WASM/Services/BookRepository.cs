using System.Net.Http;
using Blazored.LocalStorage;
using BookStore_UI.WASM.Contracts;
using BookStore_UI.WASM.Models;

namespace BookStore_UI.WASM.Services
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorageService;

        public BookRepository(HttpClient client, ILocalStorageService localStorageService) : base(client,
            localStorageService)
        {
            _client = client;
            _localStorageService = localStorageService;
        }
    }
}