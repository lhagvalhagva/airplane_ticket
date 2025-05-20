using System;
using System.Windows;
using AirplaneTicket.WPF.Models;

namespace AirplaneTicket.WPF.Dialogs
{
    public partial class PassengerDialog : Window
    {
        private readonly Passenger _passenger;
        private readonly bool _isEdit;

        public string DialogTitle => _isEdit ? "Edit Passenger" : "Add Passenger";

        public PassengerDialog(Passenger passenger = null)
        {
            InitializeComponent();
            _isEdit = passenger != null;
            _passenger = passenger ?? new Passenger();

            if (_isEdit)
            {
                txtFirstName.Text = _passenger.FirstName;
                txtLastName.Text = _passenger.LastName;
                txtEmail.Text = _passenger.Email;
                txtPhone.Text = _passenger.PhoneNumber;
                dpDateOfBirth.SelectedDate = _passenger.DateOfBirth;
                txtPassport.Text = _passenger.PassportNumber;
            }

            DataContext = this;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                !dpDateOfBirth.SelectedDate.HasValue ||
                string.IsNullOrWhiteSpace(txtPassport.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _passenger.FirstName = txtFirstName.Text;
            _passenger.LastName = txtLastName.Text;
            _passenger.Email = txtEmail.Text;
            _passenger.PhoneNumber = txtPhone.Text;
            _passenger.DateOfBirth = dpDateOfBirth.SelectedDate.Value;
            _passenger.PassportNumber = txtPassport.Text;

            DialogResult = true;
            Close();
        }

        public Passenger GetPassenger() => _passenger;
    }
} 