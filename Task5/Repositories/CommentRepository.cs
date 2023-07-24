using System.Xml.Linq;
using Task5.Interfaces;
using Task5.Migrations;
using Task5.Models;

namespace Task5.Repositories
{
    public class CommentRepository: ICommentRepository
    {
        private Data.ApplicationDbContext _dbContext;

        public CommentRepository(Data.ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Comment> GetAll()
        {
            return _dbContext.Comments.ToList();
        }

        public Comment GetById(int id)
        {
            return _dbContext.Comments.Find(id);
        }

        public void Add(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();
        }

        public Comment Delete(int id)
        {
            var comment = GetById(id);
            _dbContext.Comments.Remove(GetById(id));
            _dbContext.SaveChanges();
            return comment;
        }
    }
}
