using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AirplaneTicket.WPF.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "онлайн" => new SolidColorBrush(Colors.Green),
                    "хүлээгдэж буй" => new SolidColorBrush(Colors.Orange),
                    "дууссан" => new SolidColorBrush(Colors.Red),
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 