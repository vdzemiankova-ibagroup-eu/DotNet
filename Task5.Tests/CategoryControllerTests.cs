using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Task5.Controllers;
using Task5.Data;
using Task5.Models;

namespace Task5.Tests
{
    public class CategoryControllerTests
    {
        private readonly Data.ApplicationDbContext _dbContext;
        private readonly CategoryController _categoryController;

        public CategoryControllerTests()
        {
            var options = new DbContextOptionsBuilder<Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbCategory")
            .Options;

            _dbContext = new Data.ApplicationDbContext(options);
            _categoryController = new CategoryController(_dbContext);
        }

        [Fact]
        public void Add_Get_ReturnsViewResult()
        {
            // Act
            var result = _categoryController.Add();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public void Add_Post_RedirectsToAction()
        {
            // Arrange
            var category = new Category { Id = 1, Name = "Category 1" };

            // Act
            var result = _categoryController.Add(category);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Add", redirectToActionResult.ActionName);
            Assert.Equal("Edit", redirectToActionResult.ControllerName);

            _dbContext.Database.EnsureDeleted();
        }
    }
}
