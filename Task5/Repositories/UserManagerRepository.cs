using Microsoft.AspNetCore.Identity;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Task5.Interfaces;
using Task5.Models;

namespace Task5.Repositories
{
    public class UserManagerRepository: IUserManagerRepository
    {
        private Data.ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerRepository(Data.ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            SeedData();
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }

        public async Task<List<UserViewModel>> GetAllUserViewModel()
        {
            var users = GetAllUsers();

            var listUserViewModel = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRoles = await GetRolesByUserAsync(user);
                var userViewModel = new UserViewModel() { UserId = user.Id, UserName = user.UserName, Email = user.Email, Role = string.Join(", ", userRoles) };
                listUserViewModel.Add(userViewModel);
            }
            return listUserViewModel;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task RemoveUserFromRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task AddUserToRoleAsync(ApplicationUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task ChangeUserRole(string userId, string oldRole, string newRole)
        {
            var user = await GetUserByIdAsync(userId);

            if (!oldRole.IsNullOrEmpty())
            {
                await RemoveUserFromRoleAsync(user, oldRole);
            }
            await AddUserToRoleAsync(user, newRole);
        }

        public async Task<IList<String>> GetRolesByUserAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        private async void SeedData()
        {
            string[] roles = new string[] { "Admin", "User" };
            foreach (string role in roles)
            {
                if (!_dbContext.IdentityUserRole.Any(r => r.Name == role))
                {
                    _dbContext.IdentityUserRole.Add(new IdentityRole(role) { NormalizedName = role.ToUpper() });
                    _dbContext.SaveChanges();
                }
            }

            if (!_dbContext.ApplicationUsers.Any(u => u.UserName == "admin@gmail.com"))
            {
                var user = new ApplicationUser { UserName = "admin@gmail.com", Email = "admin@gmail.com" };
                var result = await _userManager.CreateAsync(user, "gkm%3/v8vQv9c9R");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                _dbContext.SaveChanges();
            }
        }
    }
}
