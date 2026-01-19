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
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            // TODO: Проверка в БД
            // Временная заглушка
            if (login == "admin@globus.ru" && password == "7f8d2a")
            {
                MessageBox.Show("Вход выполнен!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Открываем главное окно
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
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
