using Microsoft.EntityFrameworkCore;
using TechBooks.Models;

namespace TechBooks.Data;

public static class AuthorsData
{
    public static async Task Insert(Author author, TechBooksContext context)
    {
        if (context.Authors.Any(a => a.Email == author.Email))
            throw new Exception("This email address has already been used.");

        author.CreationDate = DateTime.Now;
        context.Authors.Add(author);
        await context.SaveChangesAsync();
    }

    public static async Task Update(Author author, TechBooksContext context)
    {
        if (context.Authors.Any(a => a.AuthorId != author.AuthorId && a.Email == author.Email))
            throw new Exception("This email address has already been used.");

        var entity = context.Authors.Update(author);
        entity.Property(c => c.CreationDate).IsModified = false;
        await context.SaveChangesAsync();
    }

    public static async Task<Author?> GetAuthor(int authorId, TechBooksContext context)
    {
        return await context.Authors.FindAsync(authorId);
    }

    public static async Task<Author?> GetAuthor(string email, TechBooksContext context)
    {
        return await context.Authors.SingleOrDefaultAsync(a => a.Email == email);
    }

    public static async Task<List<Author>> GetList(TechBooksContext context)
    {
        return await context.Authors.ToListAsync();
    }

    public static async Task Delete(Author author, TechBooksContext context)
    {
        context.Authors.Remove(author);
        await context.SaveChangesAsync();
    }

    public static async Task<bool> HasBooks(Author author, TechBooksContext context)
    {
        return await context.AuthorBooks.AnyAsync(ab => ab.AuthorId == author.AuthorId);
    }
}

