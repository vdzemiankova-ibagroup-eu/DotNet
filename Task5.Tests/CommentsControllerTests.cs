using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Task5.Controllers;
using Task5.Models;

namespace Task5.Tests
{
    public class CommentsControllerTests
    {
        private readonly Data.ApplicationDbContext _dbContext;
        private readonly CommentsController _commentsController;

        public CommentsControllerTests()
        {
            var options = new DbContextOptionsBuilder<Data.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbComments")
            .Options;

            _dbContext = new Data.ApplicationDbContext(options);
            _commentsController = new CommentsController(_dbContext);
        }

        [Fact]
        public void Add_Returns_RedirectToAction_When_Successful()
        {
            // Arrange
            var movieId = 1;
            var grade = 4.5m;
            var comment = "Great movie!";
            var userId = "123";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId)
                }));

            _commentsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = _commentsController.Add(movieId, grade, comment);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);

            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public void Delete_Returns_RedirectToAction_When_Successful()
        {
            // Arrange
            var comment = new Comment { Id = 1, MovieId = 1, UserId = "123" };
            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();

            // Act
            var result = _commentsController.Delete(1);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);

            _dbContext.Database.EnsureDeleted();
        }
    }
}
