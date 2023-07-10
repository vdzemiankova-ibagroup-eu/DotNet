using Abp.Runtime.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task5.Models;

namespace Task5.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private Data.ApplicationDbContext _dbContext;

        public CommentsController(Data.ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Add(int movieId, decimal grade, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _dbContext.Comments.Add(new Comment { MovieId = movieId, UserComment = comment, UserGrade = grade, UserId = userId});
            
            _dbContext.SaveChanges();
            return RedirectToAction("Details", "Movies", new { id = movieId});
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var comment = _dbContext.Comments.Find(id);
            _dbContext.Comments.Remove(comment);
            _dbContext.SaveChanges();

            return RedirectToAction("Details", "Movies", new { id = comment.MovieId });
        }
    }
}
