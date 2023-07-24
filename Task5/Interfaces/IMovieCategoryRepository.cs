using Task5.Models;

namespace Task5.Interfaces
{
    public interface IMovieCategoryRepository
    {
        List<MovieCategory> GetAll();
        List<MovieCategory> GetByMovieId(int movieId);
        List<int> GetIdsByMovieId(int movieId);
        void Add(MovieCategory movieCategory);
        void DeleteByMovieId(int movieId);
    }
}
