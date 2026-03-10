using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PublishYear { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }

        // Связь многие-ко-многим с авторами
        public ICollection<Author> Authors { get; set; } = new List<Author>();
        // Связь многие-ко-многим с жанрами
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();

        // Свойства для отображения в DataGrid
        public string AuthorsDisplay => Authors != null ? string.Join(", ", Authors.Select(a => a.FullName)) : "";
        public string GenresDisplay => Genres != null ? string.Join(", ", Genres.Select(g => g.Name)) : "";
    }
}