using Microsoft.AspNetCore.Mvc;
using TechBooks.Data;
using TechBooks.Models;

namespace TechBooks.WebUI.Controllers
{
    public class ManageCategoriesController : Controller
    {
        private readonly TechBooksContext _context;

        public ManageCategoriesController(TechBooksContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listOfCategories = new List<Category>();
            try
            {
                listOfCategories = await CategoriesData.GetList(_context);
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return View(listOfCategories);
        }

        public async Task<IActionResult> AddOrUpdate(int? id)
        {
            if (id == null) return View();

            Category? category = null;
            try
            {
                category = await CategoriesData.GetCategory((int)id, _context);
                if (category == null)
                    return RedirectToAction("Index", "NotFound", new { entity = "Category", backUrl = "/ManageCategories/" });
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdate(Category category)
        {
            if (!ModelState.IsValid)
                return (category.CategoryId == 0) ? View() : View(category);

            try
            {
                if (category.CategoryId == 0)
                    await CategoriesData.Insert(category, _context);
                else
                    await CategoriesData.Update(category, _context);
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
                return (category.CategoryId == 0) ? View() : View(category);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Category category)
        {
            try
            {
                if (await CategoriesData.HasBooks(category, _context))
                    throw new Exception("This Category cannot be removed because it has been associated with one or more books. Remove all associations first.");
                else
                    await CategoriesData.Delete(category, _context);
            }
            catch (Exception ex)
            {
                TempData["DangerMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
