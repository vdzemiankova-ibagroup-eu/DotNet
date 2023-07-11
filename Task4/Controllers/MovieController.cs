using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4.Interfaces;
using Task4.Models;

namespace Task4.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _repository;

        public MovieController(IMovieRepository movieRepository)
        {
            _repository = movieRepository;
        }

        [HttpGet]
        public ActionResult<List<Movie>> GetAll()
        {
            var movies = _repository.GetAll();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var movie = _repository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [HttpPost]
        public IActionResult Add(Movie movie)
        {
            _repository.Add(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Movie movie)
        {
            var existingMovie = _repository.GetById(id);
            if (existingMovie == null)
            {
                return NotFound();
            }

            _repository.Update(existingMovie, movie);
            return Ok(existingMovie);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingUser = _repository.GetById(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            _repository.Delete(id);
            return Ok(existingUser);
        }
    }
}
