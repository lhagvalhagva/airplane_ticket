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
    public partial class FlightsPage : Page
    {
        private readonly AirplaneService _airplaneService;
        private List<Flight> _allFlights;

        public FlightsPage()
        {
            InitializeComponent();
            _airplaneService = new AirplaneService();
            _allFlights = new List<Flight>();
            LoadFlights();
        }

        private async void LoadFlights()
        {
            try
            {
                _allFlights = await _airplaneService.GetFlightsAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading flights: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allFlights == null) return;

            var filteredFlights = _allFlights.AsEnumerable();

           
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                filteredFlights = filteredFlights.Where(f =>
                    f.FlightNumber.ToLower().Contains(searchTerm) ||
                    f.DepartureCity.ToLower().Contains(searchTerm) ||
                    f.ArrivalCity.ToLower().Contains(searchTerm));
            }

           
            if (cmbFilter.SelectedIndex > 0)
            {
                switch (cmbFilter.SelectedIndex)
                {
                    case 1:
                        filteredFlights = filteredFlights.Where(f => f.AvailableSeats > 0);
                        break;
                    case 2:
                        var today = DateTime.Today;
                        filteredFlights = filteredFlights.Where(f => 
                            f.DepartureTime.Date == today || f.ArrivalTime.Date == today);
                        break;
                }
            }

            dgFlights.ItemsSource = filteredFlights.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadFlights();
        }

        private async void btnAddFlight_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FlightDialog();
            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                try
                {
                    await _airplaneService.CreateFlightAsync(dialog.Result);
                    LoadFlights();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating flight: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgFlights.SelectedItem is Flight selectedFlight)
            {
                var dialog = new FlightDialog(selectedFlight);
                if (dialog.ShowDialog() == true && dialog.Result != null)
                {
                    try
                    {
                        await _airplaneService.UpdateFlightAsync(dialog.Result);
                        LoadFlights();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating flight: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgFlights.SelectedItem is Flight selectedFlight)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete flight {selectedFlight.FlightNumber}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _airplaneService.DeleteFlightAsync(selectedFlight.Id);
                        LoadFlights();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting flight: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void dgFlights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
    }
} 