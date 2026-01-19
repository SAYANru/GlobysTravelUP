using System.Net.NetworkInformation;
using System.Windows;
using GlobusTravelManager.Database;

namespace GlobusTravelManager.Views
{
    public partial class GuestToursWindow : Window
    {
        public GuestToursWindow()
        {
            InitializeComponent();
            LoadTours();
        }

        private void LoadTours()
        {
            try
            {
                var tours = DatabaseHelper.GetAllTours();
                dgTours.ItemsSource = tours;
                tbStatus.Text = $"Загружено туров: {tours.Count}";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Возврат к окну авторизации
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
