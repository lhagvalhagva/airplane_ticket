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
using System.Net.Http;
using System.ComponentModel;

namespace AirplaneTicket.WPF.Pages
{
    public partial class RegistrationCheckInPage : Page
    {
        // Constants
        private const int Rows = 10;
        private const int Columns = 6;
        
        // UI objects
        private Button? _selectedSeatButton = null;
        private DispatcherTimer toastTimer;
        
        // Services
        private readonly AirplaneService _airplaneService;
        private readonly WebSocketClient _webSocketClient;
        
        // Data objects
        private List<Passenger> passengers = new List<Passenger>();
        private HashSet<string> availableSeats = new HashSet<string>();
        private List<Flight> flights = new List<Flight>();
        private List<Seat> seatList = new List<Seat>();
        
        // Current selections
        private Passenger? currentPassenger = null;
        private int currentFlightId = 1;
        private Seat? _selectedSeat;

        /// <summary>
        /// Seat selection states for UI display
        /// </summary>
        private enum SeatSelectionState
        {
            Available,
            Occupied,
            Selected
        }

        public RegistrationCheckInPage()
        {
            InitializeComponent();
            _airplaneService = new AirplaneService();
            
            // Initialize WebSocket client
            _webSocketClient = new WebSocketClient("localhost", 9009);
            _webSocketClient.SeatAssigned += WebSocketClient_SeatAssigned;
            
            // Setup UI event handlers
            SetupEventHandlers();
            
            // Initialize toast notification timer
            toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            toastTimer.Tick += (s, e) => { ToastNotification.Visibility = Visibility.Collapsed; toastTimer.Stop(); };

            // Initialize flight status options
            FlightStatusComboBox.ItemsSource = Enum.GetValues(typeof(FlightStatus));
        }

        private void SetupEventHandlers()
        {
            Loaded += RegistrationCheckInPage_Loaded;
            SearchButton.Click += SearchButton_Click;
            FlightStatusComboBox.SelectionChanged += FlightStatusComboBox_SelectionChanged;
            FlightComboBox.SelectionChanged += FlightComboBox_SelectionChanged;
            SubmitButton.Click += SubmitButton_Click;
            PassengerList.SelectionChanged += PassengerList_SelectionChanged;
        }

        #region UI Utilities

        /// <summary>
        /// Shows a toast notification with the specified message and type
        /// </summary>
        private void ShowToast(string message, string type = "info")
        {
            ToastText.Text = message;
            ToastNotification.Background = type == "error" 
                ? new SolidColorBrush(Color.FromRgb(220, 53, 69)) 
                : new SolidColorBrush(Color.FromRgb(50, 50, 50));
            ToastNotification.Visibility = Visibility.Visible;
            toastTimer.Stop();
            toastTimer.Start();
        }

        /// <summary>
        /// Shows or hides the loading overlay
        /// </summary>
        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Updates the UI to display flight information
        /// </summary>
        private void ShowFlightInfo(Flight flight)
        {
            if (flight == null) return;
            
            FlightInfoNumber.Text = $"Нислэг: {flight.FlightNumber}";
            FlightInfoRoute.Text = $"{flight.DepartureCity} → {flight.ArrivalCity}";
            FlightInfoTime.Text = $"Явах: {flight.DepartureTime:yyyy-MM-dd HH:mm}  Ирэх: {flight.ArrivalTime:yyyy-MM-dd HH:mm}";
            FlightInfoStatus.Text = $"Төлөв: {flight.Status}";
        }

        /// <summary>
        /// Displays passenger information in the UI
        /// </summary>
        private void DisplayPassengerInfo(Passenger passenger)
        {
            if (passenger == null) return;
            
            PassengerNameText.Text = $"{passenger.FirstName} {passenger.LastName}";
            PassengerPassportText.Text = $"Паспортын дугаар: {passenger.PassportNumber}";
            PassengerContactText.Text = $"Утас: {passenger.PhoneNumber}";
        }

        /// <summary>
        /// Clears passenger information from the UI
        /// </summary>
        private void ClearPassengerInfo()
        {
            PassengerNameText.Text = string.Empty;
            PassengerPassportText.Text = string.Empty;
            PassengerContactText.Text = string.Empty;
        }

        /// <summary>
        /// Updates the style of a seat button based on its selection state
        /// </summary>
        private void UpdateSeatButtonStyle(Button button, SeatSelectionState state)
        {
            if (button == null) return;
            
            button.Background = state switch
            {
                SeatSelectionState.Available => Brushes.Green,
                SeatSelectionState.Occupied => Brushes.Red,
                SeatSelectionState.Selected => Brushes.Yellow,
                _ => Brushes.Green
            };
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// Loads and refreshes all necessary data when the page is loaded
        /// </summary>
        private async void RegistrationCheckInPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoading(true);
                
                // Connect to WebSocket server
                await ConnectToWebSocketAsync();
                
                // Load flight data
                await LoadFlightsAsync();
                
                if (flights != null && flights.Count > 0)
                {
                    // Flight selection will trigger passenger loading in the SelectionChanged event
                    FlightComboBox.SelectedIndex = 0;
                }
                else
                {
                    ShowToast("Нислэгийн мэдээлэл олдсонгүй", "error");
                }
            }
            catch (HttpRequestException ex)
            {
                ShowToast($"Сервертэй холбогдоход алдаа гарлаа: {ex.Message}", "error");
            }
            catch (Exception ex)
            {
                ShowToast($"Error initializing page: {ex.Message}", "error");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        /// <summary>
        /// Connects to the WebSocket server for real-time updates
        /// </summary>
        private async Task ConnectToWebSocketAsync()
        {
            try
            {
                await _webSocketClient.ConnectAsync();
            }
            catch (Exception wsEx)
            {
                // WebSocket connection failure is non-fatal
                Console.WriteLine($"WebSocket серверт холбогдоход алдаа гарлаа: {wsEx.Message}");
            }
        }

        /// <summary>
        /// Loads flight data from the server
        /// </summary>
        private async Task LoadFlightsAsync()
        {
            try
            {
                flights = await _airplaneService.GetFlightsAsync();
                if (flights == null)
                {
                    ShowToast("Сервертэй холбогдоход алдаа гарлаа. Сервер ажиллаж байгаа эсэхийг шалгана уу.", "error");
                    return;
                }
                
                FlightComboBox.ItemsSource = flights;
                FlightComboBox.DisplayMemberPath = "FlightNumber";
                FlightComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                ShowToast($"Error loading flights: {ex.Message}", "error");
                throw;
            }
        }

        /// <summary>
        /// Loads passenger data for the selected flight
        /// </summary>
        private async Task LoadPassengersAsync()
        {
            try
            {
                passengers = await _airplaneService.GetPassengersByFlightAsync(currentFlightId);
                if (passengers == null)
                {
                    ShowToast("Failed to load passengers. Please check if the server is running.", "error");
                    return;
                }
                UpdatePassengerList();
            }
            catch (HttpRequestException ex)
            {
                ShowToast($"Сервертэй холбогдоход алдаа гарлаа: {ex.Message}", "error");
            }
            catch (Exception ex)
            {
                ShowToast($"Error loading passengers: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Updates the passenger list UI with the current passenger data
        /// </summary>
        private void UpdatePassengerList()
        {
            try
            {
                var registeredPassengers = passengers
                    .Select(p => new
                    {
                        FullName = $"{p.FirstName} {p.LastName}",
                        p.PassportNumber,
                        SeatNumber = GetSeatNumberForPassenger(p) ?? "Not assigned"
                    })
                    .OrderByDescending(p => p.SeatNumber)
                    .ToList();
                PassengerList.ItemsSource = registeredPassengers;
            }
            catch (Exception ex)
            {
                ShowToast($"Зорчигчийн жагсаалт шинэчлэхэд алдаа гарлаа: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Gets the seat number for a passenger if one is assigned
        /// </summary>
        private string? GetSeatNumberForPassenger(Passenger passenger)
        {
            try
            {
                var seat = seatList.FirstOrDefault(s => s.IsOccupied && 
                    s.PassengerId == passenger.Id);
                    
                return seat?.SeatNumber;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Loads seat data for the selected flight
        /// </summary>
        private async Task LoadAvailableSeatsAsync()
        {
            try
            {
                seatList = (await _airplaneService.GetFlightSeatsAsync(currentFlightId)).ToList();
                if (seatList == null)
                {
                    ShowToast("Суудлын мэдээлэл ачаалахад алдаа гарлаа. Сервер ажиллаж байгаа эсэхийг шалгана уу.", "error");
                    return;
                }
                
                // Update available seats cache
                availableSeats.Clear();
                foreach (var seat in seatList.Where(s => !s.IsOccupied))
                {
                    availableSeats.Add(seat.SeatNumber);
                }
            }
            catch (HttpRequestException ex)
            {
                ShowToast($"Суудлын мэдээлэл ачаалахад алдаа гарлаа: {ex.Message}", "error");
            }
            catch (Exception ex)
            {
                ShowToast($"Суудлын мэдээлэл ачаалахад алдаа гарлаа: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Refreshes flight and passenger data after changes
        /// </summary>
        private async Task RefreshDataAsync()
        {
            await LoadAvailableSeatsAsync();
            await LoadPassengersAsync();
            GenerateSeatMap();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles selection of a flight from the dropdown
        /// </summary>
        private async void FlightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(FlightComboBox.SelectedItem is Flight selectedFlight))
                return;
                
            try
            {
                currentFlightId = selectedFlight.Id;
                ShowFlightInfo(selectedFlight);
                ShowLoading(true);
                
                // Get updated flight info (status may have changed)
                Flight updatedFlight = await _airplaneService.GetFlightAsync(selectedFlight.Id);
                if (updatedFlight != null)
                {
                    // Update flight status in UI
                    FlightStatusComboBox.SelectedItem = updatedFlight.Status;
                    
                    // Update flight info in cached list
                    UpdateFlightInCache(updatedFlight);
                }
                
                // Load data in parallel
                await RefreshDataAsync();
                
                ShowToast($"Нислэг сонгогдлоо: {selectedFlight.FlightNumber}, Төлөв: {updatedFlight.Status}");
            }
            catch (Exception ex)
            {
                ShowToast($"Нислэг сонгоход алдаа гарлаа: {ex.Message}", "error");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        /// <summary>
        /// Updates a flight in the cached flight list
        /// </summary>
        private void UpdateFlightInCache(Flight updatedFlight)
        {
            int index = flights.FindIndex(f => f.Id == updatedFlight.Id);
            if (index >= 0)
            {
                flights[index] = updatedFlight;
            }
        }

        /// <summary>
        /// Handles flight status changes from the dropdown
        /// </summary>
        private async void FlightStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || !IsLoaded || !(FlightStatusComboBox.SelectedItem is FlightStatus status))
                return;
                
            var selectedFlight = flights.FirstOrDefault(f => f.Id == currentFlightId);
            if (selectedFlight == null)
                return;
            
            try
            {
                ShowLoading(true);
                
                var previousStatus = selectedFlight.Status;
                
                if (previousStatus == status)
                {
                    ShowToast($"Нислэгийн төлөв өөрчлөгдөөгүй: {status}");
                    return;
                }
                
                var updatedStatus = await _airplaneService.UpdateFlightStatusAsync(selectedFlight.Id, status);
                
                selectedFlight.Status = updatedStatus;
                
                // If status equals the requested status, update was successful
                if (updatedStatus == status)
                {
                    ShowToast($"Нислэгийн төлөв шинэчлэгдлээ: {previousStatus} -> {updatedStatus}");
                    
                    // Get updated flight data
                    Flight updatedFlight = await _airplaneService.GetFlightAsync(selectedFlight.Id);
                    if (updatedFlight != null)
                    {
                        UpdateFlightInCache(updatedFlight);
                    }
                }
                else
                {
                    FlightStatusComboBox.SelectedItem = updatedStatus;
                    ShowToast($"Нислэгийн төлөв {updatedStatus} болж өөрчлөгдлөө", "info");
                }
            }
            catch (Exception ex)
            {
                FlightStatusComboBox.SelectedItem = selectedFlight.Status;
                ShowToast($"Нислэгийн төлөв шинэчлэхэд алдаа гарлаа: {ex.Message}", "error");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        /// <summary>
        /// Handles passenger search from the search box
        /// </summary>
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var passport = PassportNumberTextBox.Text.Trim();
            if (string.IsNullOrEmpty(passport) || passport == "Паспортын дугаар")
            {
                ShowToast("Паспортын дугаар оруулна уу", "error");
                return;
            }

            try
            {
                ShowLoading(true);
                if (passengers.Count == 0)
                {
                    await LoadPassengersAsync();
                }
                
                var passenger = passengers?.FirstOrDefault(p => p.PassportNumber == passport);
                if (passenger != null)
                {
                    currentPassenger = passenger;
                    DisplayPassengerInfo(passenger);
                    ShowToast("Зорчигч олдлоо. Суудал сонгоно уу.");
                }
                else
                {
                    currentPassenger = null;
                    ClearPassengerInfo();
                    ShowToast("Зорчигч олдсонгүй.", "error");
                }
            }
            catch (Exception ex)
            {
                ShowToast($"Зорчигч хайхад алдаа гарлаа: {ex.Message}", "error");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        /// <summary>
        /// Handles seat assignment submission
        /// </summary>
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPassenger == null || _selectedSeat == null)
            {
                ShowToast("Зорчигч болон суудал сонгоно уу.", "error");
                return;
            }

            try
            {
                ShowLoading(true);
                await AssignSeatToPassengerAsync(currentFlightId, currentPassenger, _selectedSeat);
            }
            catch (Exception ex)
            {
                ShowToast($"Суудал онооход алдаа гарлаа: {ex.Message}", "error");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        /// <summary>
        /// Updates data when a seat is assigned via WebSocket notification
        /// </summary>
        private async void WebSocketClient_SeatAssigned(object sender, SeatAssignedEventArgs e)
        {
            try
            {
                // Only update if notification is for the current flight
                if (e.FlightId == currentFlightId)
                {
                    Console.WriteLine($"Одоогийн нислэгт суудал оноолтын мэдэгдэл хүлээн авлаа: {e.SeatNumber}");
                    
                    // Show notification to user
                    ShowToast($"{e.SeatNumber} суудал оноогдлоо", "info");
                    
                    // Refresh data
                    await RefreshDataAsync();
                }
                else
                {
                    Console.WriteLine($"Өөр нислэгт суудал оноолтын мэдэгдэл хүлээн авлаа: Нислэг {e.FlightId}, Суудал {e.SeatNumber}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Суудал оноолтын мэдэгдэл боловсруулахад алдаа гарлаа: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles selection from the passenger list
        /// </summary>
        private void PassengerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PassengerList.SelectedItem == null)
                return;
                
            try
            {
                var selectedPassenger = (dynamic)PassengerList.SelectedItem;
                var passenger = passengers.FirstOrDefault(p => p.PassportNumber == selectedPassenger.PassportNumber);
                if (passenger != null)
                {
                    currentPassenger = passenger;
                    DisplayPassengerInfo(passenger);
                    
                    // Show guidance to user
                    ShowToast("Зорчигч сонгогдлоо. Суудал сонгоно уу.", "info");
                    
                    // If passenger already has a seat, highlight it
                    string seatNumber = selectedPassenger.SeatNumber;
                    if (seatNumber != "Not assigned" && seatNumber != null)
                    {
                        HighlightPassengerSeat(seatNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowToast($"Зорчигч сонгоход алдаа гарлаа: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Highlights a passenger's already assigned seat in the seat map
        /// </summary>
        private void HighlightPassengerSeat(string seatNumber)
        {
            try
            {
                foreach (var child in SeatMapGrid.Children)
                {
                    if (child is Button button && button.Content.ToString() == seatNumber)
                    {
                        // Get the current seat selection state
                        var seat = seatList.FirstOrDefault(s => s.SeatNumber == seatNumber);
                        if (seat != null && seat.IsOccupied)
                        {
                            // Highlight the seat temporarily
                            var originalBrush = button.Background;
                            button.Background = Brushes.Orange;
                            
                            // Reset after 1.5 seconds
                            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
                            timer.Tick += (s, e) => 
                            { 
                                button.Background = originalBrush;
                                timer.Stop();
                            };
                            timer.Start();
                            
                            // Show info about seat
                            ShowToast($"Зорчигч {seatNumber} суудалд бүртгэлтэй байна", "info");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Суудал тодруулахад алдаа гарлаа: {ex.Message}");
            }
        }

        #endregion

        #region Seat Management

        /// <summary>
        /// Generates the seat map UI based on current seat data
        /// </summary>
        private void GenerateSeatMap()
        {
            try
            {
                SeatMapGrid.Children.Clear();
                SeatMapGrid.RowDefinitions.Clear();
                SeatMapGrid.ColumnDefinitions.Clear();

                // Parse seat numbers to determine grid size
                var seatNumbers = seatList.Select(s => s.SeatNumber).ToList();
                var rowNumbers = seatNumbers.Select(sn => int.Parse(new string(sn.TakeWhile(char.IsDigit).ToArray()))).Distinct().OrderBy(n => n).ToList();
                var colLetters = seatNumbers.Select(sn => sn.FirstOrDefault(c => char.IsLetter(c))).Distinct().OrderBy(c => c).ToList();

                foreach (var _ in rowNumbers)
                    SeatMapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                foreach (var _ in colLetters)
                    SeatMapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                foreach (var seat in seatList)
                {
                    int row = rowNumbers.IndexOf(int.Parse(new string(seat.SeatNumber.TakeWhile(char.IsDigit).ToArray())));
                    int col = colLetters.IndexOf(seat.SeatNumber.FirstOrDefault(c => char.IsLetter(c)));

                    var button = new Button
                    {
                        Content = seat.SeatNumber,
                        Margin = new Thickness(5),
                        Background = seat.IsOccupied ? Brushes.Red : Brushes.Green,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold,
                        IsEnabled = !seat.IsOccupied
                    };
                    button.Click += SeatButton_Click;

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    SeatMapGrid.Children.Add(button);
                }
                
                _selectedSeatButton = null;
                _selectedSeat = null;
                SubmitButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                ShowToast($"Суудлын газрын зураг үүсгэхэд алдаа гарлаа: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Handles seat button clicks
        /// </summary>
        private void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPassenger == null)
            {
                ShowToast("Зорчигч сонгоно уу.", "error");
                return;
            }

            if (sender is Button button)
            {
                string seatNumber = button.Content.ToString();
                var seat = seatList.FirstOrDefault(s => s.SeatNumber == seatNumber);
                
                if (seat == null)
                {
                    ShowToast("Суудлын мэдээлэл олдсонгүй.", "error");
                    return;
                }

                if (seat.IsOccupied)
                {
                    HandleOccupiedSeatSelection(button, seat, seatNumber);
                }
                else
                {
                    HandleFreeSeatSelection(button, seat);
                }
            }
        }

        /// <summary>
        /// Handles selection of an occupied seat
        /// </summary>
        private void HandleOccupiedSeatSelection(Button button, Seat seat, string seatNumber)
        {
            var seatOwner = passengers.FirstOrDefault(p => seat.PassengerId == p.Id);
            if (seatOwner == null)
            {
                ShowToast("Энэ суудал захиалагдсан байна.", "error");
                return;
            }
            
            _selectedSeat = seat;
            
            ResetPreviousSeatSelection();
            
            _selectedSeatButton = button;
            UpdateSeatButtonStyle(button, SeatSelectionState.Occupied);
            
            ShowToast($"Суудал {seatNumber} ({seatOwner.FirstName} {seatOwner.LastName})-д захиалагдсан. Өөр суудал сонгоно уу.", "info");
        }

        /// <summary>
        /// Handles selection of an available seat
        /// </summary>
        private void HandleFreeSeatSelection(Button button, Seat seat)
        {
            ResetPreviousSeatSelection();
            
            // Select new seat
            _selectedSeatButton = button;
            _selectedSeat = seat;
            UpdateSeatButtonStyle(button, SeatSelectionState.Selected);
            SubmitButton.IsEnabled = true;
        }

        /// <summary>
        /// Resets the previously selected seat to its original state
        /// </summary>
        private void ResetPreviousSeatSelection()
        {
            if (_selectedSeatButton != null)
            {
                var prevSeat = seatList.FirstOrDefault(s => s.SeatNumber == _selectedSeatButton.Content.ToString());
                var state = (prevSeat != null && prevSeat.IsOccupied) ? SeatSelectionState.Occupied : SeatSelectionState.Available;
                UpdateSeatButtonStyle(_selectedSeatButton, state);
            }
        }

        /// <summary>
        /// Assigns a seat to a passenger via the API
        /// </summary>
        private async Task AssignSeatToPassengerAsync(int flightId, Passenger passenger, Seat seat)
        {
            var success = await _airplaneService.AssignSeatAsync(flightId, passenger.Id, seat.Id);
            
            if (!success)
            {
                ShowToast("Суудал оноох үед алдаа гарлаа (API).", "error");
                return;
            }

            ShowToast($"Суудал {seat.SeatNumber} {passenger.FirstName} {passenger.LastName} дээр оноогдлоо");
            
            await PrintTicketAsync(flightId, passenger, seat);
            
            await RefreshDataAsync();

            _selectedSeatButton = null;
            _selectedSeat = null;
            SubmitButton.IsEnabled = false;
        }

        /// <summary>
        /// Тасалбар хэвлэх
        /// </summary>
        private async Task PrintTicketAsync(int flightId, Passenger passenger, Seat seat)
        {
            try
            {
                // Нислэгийн мэдээлэл авах
                var flight = flights.FirstOrDefault(f => f.Id == flightId);
                if (flight == null)
                {
                    ShowToast("Нислэгийн мэдээлэл олдсонгүй. Тасалбар хэвлэгдсэнгүй.", "error");
                    return;
                }

                // Тасалбарын мэдээлэл бэлтгэх
                var ticketInfo = new TicketInfo
                {
                    FlightNumber = flight.FlightNumber,
                    DepartureCity = flight.DepartureCity,
                    ArrivalCity = flight.ArrivalCity,
                    DepartureTime = flight.DepartureTime,
                    ArrivalTime = flight.ArrivalTime,
                    PassengerName = $"{passenger.FirstName} {passenger.LastName}",
                    PassportNumber = passenger.PassportNumber,
                    PhoneNumber = passenger.PhoneNumber,
                    SeatNumber = seat.SeatNumber,
                    RegistrationDate = DateTime.Now
                };

                // Хэвлэх сервис ашиглан тасалбар хэвлэх
                var printService = new TicketPrintService();
                printService.PrintTicket(ticketInfo);
                
                ShowToast("Тасалбар хэвлэгдлээ!", "info");
            }
            catch (Exception ex)
            {
                ShowToast($"Тасалбар хэвлэхэд алдаа гарлаа: {ex.Message}", "error");
                // Хэвлэх алдаа суудал оноолтод нөлөөлөхгүй
            }
        }

        #endregion
    }
}