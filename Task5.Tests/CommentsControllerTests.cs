using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Task5.Controllers;
using Task5.Interfaces;
using Task5.Models;

namespace Task5.Tests
{
    public class CommentsControllerTests
    {
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly CommentsController _commentsController;

        public CommentsControllerTests()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _commentsController = new CommentsController(_mockCommentRepository.Object);
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
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            Assert.Equal("Movies", redirectToActionResult.ControllerName);
            Assert.Equal(movieId, redirectToActionResult.RouteValues["id"]);
        }

        [Fact]
        public void Delete_Returns_RedirectToAction_When_Successful()
        {
            // Arrange
            var comment = new Comment { Id = 1, MovieId = 1, UserId = "123" };
            _mockCommentRepository.Setup(x => x.Delete(comment.Id)).Returns(comment);

            // Act
            var result = _commentsController.Delete(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            Assert.Equal("Movies", redirectToActionResult.ControllerName);
            Assert.Equal(comment.MovieId, redirectToActionResult.RouteValues["id"]);
        }
    }
}
