using Microsoft.EntityFrameworkCore;
using Task4.Interfaces;
using Task4.Models;

namespace Task4.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DbContext _context;

        public MovieRepository(DbContext context)
        {
            _context = context;
        }

        public List<Movie> GetAll()
        {
            return _context.Set<Movie>().ToList();
        }

        public Movie GetById(int id)
        {
            return _context.Set<Movie>().Find(id);
        }

        public void Add(Movie movie)
        {
            _context.Set<Movie>().Add(movie);
            _context.SaveChanges();
        }

        public void Update(Movie existingMovie, Movie updatedMovie)
        {
            existingMovie.FirstName = updatedMovie.FirstName;
            existingMovie.LastName = updatedMovie.LastName;
            existingMovie.MovieName = updatedMovie.MovieName;
            existingMovie.MovieYear = updatedMovie.MovieYear;
            existingMovie.MovieRating = updatedMovie.MovieRating;

            _context.Entry(existingMovie).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _context.Set<Movie>().Remove(GetById(id));
            _context.SaveChanges();
        }
    }
}
