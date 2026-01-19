using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class NewBookingWindow : Window
    {
        private Tour _selectedTour;
        private decimal _tourPrice;
        private int _availableSeats;

        public NewBookingWindow()
        {
            InitializeComponent();
            LoadClients();
            LoadTours();
        }

        private void LoadClients()
        {
            try
            {
                var clients = DatabaseHelper.GetAllClients();
                cmbClient.ItemsSource = clients;
                if (clients.Count > 0)
                    cmbClient.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTours()
        {
            try
            {
                var tours = DatabaseHelper.GetAllTours()
                    .Where(t => t.AvailableSeats > 0)
                    .ToList();

                cmbTour.ItemsSource = tours;
                if (tours.Count > 0)
                    cmbTour.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbTour_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedTour = cmbTour.SelectedItem as Tour;
            if (_selectedTour != null)
            {
                _tourPrice = _selectedTour.Price;
                _availableSeats = _selectedTour.AvailableSeats;

                // Показываем информацию о туре
                tourInfoPanel.Visibility = Visibility.Visible;
                tbTourInfo.Text = $"Страна: {_selectedTour.CountryName}\n" +
                                 $"Дата: {_selectedTour.StartDate:dd.MM.yyyy}\n" +
                                 $"Длительность: {_selectedTour.DurationDays} дней\n" +
                                 $"Тип автобуса: {_selectedTour.BusTypeName}";

                tbTourSeats.Text = $"Свободных мест: {_availableSeats}";
                UpdateTotalPrice();
            }
            else
            {
                tourInfoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void TxtPeopleCount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            if (int.TryParse(txtPeopleCount.Text, out int peopleCount) && peopleCount > 0)
            {
                decimal totalPrice = _tourPrice * peopleCount;
                tbTotalPrice.Text = $"{totalPrice:N0} руб.";

                // Проверка на количество мест
                if (peopleCount > _availableSeats)
                {
                    tbTotalPrice.Foreground = System.Windows.Media.Brushes.Red;
                    tbTotalPrice.Text += $" (Недостаточно мест! Доступно: {_availableSeats})";
                }
                else
                {
                    tbTotalPrice.Foreground = System.Windows.Media.Brushes.Green;
                }
            }
            else
            {
                tbTotalPrice.Text = "0 руб.";
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (cmbClient.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbTour.SelectedItem == null)
            {
                MessageBox.Show("Выберите тур", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtPeopleCount.Text, out int peopleCount) || peopleCount < 1)
            {
                MessageBox.Show("Введите корректное количество человек", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (peopleCount > _availableSeats)
            {
                MessageBox.Show($"Недостаточно свободных мест! Доступно: {_availableSeats}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var client = cmbClient.SelectedItem as User;
            var tour = cmbTour.SelectedItem as Tour;
            decimal totalPrice = _tourPrice * peopleCount;
            string comment = txtComment.Text;

            // Создаем заявку
            if (DatabaseHelper.CreateBooking(tour.Id, client.Id, peopleCount, totalPrice, comment))
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