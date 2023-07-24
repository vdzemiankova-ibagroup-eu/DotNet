using Abp.Runtime.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task5.Interfaces;
using Task5.Models;

namespace Task5.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private ICommentRepository _commentRepository;

        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        [HttpPost]
        public IActionResult Add(int movieId, decimal grade, string comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _commentRepository.Add(new Comment { MovieId = movieId, UserComment = comment, UserGrade = grade, UserId = userId });

            return RedirectToAction("Details", "Movies", new { id = movieId});
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var comment = _commentRepository.Delete(id);

            return RedirectToAction("Details", "Movies", new { id = comment.MovieId });
        }
    }
}
