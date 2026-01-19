using System.Windows;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class ClientEditWindow : Window
    {
        private readonly User _client;

        public ClientEditWindow(User client)
        {
            InitializeComponent();
            _client = client;
            LoadClientData();
        }

        private void LoadClientData()
        {
            if (_client != null)
            {
                txtFullName.Text = _client.FullName;
                txtLogin.Text = _client.Login;
                // Пароль не показываем из соображений безопасности
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО клиента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин (email)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string password = txtPassword.Password;
            string passwordConfirm = txtPasswordConfirm.Password;

            if (!string.IsNullOrEmpty(password) && password != passwordConfirm)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Если пароль не изменен, оставляем старый
            if (string.IsNullOrEmpty(password))
            {
                password = _client.Password;
            }

            string fullName = txtFullName.Text.Trim();
            string login = txtLogin.Text.Trim();

            if (DatabaseHelper.UpdateClient(_client.Id, fullName, login, password))
            {
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}