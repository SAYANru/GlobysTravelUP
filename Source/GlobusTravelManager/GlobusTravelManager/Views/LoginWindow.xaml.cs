using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace GlobusTravelManager
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
            // Открываем окно для гостя
            GuestToursWindow guestWindow = new GuestToursWindow();
            guestWindow.Show();
            this.Close();
        }
    }
}
