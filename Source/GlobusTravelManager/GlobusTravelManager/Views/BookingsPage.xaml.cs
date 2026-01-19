using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class BookingsPage : Page
    {
        private List<Booking> _allBookings = new List<Booking>();

        public BookingsPage()
        {
            InitializeComponent();
            LoadBookings();
            LoadStatuses();
        }

        private void LoadBookings()
        {
            try
            {
                _allBookings = DatabaseHelper.GetAllBookings();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatuses()
        {
            cmbStatus.Items.Clear();
            var statuses = DatabaseHelper.GetBookingStatuses();

            foreach (var status in statuses)
            {
                cmbStatus.Items.Add(status);
            }

            cmbStatus.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = _allBookings.AsEnumerable();

            // Поиск
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();

                if (rbSearchClient.IsChecked == true)
                {
                    filtered = filtered.Where(b => b.ClientName.ToLower().Contains(search));
                }
                else if (rbSearchId.IsChecked == true)
                {
                    if (int.TryParse(search, out int searchId))
                    {
                        filtered = filtered.Where(b => b.Id == searchId);
                    }
                }
            }

            // Фильтр по статусу
            if (cmbStatus.SelectedIndex > 0 && cmbStatus.SelectedItem != null)
            {
                string selectedStatus = cmbStatus.SelectedItem.ToString();
                filtered = filtered.Where(b => b.Status == selectedStatus);
            }

            // Сортировка
            if (cmbSort.SelectedIndex == 0) // По дате (новые)
                filtered = filtered.OrderByDescending(b => b.BookingDate);
            else if (cmbSort.SelectedIndex == 1) // По дате (старые)
                filtered = filtered.OrderBy(b => b.BookingDate);
            else if (cmbSort.SelectedIndex == 2) // По сумме (↑)
                filtered = filtered.OrderBy(b => b.TotalPrice);
            else if (cmbSort.SelectedIndex == 3) // По сумме (↓)
                filtered = filtered.OrderByDescending(b => b.TotalPrice);

            dgBookings.ItemsSource = filtered.ToList();
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var bookings = dgBookings.ItemsSource as List<Booking> ?? new List<Booking>();
            tbTotalBookings.Text = bookings.Count.ToString();
            tbNewBookings.Text = bookings.Count(b => b.Status == "Новая").ToString();
            tbConfirmedBookings.Text = bookings.Count(b => b.Status == "Подтверждена").ToString();
            tbCancelledBookings.Text = bookings.Count(b => b.Status == "Отменена").ToString();
        }

        private Booking GetSelectedBooking()
        {
            return dgBookings.SelectedItem as Booking;
        }

        private void BtnNewBooking_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewBookingWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadBookings();
                MessageBox.Show("Новая заявка создана успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            var booking = GetSelectedBooking();
            if (booking == null)
            {
                MessageBox.Show("Выберите заявку для подтверждения", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (booking.Status == "Подтверждена")
            {
                MessageBox.Show("Эта заявка уже подтверждена", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Подтвердить заявку №{booking.Id} от {booking.ClientName}?",
                "Подтверждение заявки", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (DatabaseHelper.UpdateBookingStatus(booking.Id, "Подтверждена"))
                {
                    MessageBox.Show("Заявка подтверждена успешно!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookings();
                }
            }
        }

        private void BtnCancelBooking_Click(object sender, RoutedEventArgs e)
        {
            var booking = GetSelectedBooking();
            if (booking == null)
            {
                MessageBox.Show("Выберите заявку для отмены", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (booking.Status == "Отменена")
            {
                MessageBox.Show("Эта заявка уже отменена", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Отменить заявку №{booking.Id} от {booking.ClientName}?",
                "Отмена заявки", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (DatabaseHelper.UpdateBookingStatus(booking.Id, "Отменена"))
                {
                    MessageBox.Show("Заявка отменена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookings();
                }
            }
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var booking = GetSelectedBooking();
            if (booking == null)
            {
                MessageBox.Show("Выберите заявку для просмотра деталей", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var detailsWindow = new BookingDetailsWindow(booking);
            detailsWindow.ShowDialog();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBookings();
            MessageBox.Show("Список заявок обновлён", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
    }
}
