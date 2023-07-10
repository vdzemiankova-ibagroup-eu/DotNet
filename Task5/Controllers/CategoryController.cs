using Microsoft.AspNetCore.Mvc;
using Task5.Models;
using Microsoft.AspNetCore.Authorization;

namespace Task5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private Data.ApplicationDbContext _dbContext;

        public CategoryController(Data.ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return RedirectToAction("Add", "Edit");
        }
    }
}
