using System.Net.Http;
using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;

namespace BookStore_UI.Services
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILocalStorageService _localStorageService;

        public BookRepository(IHttpClientFactory clientFactory, ILocalStorageService localStorageService) :
            base(clientFactory,
                localStorageService)
        {
            _clientFactory = clientFactory;
            _localStorageService = localStorageService;
        }
    }
}