using Microsoft.AspNetCore.Identity;

namespace Task5.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUserRole() : base() { }
    }
}
