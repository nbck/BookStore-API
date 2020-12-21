using System.Threading.Tasks;
using BookStore_API.Data;

namespace BookStore_API.Contracts
{
    public interface IBookRepository : IRepositoryBase<Book>
    {
        public Task<string> GetImageFileName(int id);

    }
}