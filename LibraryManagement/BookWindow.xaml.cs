using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class BookWindow : Window
    {
        private int? _bookId; // null для новой книги, иначе ID редактируемой
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
                cmbAuthor.ItemsSource = context.Authors.OrderBy(a => a.LastName).ToList();
                cmbGenre.ItemsSource = context.Genres.OrderBy(g => g.Name).ToList();
            }
        }

        private void LoadBookData()
        {
            using (var context = new LibraryContext())
            {
                _currentBook = context.Books.Find(_bookId.Value);
                if (_currentBook != null)
                {
                    txtTitle.Text = _currentBook.Title;
                    cmbAuthor.SelectedValue = _currentBook.AuthorId;
                    cmbGenre.SelectedValue = _currentBook.GenreId;
                    txtYear.Text = _currentBook.PublishYear.ToString();
                    txtIsbn.Text = _currentBook.ISBN;
                    txtQuantity.Text = _currentBook.QuantityInStock.ToString();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения обязательных полей
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbAuthor.SelectedValue == null)
            {
                MessageBox.Show("Выберите автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbGenre.SelectedValue == null)
            {
                MessageBox.Show("Выберите жанр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            using (var context = new LibraryContext())
            {
                Book book;
                if (_bookId.HasValue)
                {
                    book = context.Books.Find(_bookId.Value);
                }
                else
                {
                    book = new Book();
                    context.Books.Add(book);
                }

                if (book != null)
                {
                    book.Title = txtTitle.Text.Trim();
                    book.AuthorId = (int)cmbAuthor.SelectedValue;
                    book.GenreId = (int)cmbGenre.SelectedValue;
                    book.PublishYear = year;
                    book.ISBN = txtIsbn.Text.Trim();
                    book.QuantityInStock = quantity;

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