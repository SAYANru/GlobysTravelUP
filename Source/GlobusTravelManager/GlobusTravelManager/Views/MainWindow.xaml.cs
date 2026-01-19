using System.Windows;
using System.Windows.Controls;
using GlobusTravelManager.Models;
using GlobusTravelManager.Views;

namespace GlobusTravelManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            ShowToursPage();
        }

        private void LoadUserInfo()
        {
            if (Session.CurrentUser != null)
            {
                tbUserName.Text = $"{Session.CurrentUser.Role}: {Session.CurrentUser.FullName}";
                tbStatus.Text = $"Добро пожаловать, {Session.CurrentUser.FullName}!";

                // Настройка доступных кнопок в зависимости от роли
                if (Session.CurrentUser.Role == "Менеджер")
                {
                    BtnClients.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                tbUserName.Text = "Гость";
            }
        }

        private void ShowToursPage()
        {
            var toursPage = new ToursPage();
            MainFrame.Navigate(toursPage);
            tbStatus.Text = "Режим управления турами";
        }

        private void BtnTours_Click(object sender, RoutedEventArgs e)
        {
            ShowToursPage();
        }

        private void BtnBookings_Click(object sender, RoutedEventArgs e)
        {
            var bookingsPage = new BookingsPage();
            MainFrame.Navigate(bookingsPage);
            tbStatus.Text = "Режим управления заявками";
        }

        private void BtnBuses_Click(object sender, RoutedEventArgs e)
        {
            var busesPage = new BusesPage();
            MainFrame.Navigate(busesPage);
            tbStatus.Text = "Режим управления автобусами";
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            var clientsPage = new ClientsPage();
            MainFrame.Navigate(clientsPage);
            tbStatus.Text = "Режим управления клиентами";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из системы?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Session.CurrentUser = null;
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Закрыть приложение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}