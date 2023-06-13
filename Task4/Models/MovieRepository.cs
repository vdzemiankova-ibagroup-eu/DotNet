using Microsoft.EntityFrameworkCore;

namespace Task4.Models
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

        public void Update(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            _context.Set<Movie>().Remove(GetById(id));
            _context.SaveChanges();
        }
    }
}
