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
using Task5.Interfaces;
using Task5.Repositories;

namespace Task5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EditController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IMovieRepository _movieRepository;
        private ICommentRepository _commentRepository;
        private IMovieCategoryRepository _movieCategoryRepository;

        public EditController(  
            ICategoryRepository categoryRepository,
            IMovieRepository movieRepository,
            ICommentRepository commentRepository,
            IMovieCategoryRepository movieCategoryRepository
            )
        {
            _categoryRepository = categoryRepository;
            _movieRepository = movieRepository;
            _commentRepository = commentRepository;
            _movieCategoryRepository = movieCategoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAllRestEaseAsync();
            var comments = _commentRepository.GetAll();
            var categories = _categoryRepository.GetAll();
            var movieCategories = _movieCategoryRepository.GetAll();

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
            var movie = await _movieRepository.GetByIdRestEaseAsync(id);
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movieRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            List<Category> category = _categoryRepository.GetAll();
            MovieCategoriesViewModel movieCategoriesViewModel = new MovieCategoriesViewModel() { Categories = category };
            return View(movieCategoriesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Movie movie, List<int> categoryIds)
        {
            var result = await _movieRepository.AddAsync(movie);
            if (result != null)
            {
                foreach (int id in categoryIds)
                {
                    _movieCategoryRepository.Add(new MovieCategory { MovieId = result.Id, CategoryId = id});
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
            Movie movie = await _movieRepository.GetByIdRestEaseAsync(id);

            List<Category> category = _categoryRepository.GetAll();
            MovieCategoriesViewModel movieCategoriesViewModel = new MovieCategoriesViewModel() { Movie = movie, Categories = category };
            ViewBag.SelectedCategories = _movieCategoryRepository.GetIdsByMovieId(movie.Id); 

            return View(movieCategoriesViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, List<int> categoryIds)
        {
            
            Movie result = await _movieRepository.UpdateAsync(movie);

            if (result != null)
            {
                _movieCategoryRepository.DeleteByMovieId(movie.Id);

                foreach (int id in categoryIds)
                {
                    _movieCategoryRepository.Add(new MovieCategory { MovieId = result.Id, CategoryId = id });
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
