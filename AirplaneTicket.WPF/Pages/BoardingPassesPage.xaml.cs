using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AirplaneTicket.WPF.Models;
using AirplaneTicket.WPF.Services;

namespace AirplaneTicket.WPF.Pages
{
    public partial class BoardingPassesPage : Page
    {
        private readonly AirplaneService _airplaneService;
        private List<BoardingPass> _allBoardingPasses;

        public BoardingPassesPage()
        {
            InitializeComponent();
            _airplaneService = new AirplaneService();
            LoadBoardingPasses();
        }

        private async void LoadBoardingPasses()
        {
            try
            {
                _allBoardingPasses = await _airplaneService.GetBoardingPassesAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading boarding passes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_allBoardingPasses == null) return;

            var filteredBoardingPasses = _allBoardingPasses.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchTerm = txtSearch.Text.ToLower();
                filteredBoardingPasses = filteredBoardingPasses.Where(bp =>
                    bp.Flight?.FlightNumber.ToLower().Contains(searchTerm) == true ||
                    bp.Passenger?.LastName.ToLower().Contains(searchTerm) == true ||
                    bp.SeatNumber.ToLower().Contains(searchTerm) ||
                    bp.Gate.ToLower().Contains(searchTerm) ||
                    bp.Status.ToLower().Contains(searchTerm));
            }

            dgBoardingPasses.ItemsSource = filteredBoardingPasses.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBoardingPasses();
        }

        private void btnAddBoardingPass_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement create boarding pass dialog
            MessageBox.Show("Create Boarding Pass functionality will be implemented soon.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void dgBoardingPasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBoardingPasses.SelectedItem is BoardingPass selectedBoardingPass)
            {
                // TODO: Implement boarding pass selection logic
                // For example, show boarding pass details or print boarding pass
            }
        }
    }
} 