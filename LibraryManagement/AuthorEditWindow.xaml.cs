using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class AuthorEditWindow : Window
    {
        private int? _authorId;

        public AuthorEditWindow(int? authorId = null)
        {
            InitializeComponent();
            _authorId = authorId;
            if (_authorId.HasValue)
            {
                LoadAuthorData();
            }
        }

        private void LoadAuthorData()
        {
            using (var context = new LibraryContext())
            {
                var author = context.Authors.Find(_authorId.Value);
                if (author != null)
                {
                    txtFirstName.Text = author.FirstName;
                    txtLastName.Text = author.LastName;
                    dpBirthDate.SelectedDate = author.BirthDate;
                    txtCountry.Text = author.Country;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Имя и фамилия обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату рождения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new LibraryContext())
            {
                // Проверка уникальности (имя + фамилия)
                bool exists;
                if (_authorId.HasValue)
                {
                    exists = context.Authors.Any(a => a.FirstName == txtFirstName.Text.Trim()
                                                   && a.LastName == txtLastName.Text.Trim()
                                                   && a.Id != _authorId.Value);
                }
                else
                {
                    exists = context.Authors.Any(a => a.FirstName == txtFirstName.Text.Trim()
                                                   && a.LastName == txtLastName.Text.Trim());
                }
                if (exists)
                {
                    MessageBox.Show("Автор с таким именем и фамилией уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Author author;
                if (_authorId.HasValue)
                {
                    author = context.Authors.Find(_authorId.Value);
                }
                else
                {
                    author = new Author();
                    context.Authors.Add(author);
                }

                if (author != null)
                {
                    author.FirstName = txtFirstName.Text.Trim();
                    author.LastName = txtLastName.Text.Trim();
                    author.BirthDate = dpBirthDate.SelectedDate.Value;
                    author.Country = txtCountry.Text?.Trim() ?? "";

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