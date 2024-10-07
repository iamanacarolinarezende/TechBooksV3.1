using Microsoft.AspNetCore.Mvc;
using TechBooks.Data;
using TechBooks.Models;

namespace TechBooks.WebUI.Controllers;

public class ManageAuthorsController : Controller
{
    private readonly TechBooksContext _context;

    public ManageAuthorsController(TechBooksContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var listOfAuthors = new List<Author>();
        try
        {
            listOfAuthors = await AuthorsData.GetList(_context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }
        return View(listOfAuthors);
    }

    public async Task<IActionResult> AddOrUpdate(int? id)
    {
        if (id == null) return View();

        Author? author = null;
        try
        {
            author = await AuthorsData.GetAuthor((int)id, _context);
            if (author == null)
                return RedirectToAction("Index", "NotFound", new { entity = "Author", backUrl = "/ManageAuthors/" });
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }
        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrUpdate(Author author)
    {
        if (!ModelState.IsValid)
            return (author.AuthorId == 0) ? View() : View(author);

        try
        {
            if (author.AuthorId == 0)
                await AuthorsData.Insert(author, _context);
            else
                await AuthorsData.Update(author, _context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
            return (author.AuthorId == 0) ? View() : View(author);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Remove(Author author)
    {
        try
        {
            if (await AuthorsData.HasBooks(author, _context))
                throw new Exception("This Author cannot be removed because it has been associated with one or more books. Remove all associations first.");
            else
                await AuthorsData.Delete(author, _context);
        }
        catch (Exception ex)
        {
            TempData["DangerMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

