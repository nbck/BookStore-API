using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore_API.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IList<Book>> FindAll()
        {
            List<Book> books = await _db.Books.Include(b => b.Author)
                .ToListAsync().ConfigureAwait(false);
            return books;
        }

        public async Task<Book> FindById(int id)
        {
            Book book = await _db.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id).ConfigureAwait(false);
            return book;
        }

        public async Task<bool> Create(Book entity)
        {
            await _db.Books.AddAsync(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> IsExists(int id) => await _db.Books.AnyAsync(q => q.Id == id).ConfigureAwait(false);

        public async Task<bool> Update(Book entity)
        {
            _db.Books.Update(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> Delete(Book entity)
        {
            _db.Books.Remove(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> Save()
        {
            int changes = await _db.SaveChangesAsync().ConfigureAwait(false);
            return changes > 0;
        }

        public async Task<string> GetImageFileName(int id)
        {
            Book book = await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            return book?.ImageName;
        }
    }
}