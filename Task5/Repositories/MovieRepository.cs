using Task5.Interfaces;
using Task5.Models;

namespace Task5.Repositories
{
    public class MovieRepository: IMovieRepository
    {
        private IMovieApi _movieApi;
        private HttpClient _movieHTTPClient;

        public MovieRepository(IMovieApi movieApi, IHttpClientFactory httpClientFactory)
        {
            _movieApi = movieApi;
            _movieHTTPClient = httpClientFactory.CreateClient("movieHTTPClient");
        }

        public async Task<List<Movie>> GetAllAsync()
        {
            return await _movieHTTPClient.GetFromJsonAsync<List<Movie>>("api/Movie");
        }

        public async Task<Movie> GetByIdAsync(int id)
        {
            return await _movieHTTPClient.GetFromJsonAsync<Movie>($"api/Movie/{id}");
        }

        public async Task<List<Movie>> GetAllRestEaseAsync()
        {
            return await _movieApi.GetAllAsync();
        }

        public async Task<Movie> GetByIdRestEaseAsync(int id)
        {
            return await _movieApi.GetByIdAsync(id);
        }

        public async Task<Movie> AddAsync(Movie movie)
        {
            return await _movieApi.AddAsync(movie);
        }

        public async Task<Movie> UpdateAsync(Movie movie)
        {
            return await _movieApi.UpdateAsync(movie.Id, movie);
        }

        public async Task DeleteAsync(int id)
        {
            await _movieApi.DeleteAsync(id);
        }
    }
}
