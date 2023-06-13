using Microsoft.EntityFrameworkCore;

namespace Task4.Models
{
    public class MoviesContext : DbContext
    {
        private static MoviesContext _instance;

        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }

        public static MoviesContext GetInstance()
        {
            if (_instance == null)
            {
                var optionsBuilder = new DbContextOptionsBuilder<MoviesContext>();
                //optionsBuilder.UseSqlServer("@\"Server=(localdb)\\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;\"");
                _instance = new MoviesContext(optionsBuilder.Options);
                //_instance = new MoviesContext();
            }
            return _instance;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=helloappdb;Trusted_Connection=True;");
        }

        public DbSet<Movie> Movies { get; set; }
    }
}