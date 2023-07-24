using Task5.Models;

namespace Task5.Interfaces
{
    public interface ICommentRepository
    {
        List<Comment> GetAll();
        Comment GetById(int id);
        void Add(Comment comment);
        Comment Delete(int id);
    }
}
