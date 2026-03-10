using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace LibraryManagement
{
    public partial class BookWindow : Window
    {
        private int? _bookId;
        private Book _currentBook;

        public BookWindow(int? bookId = null)
        {
            InitializeComponent();
            _bookId = bookId;
            LoadAuthorsAndGenres();
            if (_bookId.HasValue)
            {
                LoadBookData();
            }
        }

        private void LoadAuthorsAndGenres()
        {
            using (var context = new LibraryContext())
            {
                lstAuthors.ItemsSource = context.Authors.OrderBy(a => a.LastName).ToList();
                lstGenres.ItemsSource = context.Genres.OrderBy(g => g.Name).ToList();
            }
        }

        private void LoadBookData()
        {
            using (var context = new LibraryContext())
            {
                _currentBook = context.Books
                    .Include(b => b.Authors)
                    .Include(b => b.Genres)
                    .FirstOrDefault(b => b.Id == _bookId.Value);

                if (_currentBook != null)
                {
                    txtTitle.Text = _currentBook.Title;
                    txtYear.Text = _currentBook.PublishYear.ToString();
                    txtIsbn.Text = _currentBook.ISBN;
                    txtQuantity.Text = _currentBook.QuantityInStock.ToString();

                    foreach (var author in _currentBook.Authors)
                        lstAuthors.SelectedItems.Add(author);
                    foreach (var genre in _currentBook.Genres)
                        lstGenres.SelectedItems.Add(genre);
                }
            }
        }

        private bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return false;
            isbn = isbn.Replace("-", "").Replace(" ", "");
            if (isbn.Length == 10)
                return Regex.IsMatch(isbn, @"^\d{9}[\dX]$");
            if (isbn.Length == 13)
                return long.TryParse(isbn, out _);
            return false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (lstAuthors.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (lstGenres.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один жанр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtYear.Text, out int year) || year < 0 || year > DateTime.Now.Year)
            {
                MessageBox.Show("Введите корректный год издания (число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество (неотрицательное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string isbn = txtIsbn.Text.Trim();
            if (!string.IsNullOrEmpty(isbn) && !IsValidIsbn(isbn))
            {
                MessageBox.Show("Введите корректный ISBN (10 или 13 цифр, допускаются дефисы).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new LibraryContext())
            {
                Book book;
                if (_bookId.HasValue)
                {
                    book = context.Books
                        .Include(b => b.Authors)
                        .Include(b => b.Genres)
                        .FirstOrDefault(b => b.Id == _bookId.Value);
                }
                else
                {
                    book = new Book();
                    context.Books.Add(book);
                }

                if (book != null)
                {
                    book.Title = txtTitle.Text.Trim();
                    book.PublishYear = year;
                    book.ISBN = isbn;
                    book.QuantityInStock = quantity;

                    // Получаем ID выбранных авторов и жанров
                    var selectedAuthorIds = lstAuthors.SelectedItems.Cast<Author>().Select(a => a.Id).ToList();
                    var selectedGenreIds = lstGenres.SelectedItems.Cast<Genre>().Select(g => g.Id).ToList();

                    // Загружаем соответствующие сущности из текущего контекста
                    var selectedAuthors = context.Authors.Where(a => selectedAuthorIds.Contains(a.Id)).ToList();
                    var selectedGenres = context.Genres.Where(g => selectedGenreIds.Contains(g.Id)).ToList();

                    // Обновляем коллекции
                    book.Authors.Clear();
                    foreach (var author in selectedAuthors)
                        book.Authors.Add(author);

                    book.Genres.Clear();
                    foreach (var genre in selectedGenres)
                        book.Genres.Add(genre);

                    context.SaveChanges();
                }
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}