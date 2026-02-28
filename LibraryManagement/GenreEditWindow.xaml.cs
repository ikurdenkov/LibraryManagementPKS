using System;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class GenreEditWindow : Window
    {
        private int? _genreId;
        private Genre _currentGenre;

        public GenreEditWindow(int? genreId = null)
        {
            InitializeComponent();
            _genreId = genreId;
            if (_genreId.HasValue)
            {
                LoadGenreData();
            }
        }

        private void LoadGenreData()
        {
            using (var context = new LibraryContext())
            {
                _currentGenre = context.Genres.Find(_genreId.Value);
                if (_currentGenre != null)
                {
                    txtName.Text = _currentGenre.Name;
                    txtDescription.Text = _currentGenre.Description;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Название жанра обязательно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new LibraryContext())
            {
                Genre genre;
                if (_genreId.HasValue)
                {
                    genre = context.Genres.Find(_genreId.Value);
                }
                else
                {
                    genre = new Genre();
                    context.Genres.Add(genre);
                }

                if (genre != null)
                {
                    genre.Name = txtName.Text.Trim();
                    genre.Description = txtDescription.Text?.Trim();

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