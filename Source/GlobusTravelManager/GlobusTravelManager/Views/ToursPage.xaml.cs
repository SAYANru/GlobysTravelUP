using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class ToursPage : Page
    {
        private List<Tour> _allTours = new List<Tour>();

        public ToursPage()
        {
            InitializeComponent();
            LoadTours();
            LoadCountries();
        }

        private void LoadTours()
        {
            try
            {
                _allTours = DatabaseHelper.GetAllTours();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCountries()
        {
            cmbCountry.Items.Clear();
            cmbCountry.Items.Add("Все страны");

            var countries = _allTours
                .Select(t => t.CountryName)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c);

            foreach (var country in countries)
            {
                cmbCountry.Items.Add(country);
            }

            cmbCountry.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filtered = _allTours.AsEnumerable();

            // Поиск по названию
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                filtered = filtered.Where(t => t.Name.ToLower().Contains(search));
            }

            // Фильтр по стране
            if (cmbCountry.SelectedIndex > 0 && cmbCountry.SelectedItem != null)
            {
                string selectedCountry = cmbCountry.SelectedItem.ToString();
                filtered = filtered.Where(t => t.CountryName == selectedCountry);
            }

            // Сортировка
            if (cmbSort.SelectedIndex == 0) // По дате (возр.)
                filtered = filtered.OrderBy(t => t.StartDate);
            else if (cmbSort.SelectedIndex == 1) // По дате (убыв.)
                filtered = filtered.OrderByDescending(t => t.StartDate);
            else if (cmbSort.SelectedIndex == 2) // По цене (возр.)
                filtered = filtered.OrderBy(t => t.Price);
            else if (cmbSort.SelectedIndex == 3) // По цене (убыв.)
                filtered = filtered.OrderByDescending(t => t.Price);
            else if (cmbSort.SelectedIndex == 4) // По названию
                filtered = filtered.OrderBy(t => t.Name);

            dgTours.ItemsSource = filtered.ToList();
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            var tours = dgTours.ItemsSource as List<Tour> ?? new List<Tour>();
            tbTotalTours.Text = tours.Count.ToString();
            tbAvailableTours.Text = tours.Count(t => t.AvailableSeats > 0).ToString();
            tbNoSeatsTours.Text = tours.Count(t => t.AvailableSeats == 0).ToString();
        }

        private void BtnAddTour_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция добавления тура", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEditTour_Click(object sender, RoutedEventArgs e)
        {
            if (dgTours.SelectedItem is Tour selectedTour)
            {
                MessageBox.Show($"Редактирование тура: {selectedTour.Name}", "Редактирование",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите тур для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteTour_Click(object sender, RoutedEventArgs e)
        {
            if (dgTours.SelectedItem is Tour selectedTour)
            {
                var result = MessageBox.Show($"Удалить тур '{selectedTour.Name}'?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show($"Тур '{selectedTour.Name}' удалён", "Удаление",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadTours();
                }
            }
            else
            {
                MessageBox.Show("Выберите тур для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTours();
            MessageBox.Show("Список обновлён", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
    }
}