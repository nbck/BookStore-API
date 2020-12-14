using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore_API.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext db;

        public AuthorRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IList<Author>> FindAll()
        {
            var authors = await this.db.Authors.ToListAsync();
            return authors;
        }

        public async Task<Author> FindById(int id)
        {
            var author = await this.db.Authors.FindAsync(id).ConfigureAwait(false);
            return author;
        }

        public async Task<bool> Create(Author entity)
        {
            await this.db.AddAsync(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> IsExists(int id)
        {
            return await this.db.Authors.AnyAsync(q => q.Id == id);
        }

        public async Task<bool> update(Author entity)
        {
            this.db.Update(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> Delete(Author entity)
        {
            this.db.Remove(entity);
            return await this.Save().ConfigureAwait(false);
        }

        public async Task<bool> Save()
        {
            var changes = await this.db.SaveChangesAsync().ConfigureAwait(false);
            return changes > 0;
        }
    }
}