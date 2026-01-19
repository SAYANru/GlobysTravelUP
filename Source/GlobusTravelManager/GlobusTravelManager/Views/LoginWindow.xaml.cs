using GlobusTravelManager.Models;
using System.Windows;

namespace GlobusTravelManager.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Тестируем подключение
            if (!Database.DatabaseHelper.TestConnection())
                return;

            // Авторизация
            var user = Database.DatabaseHelper.AuthenticateUser(login, password);

            if (user != null)
            {
                // Сохраняем пользователя в статическом классе
                Session.CurrentUser = user;

                MessageBox.Show($"Добро пожаловать, {user.FullName}!", "Успешный вход",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Открываем главное окно менеджера/админа
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null; // Гостевой режим
            GuestToursWindow guestWindow = new GuestToursWindow();
            guestWindow.Show();
            this.Close();
        }
    }
}
