using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using AirplaneTicket.WPF.Models;
using AirplaneTicket.WPF.Services;

namespace AirplaneTicket.WPF.Pages
{
    public partial class CheckInPage : Page
    {
        private const int Rows = 10;
        private const int Columns = 6;
        private Button[,] seatButtons = new Button[Rows, Columns];
        private List<Passenger> passengers = new List<Passenger>();
        private HashSet<string> availableSeats = new HashSet<string>();
        private Passenger? currentPassenger = null;
        private readonly ApiService _apiService = new ApiService();
        private int currentFlightId = 1;
        private List<Flight> flights = new List<Flight>();
        private DispatcherTimer toastTimer;

        public CheckInPage()
        {
            InitializeComponent();
            Loaded += CheckInPage_Loaded;
            SearchButton.Click += SearchButton_Click;
            PrintBoardingPassButton.Click += PrintBoardingPassButton_Click;
            FlightStatusComboBox.SelectionChanged += FlightStatusComboBox_SelectionChanged;
            FlightComboBox.SelectionChanged += FlightComboBox_SelectionChanged;
            toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            toastTimer.Tick += (s, e) => { ToastNotification.Visibility = Visibility.Collapsed; toastTimer.Stop(); };
        }

        private void ShowToast(string message, string type = "info")
        {
            ToastText.Text = message;
            ToastNotification.Background = type == "error" ? new SolidColorBrush(Color.FromRgb(220, 53, 69)) : new SolidColorBrush(Color.FromRgb(50, 50, 50));
            ToastNotification.Visibility = Visibility.Visible;
            toastTimer.Stop();
            toastTimer.Start();
        }

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void CheckInPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);
            await LoadFlightsAsync();
            await LoadPassengersAsync();
            if (flights.Count > 0)
            {
                FlightComboBox.SelectedIndex = 0;
            }
            ShowLoading(false);
            FlightStatusComboBox.SelectedIndex = 0;
            ShowToast("Ready.");
        }

        private async Task LoadFlightsAsync()
        {
            try
            {
                flights = await _apiService.GetFlightsAsync();
                FlightComboBox.ItemsSource = flights;
                FlightComboBox.DisplayMemberPath = "FlightNumber";
                FlightComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                ShowToast($"Error loading flights: {ex.Message}", "error");
            }
        }

        private async void FlightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FlightComboBox.SelectedItem is Flight selectedFlight)
            {
                currentFlightId = selectedFlight.Id;
                ShowLoading(true);
                await LoadAvailableSeatsAsync();
                GenerateSeatMap();
                ShowLoading(false);
                ShowToast($"Flight changed to: {selectedFlight.FlightNumber}");
            }
        }

        private async Task LoadPassengersAsync()
        {
            try
            {
                passengers = await _apiService.GetPassengersAsync();
            }
            catch (Exception ex)
            {
                ShowToast($"Error loading passengers: {ex.Message}", "error");
            }
        }

        private async Task LoadAvailableSeatsAsync()
        {
            try
            {
                var seats = await _apiService.GetAvailableSeatsAsync(currentFlightId);
                availableSeats = new HashSet<string>(seats);
            }
            catch (Exception ex)
            {
                ShowToast($"Error loading seats: {ex.Message}", "error");
            }
        }

        private void GenerateSeatMap()
        {
            SeatMapGrid.Children.Clear();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    string seatNumber = $"{(char)('A' + col)}{row + 1}";
                    var btn = new Button
                    {
                        Margin = new Thickness(4),
                        MinWidth = 36,
                        MinHeight = 36,
                        Tag = seatNumber,
                        Background = Brushes.LightGray,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        Content = seatNumber,
                        FontWeight = FontWeights.Bold
                    };
                    btn.Click += SeatButton_Click;
                    if (!availableSeats.Contains(seatNumber))
                    {
                        btn.Background = Brushes.IndianRed;
                        btn.Foreground = Brushes.White;
                        btn.IsEnabled = false;
                    }
                    else
                    {
                        btn.Background = Brushes.LightGreen;
                        btn.Foreground = Brushes.Black;
                    }
                    seatButtons[row, col] = btn;
                    SeatMapGrid.Children.Add(btn);
                }
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var passport = PassportTextBox.Text.Trim();
            if (string.IsNullOrEmpty(passport))
            {
                ShowToast("Please enter a passport number.", "error");
                return;
            }
            if (passengers.Count == 0)
            {
                ShowLoading(true);
                await LoadPassengersAsync();
                ShowLoading(false);
            }
            var passenger = passengers.FirstOrDefault(p => p.PassportNumber == passport);
            if (passenger != null)
            {
                currentPassenger = passenger;
                PassengerInfoText.Text = $"Name: {passenger.FirstName} {passenger.LastName}\nPassport: {passenger.PassportNumber}";
                ShowToast("Passenger found. Select a seat.");
            }
            else
            {
                currentPassenger = null;
                PassengerInfoText.Text = "No passenger found.";
                ShowToast("Passenger not found.", "error");
            }
        }

        private async void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPassenger == null)
            {
                ShowToast("Please search and select a passenger first.", "error");
                return;
            }
            var btn = (Button)sender;
            var seatNumber = (string)btn.Tag;
            ShowLoading(true);
            try
            {
                var success = await _apiService.ReserveSeatAsync(currentFlightId, seatNumber);
                if (success)
                {
                    availableSeats.Remove(seatNumber);
                    btn.Background = Brushes.IndianRed;
                    btn.Foreground = Brushes.White;
                    btn.IsEnabled = false;
                    ShowToast($"Seat {seatNumber} assigned to {currentPassenger.FirstName} {currentPassenger.LastName}.");
                }
                else
                {
                    ShowToast($"Seat {seatNumber} could not be reserved (already taken).", "error");
                }
            }
            catch (Exception ex)
            {
                ShowToast($"Error reserving seat: {ex.Message}", "error");
            }
            ShowLoading(false);
        }

        private void PrintBoardingPassButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPassenger == null)
            {
                ShowToast("Please select a passenger first.", "error");
                return;
            }
            ShowToast($"Boarding pass printed for {currentPassenger.FirstName} {currentPassenger.LastName}. (Simulated)");
        }

        private void FlightStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var status = (FlightStatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            ShowToast($"Flight status changed to: {status}");
        }
    }
} 