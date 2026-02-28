using System;
using System.Windows;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class AuthorEditWindow : Window
    {
        private int? _authorId;
        private Author _currentAuthor;

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
                _currentAuthor = context.Authors.Find(_authorId.Value);
                if (_currentAuthor != null)
                {
                    txtFirstName.Text = _currentAuthor.FirstName;
                    txtLastName.Text = _currentAuthor.LastName;
                    dpBirthDate.SelectedDate = _currentAuthor.BirthDate;
                    txtCountry.Text = _currentAuthor.Country;
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
                    author.Country = txtCountry.Text.Trim();

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