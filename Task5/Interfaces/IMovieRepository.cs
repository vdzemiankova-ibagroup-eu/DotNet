using Task5.Models;

namespace Task5.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync();
        Task<Movie> GetByIdAsync(int id);
        Task<List<Movie>> GetAllRestEaseAsync();
        Task<Movie> GetByIdRestEaseAsync(int id);
        Task<Movie> AddAsync(Movie movie);
        Task<Movie> UpdateAsync(Movie movie);
        Task DeleteAsync(int id);
    }
}
