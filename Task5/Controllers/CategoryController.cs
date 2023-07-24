using Microsoft.AspNetCore.Mvc;
using Task5.Models;
using Microsoft.AspNetCore.Authorization;
using Task5.Interfaces;
using Task5.Repositories;

namespace Task5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Category category)
        {
            _categoryRepository.Add(category);
            return RedirectToAction("Add", "Edit");
        }
    }
}
