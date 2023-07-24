using Task5.Interfaces;
using Task5.Models;

namespace Task5.Repositories
{
    public class MovieCategoryRepository: IMovieCategoryRepository
    {
        private Data.ApplicationDbContext _dbContext;

        public MovieCategoryRepository(Data.ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MovieCategory> GetAll()
        {
            return _dbContext.MovieCategories.ToList();
        }

        public List<MovieCategory> GetByMovieId(int movie)
        {
            return _dbContext.MovieCategories.Where(x => x.MovieId == movie).ToList();
        }

        public List<int> GetIdsByMovieId(int movie)
        {
            return _dbContext.MovieCategories.Where(x => x.MovieId == movie).Select(x => x.CategoryId).ToList();
        }

        public void Add(MovieCategory movieCategory)
        {
            _dbContext.MovieCategories.Add(movieCategory);
            _dbContext.SaveChanges();
        }

        public void DeleteByMovieId(int movieId)
        {
            var itemsToRemove = GetByMovieId(movieId);
            _dbContext.MovieCategories.RemoveRange(itemsToRemove);
            _dbContext.SaveChanges();
        }
    }
}
