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

namespace Task5.Tests
{
    public class UsersControllerTests
    {
        private readonly Data.ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<Data.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbUser")
                .Options;

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

            _roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(_dbContext),
                null,
                null,
                null,
                null);

            _controller = new UsersController(_userManager);
        }

        [Fact]
        public async Task Index_Returns_ViewResult_With_UserViewModelList()
        {
            // Arrange
            var user1 = new ApplicationUser() { UserName = "user1@example.com", Email = "user1@example.com" };
            var user2 = new ApplicationUser() { UserName = "user2@example.com", Email = "user2@example.com" };

            var role1 = new IdentityRole("Admin") { NormalizedName = "ADMIN" };
            var role2 = new IdentityRole("User") { NormalizedName = "USER" };

            await _userManager.CreateAsync(user1);
            await _userManager.CreateAsync(user2);

            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);

            await _userManager.AddToRoleAsync(user1, "Admin");
            await _userManager.AddToRoleAsync(user2, "User");

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
            
            _dbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task ChangeRole_Returns_RedirectToAction_When_Successful()
        {
            // Arrange
            var user = new ApplicationUser() { UserName = "user3@example.com", Email = "user3@example.com" };
            await _userManager.CreateAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);

            var role1 = new IdentityRole("Admin") { NormalizedName = "ADMIN" };
            var role2 = new IdentityRole("User") { NormalizedName = "USER" };

            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role2);

            await _userManager.AddToRoleAsync(user, "User");

            // Act
            var result = await _controller.ChangeRole(userId, "User", "Admin");

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            
            _dbContext.Database.EnsureDeleted();
        }
    }
}
