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
using Task5.Interfaces;
using Task5.Models;

namespace Task5.Tests
{
    public class EditControllerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMovieRepository> _mockMovieRepository;
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<IMovieCategoryRepository> _mockMovieCategoryRepository;

        private readonly EditController _controller;

        public EditControllerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMovieRepository = new Mock<IMovieRepository>();
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockMovieCategoryRepository = new Mock<IMovieCategoryRepository>();

            _controller = new EditController(
                _mockCategoryRepository.Object,
                _mockMovieRepository.Object,
                _mockCommentRepository.Object,
                _mockMovieCategoryRepository.Object
                );
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
            _mockMovieRepository.Setup(x => x.GetAllRestEaseAsync()).ReturnsAsync(movies);

            var comments = new List<Comment>
            {
                new Comment { Id = 1, MovieId = 1, UserId = "user1" },
                new Comment { Id = 2, MovieId = 1, UserId = "user2" }
            };
            _mockCommentRepository.Setup(x => x.GetAll()).Returns(comments);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            var movieCategories = new List<MovieCategory>
            {
                new MovieCategory { Id = 1, MovieId = 1, CategoryId = 1 },
                new MovieCategory { Id = 2, MovieId = 1, CategoryId = 2 },
                new MovieCategory { Id = 3, MovieId = 2, CategoryId = 2 }
            };
            _mockMovieCategoryRepository.Setup(x => x.GetAll()).Returns(movieCategories);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModelList = Assert.IsAssignableFrom<List<MovieCategoriesViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewModelList.Count);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithMovie()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            _mockMovieRepository.Setup(x => x.GetByIdRestEaseAsync(1)).ReturnsAsync(movie);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Movie>(viewResult.ViewData.Model);
            Assert.Equal(movie, model);
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
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

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
            _mockMovieRepository.Setup(x => x.AddAsync(movie)).ReturnsAsync(movie);

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
            _mockMovieRepository.Setup(x => x.AddAsync(movie)).ReturnsAsync((Movie)null);

            // Act
            var result = await _controller.Add(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
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
            _mockMovieRepository.Setup(x => x.GetByIdRestEaseAsync(1)).ReturnsAsync(movie);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            var movieCategories = new List<MovieCategory>
            {
                new MovieCategory { Id = 1, MovieId = 1, CategoryId = 1 },
                new MovieCategory { Id = 2, MovieId = 1, CategoryId = 2 }
            };
            _mockMovieCategoryRepository.Setup(x => x.GetAll()).Returns(movieCategories);

            var selectedCategories = new List<int> { 1, 2 };
            _mockMovieCategoryRepository.Setup(x => x.GetIdsByMovieId(movie.Id)).Returns(selectedCategories);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsAssignableFrom<MovieCategoriesViewModel>(viewResult.ViewData.Model);
            Assert.Equal(movie, viewModel.Movie);
            Assert.Equal(categories.Count, viewModel.Categories.Count);
            Assert.Equal(movieCategories.Select(mc => mc.CategoryId).ToList(), viewResult.ViewData["SelectedCategories"]);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToIndex_WhenMovieUpdatedSuccessfully()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            var categoryIds = new List<int> { 1, 2 };
            _mockMovieRepository.Setup(x => x.UpdateAsync(movie)).ReturnsAsync(movie);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            // Act
            var result = await _controller.Edit(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsViewResult_WhenMovieNotUpdated()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            var categoryIds = new List<int> { 1, 2 };
            _mockMovieRepository.Setup(x => x.UpdateAsync(movie)).ReturnsAsync((Movie)null);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };
            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(categories);

            // Act
            var result = await _controller.Edit(movie, categoryIds);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
