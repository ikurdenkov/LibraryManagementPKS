using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
    public class LibraryContext : DbContext
    {
        // Таблицы (наборы сущностей)
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        // Настройка подключения к базе данных
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // База данных будет в файле library.db в папке с программой
            optionsBuilder.UseSqlite("Data Source=library.db");
        }

        // Конфигурация сущностей с помощью Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id); // первичный ключ (можно не писать, но для демо)

                entity.Property(a => a.FirstName)
                    .IsRequired()           // обязательное поле
                    .HasMaxLength(50);       // максимум 50 символов

                entity.Property(a => a.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(a => a.Country)
                    .HasMaxLength(50);

                // Каскадное удаление: если удалить автора, удалятся и его книги
                entity.HasMany(a => a.Books)
                    .WithOne(b => b.Author)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(g => g.Description)
                    .HasMaxLength(200);

                entity.HasMany(g => g.Books)
                    .WithOne(b => b.Genre)
                    .HasForeignKey(b => b.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.ISBN)
                    .HasMaxLength(20);

                entity.Property(b => b.PublishYear)
                    .IsRequired();

                entity.Property(b => b.QuantityInStock)
                    .IsRequired();

                // Связи (можно и не указывать, они уже определены через навигационные свойства,
                // но для ясности пропишем)
                entity.HasOne(b => b.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId);

                entity.HasOne(b => b.Genre)
                    .WithMany(g => g.Books)
                    .HasForeignKey(b => b.GenreId);
            });
        }
    }
}