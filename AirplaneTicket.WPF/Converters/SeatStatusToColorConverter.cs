using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AirplaneTicket.WPF.Converters
{
    public class SeatStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Available" => new SolidColorBrush(Colors.Green),
                    "Occupied" => new SolidColorBrush(Colors.Red),
                    "Selected" => new SolidColorBrush(Colors.Blue),
                    "Reserved" => new SolidColorBrush(Colors.Orange),
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