using System.Windows;
using System.Xml.Linq;
using GlobusTravelManager.Database;
using GlobusTravelManager.Models;

namespace GlobusTravelManager.Views
{
    public partial class BusEditWindow : Window
    {
        private readonly BusType _bus;
        private readonly bool _isEditMode;

        public BusEditWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            Title = "Добавление типа автобуса";
        }

        public BusEditWindow(BusType bus)
        {
            InitializeComponent();
            _bus = bus;
            _isEditMode = true;
            Title = "Редактирование типа автобуса";
            LoadBusData();
        }

        private void LoadBusData()
        {
            if (_bus != null)
            {
                txtName.Text = _bus.Name;
                txtDescription.Text = _bus.Description;
                txtCapacity.Text = _bus.Capacity.ToString();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название типа автобуса", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtCapacity.Text, out int capacity) || capacity < 1)
            {
                MessageBox.Show("Введите корректную вместимость", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();

            bool success;

            if (_isEditMode)
            {
                success = DatabaseHelper.UpdateBusType(_bus.Id, name, description, capacity);
            }
            else
            {
                success = DatabaseHelper.AddBusType(name, description, capacity);
            }

            if (success)
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