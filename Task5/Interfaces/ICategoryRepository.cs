using Task5.Models;

namespace Task5.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        //Category GetById(int id);
        void Add(Category category);
    }
}
