using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class AuthorsWindow : Window
    {
        public AuthorsWindow()
        {
            InitializeComponent();
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            using (var context = new LibraryContext())
            {
                dgAuthors.ItemsSource = context.Authors.OrderBy(a => a.LastName).ToList();
            }
        }

        private void btnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthorEditWindow();
            window.Owner = this;
            if (window.ShowDialog() == true)
                LoadAuthors();
        }

        private void btnEditAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (dgAuthors.SelectedItem is Author selectedAuthor)
            {
                var window = new AuthorEditWindow(selectedAuthor.Id);
                window.Owner = this;
                if (window.ShowDialog() == true)
                    LoadAuthors();
            }
            else
            {
                MessageBox.Show("Выберите автора для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (dgAuthors.SelectedItem is Author selectedAuthor)
            {
                var result = MessageBox.Show($"Удалить автора {selectedAuthor.FullName}?", "Подтверждение",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new LibraryContext())
                    {
                        context.Authors.Remove(selectedAuthor);
                        context.SaveChanges();
                    }
                    LoadAuthors();
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}