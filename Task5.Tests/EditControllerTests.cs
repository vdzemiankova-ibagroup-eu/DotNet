using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task5.Controllers;
using Task5.Models;

namespace Task5.Tests
{
    public class EditControllerTests
    {
        private readonly Mock<IMovieApi> _movieApiMock;
        private readonly Data.ApplicationDbContext _dbContext;
        private readonly EditController _controller;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditControllerTests()
        {
            var options = new DbContextOptionsBuilder<Data.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbEdit")
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

            _controller = new EditController(_movieApiMock.Object, _dbContext);
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

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModelList = Assert.IsAssignableFrom<List<MovieCategoriesViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewModelList.Count);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithMovie()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            _movieApiMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(movie);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Movie>(viewResult.ViewData.Model);
            Assert.Equal(movie, model);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex()
        {
            // Arrange
            var id = 1;

            // Act
            var result = await _controller.DeleteConfirmed(id);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Add_ReturnsViewResult_WithViewModel()
        {
            // Arrange

            // Act
            var result = _controller.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsAssignableFrom<MovieCategoriesViewModel>(viewResult.ViewData.Model);
            Assert.NotNull(viewModel.Categories);
        }

        [Fact]
        public async Task Add_RedirectsToIndex_WhenMovieAddedSuccessfully()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            var categoryIds = new List<int> { 1, 2 };
            _movieApiMock.Setup(x => x.AddAsync(movie)).ReturnsAsync(movie);

            // Act
            var result = await _controller.Add(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Add_ReturnsViewResult_WhenMovieNotAdded()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };

            var categoryIds = new List<int> { 1, 2 };
            _movieApiMock.Setup(x => x.AddAsync(movie)).ReturnsAsync((Movie)null);

            // Act
            var result = await _controller.Add(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public void Back_RedirectsToIndex()
        {
            // Arrange

            // Act
            var result = _controller.Back();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            _movieApiMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(movie);

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
                new MovieCategory { Id = 2, MovieId = 1, CategoryId = 2 }
            };
            foreach (var movieCategorie in movieCategories)
            {
                _dbContext.MovieCategories.Add(movieCategorie);
            }
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsAssignableFrom<MovieCategoriesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(movie, viewModel.Movie);
            Assert.Equal(categories.Count, viewModel.Categories.Count);
            Assert.Equal(movieCategories.Select(mc => mc.CategoryId).ToList(), viewResult.ViewData["SelectedCategories"]);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Edit_Post_RedirectsToIndex_WhenMovieUpdatedSuccessfully()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            var categoryIds = new List<int> { 1, 2 };
            _movieApiMock.Setup(x => x.UpdateAsync(movie.Id, movie)).ReturnsAsync(movie);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            foreach (var categorie in categories)
            {
                _dbContext.Categories.Add(categorie);
            }

            // Act
            var result = await _controller.Edit(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Edit_Post_ReturnsViewResult_WhenMovieNotUpdated()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            var categoryIds = new List<int> { 1, 2 };
            _movieApiMock.Setup(x => x.UpdateAsync(movie.Id, movie)).ReturnsAsync((Movie)null);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            foreach (var categorie in categories)
            {
                _dbContext.Categories.Add(categorie);
            }

            // Act
            var result = await _controller.Edit(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            _dbContext.Database.EnsureDeleted();
        }
    }
}
