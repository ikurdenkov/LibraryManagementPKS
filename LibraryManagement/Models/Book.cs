namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PublishYear { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }

        // Внешний ключ для автора
        public int AuthorId { get; set; }
        public Author Author { get; set; } // навигационное свойство

        // Внешний ключ для жанра
        public int GenreId { get; set; }
        public Genre Genre { get; set; }   // навигационное свойство
    }
}