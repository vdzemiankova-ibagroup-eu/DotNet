using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Task4.Models
{
    public class MoviesContext : DbContext
    {
        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
    }
}