using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task4.Models
{
    public class Movie
    {
        public int Id { get; internal set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MovieName { get; set; }
        public int MovieYear { get; set; }
        public decimal MovieRating { get; set; }
    }
}
