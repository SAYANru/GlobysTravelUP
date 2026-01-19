using System.Windows;
using System.Windows.Media;
using GlobusTravelManager.Models;
using GlobusTravelManager.Converters;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace GlobusTravelManager.Views
{
    public partial class BookingDetailsWindow : Window
    {
        private readonly Booking _booking;
        private readonly StatusColorConverter _colorConverter = new StatusColorConverter();

        public BookingDetailsWindow(Booking booking)
        {
            InitializeComponent();
            _booking = booking;
            LoadBookingDetails();
        }

        private void LoadBookingDetails()
        {
            tbId.Text = _booking.Id.ToString();
            tbClient.Text = _booking.ClientName;
            tbTour.Text = _booking.TourName;
            tbBookingDate.Text = _booking.BookingDate.ToString("dd.MM.yyyy HH:mm");
            tbStatus.Text = _booking.Status;
            tbPeopleCount.Text = _booking.PeopleCount.ToString();
            tbTotalPrice.Text = $"{_booking.TotalPrice:N0} руб.";
            tbComment.Text = string.IsNullOrEmpty(_booking.Comment) ? "Нет комментария" : _booking.Comment;

            // Устанавливаем цвет статуса
            var statusColor = (SolidColorBrush)_colorConverter.Convert(_booking.Status, null, null, null);
            statusBorder.Background = statusColor;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}