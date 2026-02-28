using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class GenresWindow : Window
    {
        public GenresWindow()
        {
            InitializeComponent();
            LoadGenres();
        }

        private void LoadGenres()
        {
            using (var context = new LibraryContext())
            {
                dgGenres.ItemsSource = context.Genres.OrderBy(g => g.Name).ToList();
            }
        }

        private void btnAddGenre_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenreEditWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
                LoadGenres();
        }

        private void btnEditGenre_Click(object sender, RoutedEventArgs e)
        {
            if (dgGenres.SelectedItem is Genre selectedGenre)
            {
                var window = new GenreEditWindow(selectedGenre.Id);
                window.Owner = this;
                if (window.ShowDialog() == true)
                    LoadGenres();
            }
            else
            {
                MessageBox.Show("Выберите жанр для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            if (dgGenres.SelectedItem is Genre selectedGenre)
            {
                var result = MessageBox.Show($"Удалить жанр \"{selectedGenre.Name}\"?", "Подтверждение",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        context.Genres.Remove(selectedGenre);
                        context.SaveChanges();
                    }
                    LoadGenres();
                }
            }
            else
            {
                MessageBox.Show("Выберите жанр для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}