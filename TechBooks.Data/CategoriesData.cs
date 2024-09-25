using Microsoft.EntityFrameworkCore;
using TechBooks.Models;

namespace TechBooks.Data;

public static class CategoriesData
{
    public static async Task Insert(Category category, TechBooksContext context)
    {
        category.CreationDate = DateTime.Now;
        context.Categories.Add(category);
        await context.SaveChangesAsync();
    }

    public static async Task Update(Category category, TechBooksContext context)
    {
        var entity = context.Categories.Update(category);
        entity.Property(c => c.CreationDate).IsModified = false;
        await context.SaveChangesAsync();
    }

    public static async Task<Category?> GetCategory(int categoryId, TechBooksContext context)
    {
        return await context.Categories.FindAsync(categoryId);
    }

    public static async Task<List<Category>> GetList(TechBooksContext context)
    {
        return await context.Categories.ToListAsync();
    }

    public static async Task Delete(Category category, TechBooksContext context)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }

    public static async Task<bool> HasBooks(Category category, TechBooksContext context)
    {
        return await context.Books.AnyAsync(b => b.CategoryId == category.CategoryId);
    }
}

