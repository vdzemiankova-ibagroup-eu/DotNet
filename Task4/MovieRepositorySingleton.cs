using Microsoft.EntityFrameworkCore;
using Task4.Models;

namespace Task4
{
    public class MovieRepositorySingleton : DbContext
    {
        private static MovieRepository _instance;
        private static readonly object _lock = new object();

        public static MovieRepository GetInstance(DbContext context)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new MovieRepository(context);
                }
                return _instance;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MyDatabase;Integrated Security=True");
        }

        public DbSet<Movie> Movies { get; set; }
    }
}
