using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc;
using RestEase;
using System.Net.Http.Headers;
using System.Web;
using Task5.Models;
using Abp;
using System.Data;

namespace Task5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EditController : Controller
    {
        private IMovieApi _movieApi;
        private Data.ApplicationDbContext _dbContext;

        public EditController(IMovieApi movieApi, Data.ApplicationDbContext dbContext)
        {
            _movieApi = movieApi;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieApi.GetAllAsync();
            var comments = _dbContext.Comments.ToList();
            var categories = _dbContext.Categories.ToList();
            var movieCategories = _dbContext.MovieCategories.ToList();

            var viewModelList = new List<MovieCategoriesViewModel>();

            foreach (var movie in movies)
            {
                var listCategories = from movieCategorie in movieCategories
                                     join categorie in categories on movieCategorie.CategoryId equals categorie.Id into result
                                     from categorieId in result.DefaultIfEmpty()
                                     where movieCategorie.MovieId == movie.Id
                                     select categorieId;

                var viewModel = new MovieCategoriesViewModel
                {
                    Movie = movie,
                    Categories = listCategories.ToList()
                };

                viewModelList.Add(viewModel);
            }

            return View(viewModelList);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.MovieId = id;
            var movie = await _movieApi.GetByIdAsync(id);
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movieApi.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            List<Category> category = _dbContext.Categories.ToList().Select(c => new Category { Id = c.Id, Name = c.Name }).ToList();
            MovieCategoriesViewModel movieCategoriesViewModel = new MovieCategoriesViewModel() { Categories = category };
            return View(movieCategoriesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Movie movie, List<int> categoryIds)
        {
            var result = await _movieApi.AddAsync(movie);
            if (result != null)
            {
                foreach (int id in categoryIds)
                {
                    _dbContext.MovieCategories.Add(new MovieCategory { MovieId = result.Id, CategoryId = id});
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Что-то не так");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Back()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.MovieId = id;
            Movie movie = await _movieApi.GetByIdAsync(id);

            List<Category> category = _dbContext.Categories.ToList().Select(c => new Category { Id = c.Id, Name = c.Name }).ToList();
            MovieCategoriesViewModel movieCategoriesViewModel = new MovieCategoriesViewModel() { Movie = movie, Categories = category };
            ViewBag.SelectedCategories = _dbContext.MovieCategories.Where(x => x.MovieId == movie.Id).Select(x => x.CategoryId).ToList();

            return View(movieCategoriesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, List<int> categoryIds)
        {
            
            Movie result = await _movieApi.UpdateAsync(movie.Id, movie);

            if (result != null)
            {
                var itemsToRemove = _dbContext.MovieCategories.Where(x => x.MovieId == movie.Id);
                _dbContext.MovieCategories.RemoveRange(itemsToRemove);
                _dbContext.SaveChanges();

                foreach (int id in categoryIds)
                {
                    _dbContext.MovieCategories.Add(new MovieCategory { MovieId = result.Id, CategoryId = id });
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                Console.WriteLine("Что-то не так");
            }
            return RedirectToAction("Index");
        }
    }
}
