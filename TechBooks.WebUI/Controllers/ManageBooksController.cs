using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechBooks.Data;
using TechBooks.Models;

namespace TechBooks.WebUI.Controllers;

public class ManageBooksController : Controller
{
    private readonly TechBooksContext _context;

    #region Support Methods
    private async Task LoadViewBag_Categories()
    {
        List<Category> listOfCategories = new List<Category>();

        try
        {
            _context.ChangeTracker.Clear(); // Necessary to avoid showing "Temp Name"
            listOfCategories = await CategoriesData.GetList(_context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }

        ViewBag.Categories = new SelectList(listOfCategories, "CategoryId", "Description");
    }
    #endregion

    public ManageBooksController(TechBooksContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var listOfBooks = new List<Book>();
        var listOfCategories = new List<Category>();
        try
        {
            listOfCategories = await CategoriesData.GetList(_context);
            listOfBooks = await BooksData.GetList(_context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }

        foreach (var book in listOfBooks)
        {
            book.Category = listOfCategories.FirstOrDefault(c => c.CategoryId == book.CategoryId)!;
        }

        return View(listOfBooks);
    }

    public async Task<IActionResult> AddOrUpdate(int? id)
    {
        Book? book = null;
        try
        {
            await LoadViewBag_Categories();
            if (id == null) return View();
            book = await BooksData.GetBook((int)id, _context);
            if (book == null)
                return RedirectToAction("Index", "NotFound", new { entity = "Book", backUrl = "/ManageBooks/" });
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrUpdate(Book book)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBag_Categories();
            return (book.BookId == 0) ? View() : View(book);
        }

        try
        {
            if (book.BookId == 0)
                await BooksData.Insert(book, _context);
            else
                await BooksData.Update(book, _context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
            await LoadViewBag_Categories();
            return (book.BookId == 0) ? View() : View(book);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Book book)
    {
        try
        {
            if (await BooksData.HasAuthors(book, _context))
                throw new Exception("This Book cannot be removed because it has been associated with one or more authors. Remove all associations first.");
            else
                await BooksData.Delete(book, _context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

