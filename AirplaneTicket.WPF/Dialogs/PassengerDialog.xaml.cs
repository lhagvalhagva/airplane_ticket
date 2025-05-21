using System;
using System.Windows;
using AirplaneTicket.WPF.Models;
using AirplaneTicket.WPF.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirplaneTicket.WPF.Dialogs
{
    public partial class PassengerDialog : Window
    {
        public Passenger? Result { get; private set; }
        public Flight? SelectedFlight { get; private set; }
        private readonly string _passportNumber;
        private readonly AirplaneService _airplaneService;

        public PassengerDialog(string passportNumber)
        {
            InitializeComponent();
            _passportNumber = passportNumber;
            _airplaneService = new AirplaneService();
            
            // Set the passport number in the window title
            Title = $"New Passenger Details - Passport: {passportNumber}";
            
            // Load available flights
            LoadFlights();
        }

        private async void LoadFlights()
        {
            try
            {
                var flights = await _airplaneService.GetFlightsAsync();
                FlightComboBox.ItemsSource = flights;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading flights: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(NationalityTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                FlightComboBox.SelectedItem == null)
            {
                MessageBox.Show("Бүх талбарыг бөглөнө үү.", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate email format
            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Зөв и-мэйл хаяг оруулна уу.", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate phone number format
            if (!IsValidPhoneNumber(PhoneTextBox.Text))
            {
                MessageBox.Show("Зөв утасны дугаар оруулна уу.", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new Passenger
            {
                PassportNumber = PassportNumberTextBox.Text.Trim(),
                FirstName = FirstNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim(),
                Nationality = NationalityTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                PhoneNumber = PhoneTextBox.Text.Trim(),
                CheckedIn = false
            };

            SelectedFlight = FlightComboBox.SelectedItem as Flight;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Basic phone number validation - can be enhanced based on your requirements
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+?[0-9\s\-\(\)]{8,}$");
        }
    }
} 