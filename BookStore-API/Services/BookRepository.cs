using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore_API.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext db;

        public BookRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IList<Book>> FindAll()
        {
            List<Book> books = await this.db.Books.Include(b => b.Author)
                .ToListAsync().ConfigureAwait(false);
            return books;
        }

        public async Task<Book> FindById(int id)
        {
            Book book = await this.db.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id).ConfigureAwait(false);
            return book;
        }

        public async Task<bool> Create(Book entity)
        {
            await this.db.Books.AddAsync(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> IsExists(int id) => await this.db.Books.AnyAsync(q => q.Id == id).ConfigureAwait(false);

        public async Task<bool> Update(Book entity)
        {
            this.db.Books.Update(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> Delete(Book entity)
        {
            this.db.Books.Remove(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> Save()
        {
            int changes = await this.db.SaveChangesAsync().ConfigureAwait(false);
            return changes > 0;
        }
    }
}