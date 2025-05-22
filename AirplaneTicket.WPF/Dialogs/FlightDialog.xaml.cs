using System;
using System.Windows;
using System.Windows.Controls;
using AirplaneTicket.WPF.Models;
using System.Text.RegularExpressions;

namespace AirplaneTicket.WPF.Dialogs
{
    public partial class FlightDialog : Window
    {
        public Flight? Result { get; private set; }
        private readonly Flight? _existingFlight;

        public FlightDialog(Flight? existingFlight = null)
        {
            InitializeComponent();
            _existingFlight = existingFlight;

            if (_existingFlight != null)
            {
                Title = "Edit Flight Details";
                PopulateExistingFlight();
            }
            else
            {
                Title = "New Flight Details";
                StatusComboBox.SelectedIndex = 0; // Default to "Checking In"
            }
        }

        private void PopulateExistingFlight()
        {
            FlightNumberTextBox.Text = _existingFlight.FlightNumber;
            DepartureCityTextBox.Text = _existingFlight.DepartureCity;
            ArrivalCityTextBox.Text = _existingFlight.ArrivalCity;
            DepartureDatePicker.SelectedDate = _existingFlight.DepartureTime.Date;
            DepartureTimeTextBox.Text = _existingFlight.DepartureTime.ToString("HH:mm");
            ArrivalDatePicker.SelectedDate = _existingFlight.ArrivalTime.Date;
            ArrivalTimeTextBox.Text = _existingFlight.ArrivalTime.ToString("HH:mm");
            TotalSeatsTextBox.Text = _existingFlight.AvailableSeats.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                var departureDateTime = CombineDateAndTime(DepartureDatePicker.SelectedDate.Value, DepartureTimeTextBox.Text);
                var arrivalDateTime = CombineDateAndTime(ArrivalDatePicker.SelectedDate.Value, ArrivalTimeTextBox.Text);

                Result = new Flight
                {
                    Id = _existingFlight?.Id ?? 0,
                    FlightNumber = FlightNumberTextBox.Text.Trim(),
                    DepartureCity = DepartureCityTextBox.Text.Trim(),
                    ArrivalCity = ArrivalCityTextBox.Text.Trim(),
                    DepartureTime = departureDateTime,
                    ArrivalTime = arrivalDateTime,
                    AvailableSeats = int.Parse(TotalSeatsTextBox.Text),
                    Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), ((ComboBoxItem)StatusComboBox.SelectedItem).Tag.ToString()),
                    Price = _existingFlight?.Price ?? 1000 // Нислэгийн үнийн анхны утга
                };

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Алдаа: {ex.Message}", "Алдаа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(FlightNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(DepartureCityTextBox.Text) ||
                string.IsNullOrWhiteSpace(ArrivalCityTextBox.Text) ||
                !DepartureDatePicker.SelectedDate.HasValue ||
                !ArrivalDatePicker.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(DepartureTimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ArrivalTimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(TotalSeatsTextBox.Text) ||
                StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Бүх талбарыг бөглөнө үү.", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!IsValidTimeFormat(DepartureTimeTextBox.Text) || !IsValidTimeFormat(ArrivalTimeTextBox.Text))
            {
                MessageBox.Show("Цагийг зөв форматаар оруулна уу (HH:mm).", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TotalSeatsTextBox.Text, out int seats) || seats <= 0)
            {
                MessageBox.Show("Суудлын тоог зөв оруулна уу.", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var departureDateTime = CombineDateAndTime(DepartureDatePicker.SelectedDate.Value, DepartureTimeTextBox.Text);
            var arrivalDateTime = CombineDateAndTime(ArrivalDatePicker.SelectedDate.Value, ArrivalTimeTextBox.Text);

            if (arrivalDateTime <= departureDateTime)
            {
                MessageBox.Show("Буух цаг нь хөөрөх цагаас хойш байх ёстой.", "Баталгаажуулалтын алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidTimeFormat(string time)
        {
            return Regex.IsMatch(time, @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
        }

        private DateTime CombineDateAndTime(DateTime date, string time)
        {
            var timeParts = time.Split(':');
            return date.AddHours(int.Parse(timeParts[0])).AddMinutes(int.Parse(timeParts[1]));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 