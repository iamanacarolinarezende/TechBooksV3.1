using Microsoft.EntityFrameworkCore;
using TechBooks.Models;

namespace TechBooks.Data;

public static class BooksData
{
    public static async Task Insert(Book book, TechBooksContext context)
    {
        book.CreationDate = DateTime.Now;
        context.Attach(book.Category); // NECESSARY TO AVOID AN EXCEPTION
        context.Books.Add(book);
        await context.SaveChangesAsync();
    }

    public static async Task Update(Book book, TechBooksContext context)
    {
        context.Attach(book.Category); // NECESSARY TO AVOID CHANGING THE CATEGORY NAME TO "Temp Name"
        var entity = context.Books.Update(book);
        entity.Property(c => c.CreationDate).IsModified = false;
        await context.SaveChangesAsync();
    }

    public static async Task<Book?> GetBook(int bookId, TechBooksContext context)
    {
        return await context.Books.FindAsync(bookId);
    }

    public static async Task<List<Book>> GetList(TechBooksContext context)
    {
        return await context.Books.ToListAsync();
    }

    public static async Task Delete(Book book, TechBooksContext context)
    {
        //book.Category = null!; // NECESSARY TO AVOID CREATING A BLANK CATEGORY
        context.Books.Remove(book);
        await context.SaveChangesAsync();
    }

    public static async Task<bool> HasAuthors(Book book, TechBooksContext context)
    {
        return await context.AuthorBooks.AnyAsync(ab => ab.BookId == book.BookId);
    }
}


