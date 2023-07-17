using Castle.MicroKernel.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Testing;
using Moq;
using Moq.EntityFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Task5.Controllers;
using Task5.Models;
using Microsoft.Graph;

namespace Task5.Tests
{
    public class MoviesControllerTests
    {
        private readonly Mock<IMovieApi> _movieApiMock;
        private readonly Data.ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            var options = new DbContextOptionsBuilder<Data.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbMovies")
                .Options;
            _movieApiMock = new Mock<IMovieApi>();
            _dbContext = new Data.ApplicationDbContext(options);

            _userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(_dbContext),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            _controller = new MoviesController(_movieApiMock.Object, _dbContext, _userManager);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithViewModelList()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 },
                new Movie { Id = 1, MovieName = "Movie 2", FirstName = "FName 2", LastName = "LMane 2", MovieRating = 2m, MovieYear = 2012 }
            };
            _movieApiMock.Setup(x => x.GetAllAsync()).ReturnsAsync(movies);

            var comments = new List<Comment>
            {
                new Comment { Id = 1, MovieId = 1, UserGrade = 5, UserId = "1" },
                new Comment { Id = 2, MovieId = 1, UserGrade = 3, UserId = "2" },
                new Comment { Id = 3, MovieId = 2, UserGrade = 4, UserId = "1" }
            };
            foreach (var comment in comments)
            {
                _dbContext.Comments.Add(comment);
            }

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            foreach (var categorie in categories)
            {
                _dbContext.Categories.Add(categorie);
            }

            var movieCategories = new List<MovieCategory>
            {
                new MovieCategory { Id = 1, MovieId = 1, CategoryId = 1 },
                new MovieCategory { Id = 2, MovieId = 1, CategoryId = 2 },
                new MovieCategory { Id = 3, MovieId = 2, CategoryId = 2 }
            };
            foreach (var movieCategorie in movieCategories)
            {
                _dbContext.MovieCategories.Add(movieCategorie);
            }
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Index(1);

            // Assert
            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            var viewModelList = Assert.IsAssignableFrom<List<MovieGradeCategoryViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewModelList.Count);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithMovieAndComments()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            _movieApiMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(movie);
            var result1 = await _movieApiMock.Object.GetByIdAsync(1);

            var user1 = new ApplicationUser() { UserName = "user1@example.com", Email = "user1@example.com" };
            var user2 = new ApplicationUser() { UserName = "user2@example.com", Email = "user2@example.com" };

            await _userManager.CreateAsync(user1);
            await _userManager.CreateAsync(user2);

            var userId1 = await _userManager.GetUserIdAsync(user1);
            var userId2 = await _userManager.GetUserIdAsync(user2);

            var comments = new List<Comment>
            {
                new Comment { Id = 1, MovieId = 1, UserId = userId1 },
                new Comment { Id = 2, MovieId = 1, UserId = userId2 }
            };
            foreach (var comment in comments)
            {
                _dbContext.Comments.Add(comment);
            }
            _dbContext.SaveChanges();
            var fdv = _dbContext.Comments;

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Movie>(viewResult.ViewData.Model);
            Assert.Equal(movie, model);

            var commentsViewModel = Assert.IsAssignableFrom<IEnumerable<CommentViewModel>>(viewResult.ViewData["Comments"]);
            Assert.Equal(2, commentsViewModel.Count());

            _dbContext.Database.EnsureDeleted();
        }
    }
}
