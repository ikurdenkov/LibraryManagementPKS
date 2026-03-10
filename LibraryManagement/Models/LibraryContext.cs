using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Models
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=library.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(50);
                entity.Property(a => a.Country).HasMaxLength(50);
            });

            // Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(50);
                entity.Property(g => g.Description).HasMaxLength(200);
            });

            // Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(100);
                entity.Property(b => b.ISBN).HasMaxLength(20);
                entity.Property(b => b.PublishYear).IsRequired();
                entity.Property(b => b.QuantityInStock).IsRequired();

                // Настройка связи многие-ко-многим с Author
                entity.HasMany(b => b.Authors)
                    .WithMany(a => a.Books)
                    .UsingEntity(j => j.ToTable("BookAuthors"));

                // Настройка связи многие-ко-многим с Genre
                entity.HasMany(b => b.Genres)
                    .WithMany(g => g.Books)
                    .UsingEntity(j => j.ToTable("BookGenres"));
            });
        }
    }
}