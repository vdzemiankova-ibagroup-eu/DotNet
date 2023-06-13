using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task4.Models;

namespace Task4
{
    public class MovieRepositoryFactory : IRepositoryFactory
    {
        private readonly DbContextOptions _dbContextOptions;

        public MovieRepositoryFactory(DbContextOptions dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public IMovieRepository CreateMovieRepository()
        {
            return new MovieRepository(new MoviesContext(_dbContextOptions as DbContextOptions<MoviesContext>));
        }
    }
}
