using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task5.Controllers;
using Task5.Models;
using System.Data;
using System.Data.Entity;
using Moq;
using Task5.Interfaces;

namespace Task5.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserManagerRepository> _mockUserManagerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserManagerRepository = new Mock<IUserManagerRepository>();
            _controller = new UsersController(_mockUserManagerRepository.Object);
        }

        [Fact]
        public async Task Index_Returns_ViewResult_With_UserViewModelList()
        {
            // Arrange
            var listUserViewModel = new List<UserViewModel>
            {
                new UserViewModel { UserName = "user1@example.com", Email = "user1@example.com", Role = "Admin" },
                new UserViewModel { UserName = "user2@example.com", Email = "user2@example.com", Role = "User" }
            };
            _mockUserManagerRepository.Setup(x => x.GetAllUserViewModel()).ReturnsAsync(listUserViewModel);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task ChangeRole_Returns_RedirectToAction_When_Successful()
        {
            // Arrange
            var userId = "user1";
            var oldRole = "User";
            var newRole = "Admin";

            // Act
            var result = await _controller.ChangeRole(userId, oldRole, newRole);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Users", redirectToActionResult.ControllerName);
        }
    }
}
