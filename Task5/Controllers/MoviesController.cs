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
using Task5.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Task5.Controllers
{
    public class MoviesController : Controller
    {
        private IMovieApi _movieApi;
        private Data.ApplicationDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;

        public MoviesController(IMovieApi movieApi, Data.ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _movieApi = movieApi;
            _dbContext = dbContext;
            _userManager = userManager;
            SeedData();
        }

        private async void SeedData()
        {
            string[] roles = new string[] { "Admin", "User" };
            foreach (string role in roles)
            {
                if (!_dbContext.IdentityRoles.Any(r => r.Name == role))
                {
                    _dbContext.IdentityRoles.Add(new IdentityRole(role) { NormalizedName = role.ToUpper()});
                    _dbContext.SaveChanges();
                }
            }

            if (!_dbContext.ApplicationUsers.Any(u => u.UserName == "admin@gmail.com"))
            {
                var user = new ApplicationUser { UserName = "admin@gmail.com", Email = "admin@gmail.com" };
                var result = await _userManager.CreateAsync(user, "gkm%3/v8vQv9c9R");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                _dbContext.SaveChanges();
            }
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var movies = await _movieApi.GetAllAsync();
            var comments = _dbContext.Comments.ToList();
            var categories = _dbContext.Categories.ToList();
            var movieCategories = _dbContext.MovieCategories.ToList();

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
            var movie = await _movieApi.GetByIdAsync(id);
            var comments = _dbContext.Comments.ToList();
            var users = _dbContext.ApplicationUsers.ToList();

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
