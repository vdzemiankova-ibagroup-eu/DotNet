using Abp.Extensions;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task5.Interfaces;
using Task5.Models;

namespace Task5.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private IUserManagerRepository _userManagerRepository;

        public UsersController(IUserManagerRepository userManagerRepository)
        {
            _userManagerRepository = userManagerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var listUserViewModel = await _userManagerRepository.GetAllUserViewModel();

            return View(listUserViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string oldRole, string newRole)
        {
            await _userManagerRepository.ChangeUserRole(userId, oldRole, newRole);

            return RedirectToAction("Index", "Users");
        }
    }
}
