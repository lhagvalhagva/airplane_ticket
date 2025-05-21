using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AirplaneTicket.WPF.Models;

namespace AirplaneTicket.WPF.Converters
{
    public class FlightStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FlightStatus status)
            {
                return status switch
                {
                    FlightStatus.CheckingIn => new SolidColorBrush((Color)App.Current.Resources["PrimaryColor"]),
                    FlightStatus.Boarding => new SolidColorBrush((Color)App.Current.Resources["SuccessColor"]),
                    FlightStatus.Departed => new SolidColorBrush((Color)App.Current.Resources["SecondaryColor"]),
                    FlightStatus.Delayed => new SolidColorBrush((Color)App.Current.Resources["WarningColor"]),
                    FlightStatus.Cancelled => new SolidColorBrush((Color)App.Current.Resources["ErrorColor"]),
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