using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task5.Models;

namespace Task5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var listUserViewModel = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userViewModel = new UserViewModel() { UserId = user.Id, UserName = user.UserName, Email = user.Email, Role = string.Join(", ", userRoles) };
                listUserViewModel.Add(userViewModel);
            }
            return View(listUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string oldRole, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (!oldRole.IsNullOrEmpty())
            {
                await _userManager.RemoveFromRoleAsync(user, oldRole);
            }
            await _userManager.AddToRoleAsync(user, newRole);

            return RedirectToAction("Index", "Users");
        }
    }
}
