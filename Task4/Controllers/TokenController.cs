/*using System.Security.Claims;
using System.Web.Http;

namespace Task4.Controllers
{
    public class TokenController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> GetToken(string username, string password)
        {
            using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                var user = await userManager.FindAsync(username, password);

                if (user != null)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("username", user.UserName));
                    identity.AddClaim(new Claim("role", "user"));

                    var ticket = new AuthenticationTicket(identity, null);

                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                }
            }

            return Ok();
        }
    }
}*/
