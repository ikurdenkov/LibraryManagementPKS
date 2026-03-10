using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Применяем миграции при запуске (если база не существует или не обновлена)
            using (var context = new LibraryContext())
            {
                context.Database.Migrate();
            }

            LoadFilters();
            LoadBooks();
        }

        private void LoadFilters()
        {
            using (var context = new LibraryContext())
            {
                var authors = context.Authors.OrderBy(a => a.LastName).ToList();
                authors.Insert(0, new Author { Id = 0, FirstName = "Все", LastName = "" });
                cmbAuthorFilter.ItemsSource = authors;
                cmbAuthorFilter.SelectedValue = 0;

                var genres = context.Genres.OrderBy(g => g.Name).ToList();
                genres.Insert(0, new Genre { Id = 0, Name = "Все" });
                cmbGenreFilter.ItemsSource = genres;
                cmbGenreFilter.SelectedValue = 0;
            }
        }

        private void LoadBooks()
        {
            using (var context = new LibraryContext())
            {
                IQueryable<Book> query = context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres);

                if (cmbAuthorFilter.SelectedValue != null && (int)cmbAuthorFilter.SelectedValue != 0)
                {
                    int authorId = (int)cmbAuthorFilter.SelectedValue;
                    query = query.Where(b => b.Authors.Any(a => a.Id == authorId));
                }

                if (cmbGenreFilter.SelectedValue != null && (int)cmbGenreFilter.SelectedValue != 0)
                {
                    int genreId = (int)cmbGenreFilter.SelectedValue;
                    query = query.Where(b => b.Genres.Any(g => g.Id == genreId));
                }

                if (!string.IsNullOrWhiteSpace(txtSearchTitle.Text))
                {
                    string search = txtSearchTitle.Text;
                    query = query.Where(b => b.Title.Contains(search));
                }

                var books = query.ToList();
                dgBooks.ItemsSource = books;

                int totalQuantity = books.Sum(b => b.QuantityInStock);
                txtTotalQuantity.Text = totalQuantity.ToString();
            }
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e) => LoadBooks();
        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbAuthorFilter.SelectedValue = 0;
            cmbGenreFilter.SelectedValue = 0;
            txtSearchTitle.Text = "";
            LoadBooks();
        }

        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var window = new BookWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadBooks();
                LoadFilters();
            }
        }

        private void btnEditBook_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem is Book selectedBook)
            {
                var window = new BookWindow(selectedBook.Id);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    LoadBooks();
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу \"{selectedBook.Title}\"?", "Подтверждение",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        context.Books.Remove(selectedBook);
                        context.SaveChanges();
                    }
                    LoadBooks();
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnManageAuthors_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthorsWindow();
            window.Owner = this;
            window.ShowDialog();
            LoadFilters();
            LoadBooks();
        }

        private void btnManageGenres_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenresWindow();
            window.Owner = this;
            window.ShowDialog();
            LoadFilters();
            LoadBooks();
        }
    }
}