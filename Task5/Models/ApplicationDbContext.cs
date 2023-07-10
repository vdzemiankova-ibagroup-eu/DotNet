using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace Task5.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("helloappdb") { }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }
    }

}
