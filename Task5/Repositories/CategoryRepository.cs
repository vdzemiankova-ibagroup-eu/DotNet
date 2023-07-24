using Task5.Interfaces;
using Task5.Models;

namespace Task5.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private Data.ApplicationDbContext _dbContext;

        public CategoryRepository(Data.ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Category> GetAll() 
        { 
            return _dbContext.Categories.ToList();
        }
        /*private Category GetById(int id)
        {

        }*/
        public void Add(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
        }
    }
}
