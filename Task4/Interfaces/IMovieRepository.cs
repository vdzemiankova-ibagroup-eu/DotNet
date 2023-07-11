using Task4.Models;

namespace Task4.Interfaces
{
    public interface IMovieRepository
    {
        List<Movie> GetAll();
        Movie GetById(int id);
        void Add(Movie movie);
        void Update(Movie existingMovie, Movie updatedMovie);
        void Delete(int id);
    }
}
