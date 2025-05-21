using System.Windows;
using System.Windows.Controls;
using AirplaneTicket.WPF.Pages;

namespace AirplaneTicket.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Navigate to Check-in page by default
            MainFrame.Navigate(new RegistrationCheckInPage());
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "btnFlights":
                        MainFrame.Navigate(new FlightsPage());
                        break;
                    case "btnPassengers":
                        MainFrame.Navigate(new PassengersPage());
                        break;
                    case "btnBoardingPasses":
                        MainFrame.Navigate(new BoardingPassesPage());
                        break;
                    case "btnCheckIn":
                        MainFrame.Navigate(new RegistrationCheckInPage());
                        break;
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
} 