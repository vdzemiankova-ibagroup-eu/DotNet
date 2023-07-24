using Task5.Models;

namespace Task5.Interfaces
{
    public interface IUserManagerRepository
    {
        List<ApplicationUser> GetAllUsers();
        Task<List<UserViewModel>> GetAllUserViewModel();
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task RemoveUserFromRoleAsync(ApplicationUser user, string role);
        Task AddUserToRoleAsync(ApplicationUser user, string role);
        Task ChangeUserRole(string userId, string oldRole, string newRole);
        Task<IList<String>> GetRolesByUserAsync(ApplicationUser user);

    }
}
