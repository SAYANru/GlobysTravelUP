using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class BusesPage : Page
    {
        private List<BusType> _allBuses = new List<BusType>();

        public BusesPage()
        {
            InitializeComponent();
            LoadBuses();
        }

        private void LoadBuses()
        {
            try
            {
                _allBuses = DatabaseHelper.GetAllBusTypes();

                // Добавляем информацию об использовании
                foreach (var bus in _allBuses)
                {
                    // В реальном приложении нужно сделать запрос к БД
                    bus.UsageCount = new Random().Next(0, 5); // Заглушка
                }

                dgBuses.ItemsSource = _allBuses;
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки автобусов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            tbBusCount.Text = _allBuses.Count.ToString();
            tbTotalCapacity.Text = _allBuses.Sum(b => b.Capacity).ToString();
        }

        private BusType GetSelectedBus()
        {
            return dgBuses.SelectedItem as BusType;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new BusEditWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadBuses();
                MessageBox.Show("Тип автобуса добавлен успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var bus = GetSelectedBus();
            if (bus == null)
            {
                MessageBox.Show("Выберите тип автобуса для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new BusEditWindow(bus);
            if (dialog.ShowDialog() == true)
            {
                LoadBuses();
                MessageBox.Show("Тип автобуса обновлен успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var bus = GetSelectedBus();
            if (bus == null)
            {
                MessageBox.Show("Выберите тип автобуса для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить тип автобуса '{bus.Name}'?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (DatabaseHelper.DeleteBusType(bus.Id))
                {
                    MessageBox.Show("Тип автобуса удален успешно!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBuses();
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBuses();
            MessageBox.Show("Список обновлен", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}