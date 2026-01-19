using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class ClientsPage : Page
    {
        private List<User> _allClients = new List<User>();

        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                _allClients = DatabaseHelper.GetAllClients();

                // Добавляем информацию о количестве заявок
                foreach (var client in _allClients)
                {
                    // В реальном приложении нужно сделать запрос к БД
                    client.BookingCount = new Random().Next(0, 10); // Заглушка
                }

                dgClients.ItemsSource = _allClients;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            tbClientCount.Text = _allClients.Count.ToString();
            tbTotalBookings.Text = _allClients.Sum(c => c.BookingCount).ToString();
        }

        private User GetSelectedClient()
        {
            return dgClients.SelectedItem as User;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var client = GetSelectedClient();
            if (client == null)
            {
                MessageBox.Show("Выберите клиента для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new ClientEditWindow(client);
            if (dialog.ShowDialog() == true)
            {
                LoadClients();
                MessageBox.Show("Данные клиента обновлены успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var client = GetSelectedClient();
            if (client == null)
            {
                MessageBox.Show("Выберите клиента для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить клиента '{client.FullName}'?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (DatabaseHelper.DeleteClient(client.Id))
                {
                    MessageBox.Show("Клиент удален успешно!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadClients();
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
            MessageBox.Show("Список клиентов обновлен", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}