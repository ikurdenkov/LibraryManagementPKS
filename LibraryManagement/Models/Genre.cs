using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Связь: у жанра может быть много книг
        public ICollection<Book> Books { get; set; }
    }
}