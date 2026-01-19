using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GlobusTravelManager.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fileName = value as string;
            if (string.IsNullOrEmpty(fileName))
            {
                // Заглушка если фото нет
                return new BitmapImage(new Uri("pack://application:,,,/Images/no_image.png"));
            }

            try
            {
                // Пытаемся загрузить фото из папки Images
                return new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));
            }
            catch
            {
                // Если файл не найден - заглушка
                return new BitmapImage(new Uri("pack://application:,,,/Images/no_image.png"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
