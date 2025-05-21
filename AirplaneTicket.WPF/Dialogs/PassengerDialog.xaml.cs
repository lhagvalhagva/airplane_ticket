using System;
using System.Windows;
using AirplaneTicket.WPF.Models;

namespace AirplaneTicket.WPF.Dialogs
{
    public partial class PassengerDialog : Window
    {
        public Passenger? Result { get; private set; }
        private readonly string _passportNumber;

        public PassengerDialog(string passportNumber)
        {
            InitializeComponent();
            _passportNumber = passportNumber;
            
            // Set the passport number in the window title
            Title = $"New Passenger Details - Passport: {passportNumber}";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate email format
            if (!IsValidEmail(EmailTextBox.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate phone number format
            if (!IsValidPhoneNumber(PhoneTextBox.Text))
            {
                MessageBox.Show("Please enter a valid phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new Passenger
            {
                PassportNumber = _passportNumber,
                FirstName = FirstNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim(),
                PhoneNumber = PhoneTextBox.Text.Trim()
            };

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