namespace Task5.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; }
        public string? UserComment { get; set; }
        public decimal UserGrade { get; set; }
    }
}
