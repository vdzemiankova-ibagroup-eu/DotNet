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
using Task5.Interfaces;

namespace Task5.Tests
{
    public class MoviesControllerTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<IMovieCategoryRepository> _mockMovieCategoryRepository;
        private readonly Mock<IUserManagerRepository> _mockUserManagerRepository;
        private readonly Mock<IMovieRepository> _mockMovieRepository;

        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockMovieCategoryRepository = new Mock<IMovieCategoryRepository>();
            _mockUserManagerRepository = new Mock<IUserManagerRepository>();
            _mockMovieRepository = new Mock<IMovieRepository>();

            _controller = new MoviesController(
                _mockCategoryRepository.Object, 
                _mockCommentRepository.Object,
                _mockMovieCategoryRepository.Object,
                _mockUserManagerRepository.Object,
                _mockMovieRepository.Object
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
            _mockMovieRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(movies);

            var comments = new List<Comment>
            {
                new Comment { Id = 1, MovieId = 1, UserGrade = 5, UserId = "user1" },
                new Comment { Id = 2, MovieId = 1, UserGrade = 3, UserId = "user2" },
                new Comment { Id = 3, MovieId = 2, UserGrade = 4, UserId = "user1" }
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
            var result = await _controller.Index(1);

            // Assert
            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            var viewModelList = Assert.IsAssignableFrom<List<MovieGradeCategoryViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, viewModelList.Count);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WithMovieAndComments()
        {
            // Arrange
            var movie = new Movie { Id = 1, MovieName = "Movie 1", FirstName = "FName 1", LastName = "LMane 1", MovieRating = 1m, MovieYear = 2011 };
            _mockMovieRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(movie);

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", UserName = "user1@example.com", Email = "user1@example.com" },
                new ApplicationUser { Id = "user2", UserName = "user2@example.com", Email = "user2@example.com" }
            };
            _mockUserManagerRepository.Setup(x => x.GetAllUsers()).Returns(users);

            var comments = new List<Comment>
            {
                new Comment { Id = 1, MovieId = 1, UserId = "user1" },
                new Comment { Id = 2, MovieId = 1, UserId = "user2" }
            };
            _mockCommentRepository.Setup(x => x.GetAll()).Returns(comments);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Movie>(viewResult.ViewData.Model);
            Assert.Equal(movie, model);

            var commentsViewModel = Assert.IsAssignableFrom<IEnumerable<CommentViewModel>>(viewResult.ViewData["Comments"]);
            Assert.Equal(2, commentsViewModel.Count());
        }
    }
}
