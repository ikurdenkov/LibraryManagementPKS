using System;
using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        // Свойство для отображения полного имени (будем использовать в интерфейсе)
        public string FullName => $"{FirstName} {LastName}";

        // Связь: у автора может быть много книг
        public ICollection<Book> Books { get; set; }
    }
}