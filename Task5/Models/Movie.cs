using System.ComponentModel.DataAnnotations;

namespace Task5.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string MovieName { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Range(1895, 2030, ErrorMessage = "Год выпуска фильма должен быть от 1895 до 2030")]
        public int MovieYear { get; set; }
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Range(0.00, 10.00, ErrorMessage = "Рейтинг фильма должен быть десятичным числом от 0 до 10")]
        public decimal MovieRating { get; set; }
    }
}
