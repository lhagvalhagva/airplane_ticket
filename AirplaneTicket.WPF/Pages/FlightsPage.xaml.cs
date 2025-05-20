using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AirplaneTicket.WPF.Models;
using AirplaneTicket.WPF.Services;

namespace AirplaneTicket.WPF.Pages
{
    public partial class FlightsPage : Page
    {
        private readonly ApiService _apiService;
        private List<Flight> _allFlights;

        public FlightsPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadFlights();
        }

        private async void LoadFlights()
        {
            try
            {
                _allFlights = await _apiService.GetFlightsAsync();
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

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                filteredFlights = filteredFlights.Where(f =>
                    f.FlightNumber.ToLower().Contains(searchTerm) ||
                    f.DepartureCity.ToLower().Contains(searchTerm) ||
                    f.ArrivalCity.ToLower().Contains(searchTerm));
            }

            // Apply combo box filter
            if (cmbFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Available Only":
                        filteredFlights = filteredFlights.Where(f => f.AvailableSeats > 0);
                        break;
                    case "Today's Flights":
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

        private void dgFlights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFlights.SelectedItem is Flight selectedFlight)
            {
                // TODO: Implement flight selection logic
                // For example, navigate to a booking page or show flight details
            }
        }
    }
} 