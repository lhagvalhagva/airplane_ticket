using System.Windows;
using System.Windows.Controls;

namespace AirplaneTicket.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.CheckInPage());
            CheckInNavButton.Click += (s, e) => MainFrame.Navigate(new Pages.CheckInPage());
            FlightsNavButton.Click += (s, e) => MainFrame.Navigate(new Pages.FlightsPage());
            PassengersNavButton.Click += (s, e) => MainFrame.Navigate(new Pages.PassengersPage());
            BoardingPassesNavButton.Click += (s, e) => MainFrame.Navigate(new Pages.BoardingPassesPage());
        }
    }
}