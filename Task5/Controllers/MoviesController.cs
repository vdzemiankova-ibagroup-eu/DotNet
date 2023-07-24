using Abp.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Task5.Models;
using Task5.Data;
using Microsoft.Graph;
using Abp;
using Task5.Migrations;
using System.Diagnostics.Metrics;
using Microsoft.Ajax.Utilities;
using Castle.Core.Internal;
using Polly;
using Abp.Runtime.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Task5.Interfaces;
using System.Net.Http;
using Task5.Repositories;

namespace Task5.Controllers
{
    public class MoviesController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private ICommentRepository _commentRepository;
        private IMovieCategoryRepository _movieCategorieRepository;
        private IUserManagerRepository _userManagerRepository;
        private IMovieRepository _movieRepository;

        public MoviesController(
            ICategoryRepository categoryRepository, 
            ICommentRepository commentRepository, 
            IMovieCategoryRepository movieCategorieRepository,
            IUserManagerRepository userManagerRepository,
            IMovieRepository movieRepository
            )
        {
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _movieCategorieRepository = movieCategorieRepository;
            _userManagerRepository = userManagerRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var movies = await _movieRepository.GetAllAsync();

            var comments = _commentRepository.GetAll();
            var categories = _categoryRepository.GetAll();
            var movieCategories = _movieCategorieRepository.GetAll();

            var viewModelList = new List<MovieGradeCategoryViewModel>();

            ViewBag.Categories = categories;

            foreach (var movie in movies)
            {
                var listCategories = from movieCategorie in movieCategories
                                 join categorie in categories on movieCategorie.CategoryId equals categorie.Id into result
                                 from categorieId in result.DefaultIfEmpty()
                                 where movieCategorie.MovieId == movie.Id
                                 where ((categoryId is not null && categoryId == categorieId.Id) || categoryId is null)
                                     select categorieId?.Name ?? string.Empty;

                if (categoryId is not null && listCategories.IsNullOrEmpty())
                    continue;

                var listComments = comments.Where(x => x.MovieId == movie.Id);
               
                var viewModel = new MovieGradeCategoryViewModel
                {
                    Movie = movie,
                    AvgGrade = listComments.Any() ? listComments.Average(x => x.UserGrade) : null,
                    Categories = string.Join(Environment.NewLine, listCategories)
                };

                viewModelList.Add(viewModel);
            }

            return View(viewModelList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            var comments = _commentRepository.GetAll();
            var users = _userManagerRepository.GetAllUsers();

            var usersComments = from user in users
                                join comment in comments  on user.Id equals comment.UserId
                                join m in  new List<Movie> { movie } on comment.MovieId equals m.Id
                                select new CommentViewModel{ 
                                    UserName = user.UserName, 
                                    Grade = comment.UserGrade, 
                                    Id = comment.Id,
                                    Comment = comment.UserComment
                                };

            ViewBag.Comments = usersComments;
            ViewBag.Count = usersComments.Count();
            return View(movie);
        }
    }
}
