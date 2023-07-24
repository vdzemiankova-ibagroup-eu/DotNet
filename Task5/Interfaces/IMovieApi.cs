using RestEase;
using Task5.Models;

namespace Task5.Interfaces
{
    [BasePath("api/Movie")]
    public interface IMovieApi
    {
        [Header("Authorization")]
        string Authorization { get; set; }

        [Get("{id}")]
        [Header("Authorization", "Bearer")]
        Task<Movie> GetByIdAsync([Path] int id);

        [Get("")]
        [Header("Authorization", "Bearer")]
        Task<List<Movie>> GetAllAsync();

        [Post("")]
        [Header("Authorization", "Bearer")]
        Task<Movie> AddAsync([Body] Movie movie);

        [Put("{id}")]
        [Header("Authorization", "Bearer")]
        Task<Movie> UpdateAsync([Path] int id, [Body] Movie movie);

        [Delete("{id}")]
        [Header("Authorization", "Bearer")]
        Task<Movie> DeleteAsync([Path] int id);
    }
}
