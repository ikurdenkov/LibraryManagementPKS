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
            using (var context = new LibraryContext())
            {
                context.Database.Migrate(); // Применяет миграции, создаёт БД, если её нет
            }
            LoadFilters(); // Загружаем списки для фильтров
            LoadBooks();    // Загружаем книги
        }

        // Загрузка авторов и жанров в ComboBox фильтров
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

        // Загрузка книг с учётом фильтров
        private void LoadBooks()
        {
            using (var context = new LibraryContext())
            {
                // Базовый запрос с включением связанных данных
                IQueryable<Book> query = context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre);

                // Фильтр по автору (если выбран не "Все")
                if (cmbAuthorFilter.SelectedValue != null && (int)cmbAuthorFilter.SelectedValue != 0)
                {
                    int authorId = (int)cmbAuthorFilter.SelectedValue;
                    query = query.Where(b => b.AuthorId == authorId);
                }

                // Фильтр по жанру
                if (cmbGenreFilter.SelectedValue != null && (int)cmbGenreFilter.SelectedValue != 0)
                {
                    int genreId = (int)cmbGenreFilter.SelectedValue;
                    query = query.Where(b => b.GenreId == genreId);
                }

                // Поиск по названию
                if (!string.IsNullOrWhiteSpace(txtSearchTitle.Text))
                {
                    string search = txtSearchTitle.Text;
                    query = query.Where(b => b.Title.Contains(search));
                }

                // Выполняем запрос и материализуем список
                var books = query.ToList();
                dgBooks.ItemsSource = books;

                // Подсчёт общего количества книг в наличии
                int totalQuantity = books.Sum(b => b.QuantityInStock);
                txtTotalQuantity.Text = totalQuantity.ToString();
            }
        }

        // Обработчик кнопки "Применить"
        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
        }

        // Обработчик кнопки "Сброс"
        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbAuthorFilter.SelectedValue = 0;
            cmbGenreFilter.SelectedValue = 0;
            txtSearchTitle.Text = "";
            LoadBooks();
        }

        // Добавление книги
        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var window = new BookWindow(); // окно для добавления/редактирования (создадим позже)
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                LoadBooks(); // обновляем список после добавления
                LoadFilters(); // возможно, появились новые авторы/жанры
            }
        }

        // Редактирование книги
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

        // Удаление книги
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

        // Открыть окно управления авторами
        private void btnManageAuthors_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthorsWindow();
            window.Owner = this;
            window.ShowDialog();
            LoadFilters();
            LoadBooks();
        }

        // Открыть окно управления жанрами
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