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

            // Если файла нет или он пустой - возвращаем заглушку
            if (string.IsNullOrWhiteSpace(fileName))
            {
                // Можно создать простую заглушку или вернуть null
                return null;
            }

            try
            {
                // Пробуем загрузить изображение из папки Images
                var uri = new Uri($"pack://application:,,,/Images/{fileName}", UriKind.Absolute);
                return new BitmapImage(uri);
            }
            catch
            {
                // Если файл не найден - возвращаем null
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}