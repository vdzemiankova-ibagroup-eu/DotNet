using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2
{
    public sealed class MovieMap : ClassMap<Movie>
    {
        public MovieMap()
        {
            Map(m => m.Id).Ignore();
            Map(m => m.FirstName).Index(0);
            Map(m => m.LastName).Index(1);
            Map(m => m.MovieName).Index(2);
            Map(m => m.MovieYear).Index(3);
            Map(m => m.MovieRating).Index(4);
        }
    }
}
