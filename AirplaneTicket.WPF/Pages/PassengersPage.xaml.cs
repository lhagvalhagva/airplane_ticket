using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AirplaneTicket.WPF.Models;
using AirplaneTicket.WPF.Services;
using AirplaneTicket.WPF.Dialogs;

namespace AirplaneTicket.WPF.Pages
{
    public partial class PassengersPage : Page
    {
        private readonly ApiService _apiService;
        private List<Passenger> _allPassengers;

        public PassengersPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadPassengers();
        }

        private async void LoadPassengers()
        {
            try
            {
                _allPassengers = await _apiService.GetPassengersAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading passengers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allPassengers == null) return;

            var filteredPassengers = _allPassengers.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                filteredPassengers = filteredPassengers.Where(p =>
                    p.FirstName.ToLower().Contains(searchTerm) ||
                    p.LastName.ToLower().Contains(searchTerm) ||
                    p.Email.ToLower().Contains(searchTerm) ||
                    p.PassportNumber.ToLower().Contains(searchTerm));
            }

            dgPassengers.ItemsSource = filteredPassengers.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPassengers();
        }

        private async void btnAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PassengerDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var passenger = dialog.GetPassenger();
                    await _apiService.CreatePassengerAsync(passenger);
                    LoadPassengers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating passenger: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgPassengers.SelectedItem is Passenger selectedPassenger)
            {
                var dialog = new PassengerDialog(selectedPassenger);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        var passenger = dialog.GetPassenger();
                        await _apiService.UpdatePassengerAsync(passenger);
                        LoadPassengers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating passenger: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgPassengers.SelectedItem is Passenger selectedPassenger)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete passenger {selectedPassenger.FirstName} {selectedPassenger.LastName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeletePassengerAsync(selectedPassenger.Id);
                        LoadPassengers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting passenger: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void dgPassengers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selection is handled by the edit and delete buttons
        }
    }
} 