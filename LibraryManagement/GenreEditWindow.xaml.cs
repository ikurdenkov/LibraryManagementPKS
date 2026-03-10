using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class GenreEditWindow : Window
    {
        private int? _genreId;

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
                var genre = context.Genres.Find(_genreId.Value);
                if (genre != null)
                {
                    txtName.Text = genre.Name;
                    txtDescription.Text = genre.Description;
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
                bool exists;
                if (_genreId.HasValue)
                {
                    exists = context.Genres.Any(g => g.Name == txtName.Text.Trim() && g.Id != _genreId.Value);
                }
                else
                {
                    exists = context.Genres.Any(g => g.Name == txtName.Text.Trim());
                }
                if (exists)
                {
                    MessageBox.Show("Жанр с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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
                    genre.Description = txtDescription.Text?.Trim() ?? "";

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