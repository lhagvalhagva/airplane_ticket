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

namespace AirplaneTicket.WPF.Pages
{
    public partial class RegistrationCheckInPage : Page
    {
        private const int Rows = 10;
        private const int Columns = 6;
        private Button[,] seatButtons = new Button[Rows, Columns];
        private List<Passenger> passengers = new List<Passenger>();
        private HashSet<string> availableSeats = new HashSet<string>();
        private Passenger? currentPassenger = null;
        private readonly AirplaneService _airplaneService;
        private int currentFlightId = 1;
        private List<Flight> flights = new List<Flight>();
        private DispatcherTimer toastTimer;
        private Flight? _selectedFlight;
        private Passenger? _selectedPassenger;
    private Passenger? _selectedSeatOwner; // Суудал эзэмшигч зорчигч
        private Seat? _selectedSeat;
        private Seat? _selectedNewSeat; // Суудал солиход шинээр сонгогдсон суудал
        private List<Seat> seatList = new List<Seat>(); // Store all seats with occupancy info
        private Button? _selectedSeatButton = null; // Track the currently selected seat button
        private Button? _selectedNewSeatButton = null; // Track the new seat button for swapping

        public RegistrationCheckInPage()
        {
            InitializeComponent();
            _airplaneService = new AirplaneService();
            Loaded += RegistrationCheckInPage_Loaded;
            SearchButton.Click += SearchButton_Click;
            FlightStatusComboBox.SelectionChanged += FlightStatusComboBox_SelectionChanged;
            FlightComboBox.SelectionChanged += FlightComboBox_SelectionChanged;
            toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            toastTimer.Tick += (s, e) => { ToastNotification.Visibility = Visibility.Collapsed; toastTimer.Stop(); };

            // Initialize flight status options
            FlightStatusComboBox.ItemsSource = Enum.GetValues(typeof(FlightStatus));
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

        private async void RegistrationCheckInPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoading(true);
                await LoadFlightsAsync();
                if (flights != null && flights.Count > 0)
                {
                    // Сонгосон нислэгийн төлөвийг тохируулах үйлдэл FlightComboBox_SelectionChanged дотор болно
                    FlightComboBox.SelectedIndex = 0;
                    await LoadPassengersAsync();
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
            catch (HttpRequestException ex)
            {
                ShowToast($"Сервертэй холбогдоход алдаа гарлаа: {ex.Message}", "error");
                throw;
            }
            catch (Exception ex)
            {
                ShowToast($"Error loading flights: {ex.Message}", "error");
                throw;
            }
        }

        private async void FlightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FlightComboBox.SelectedItem is Flight selectedFlight)
                {
                    currentFlightId = selectedFlight.Id;
                    ShowFlightInfo(selectedFlight);
                    ShowLoading(true);
                    
                    // Нислэгийн шинэчилсэн мэдээллийг авах (төлөв зэрэг өөрчлөгдсөн байж болно)
                    Flight updatedFlight = await _airplaneService.GetFlightAsync(selectedFlight.Id);
                    if (updatedFlight != null)
                    {
                        // Нислэгийн төлөвийг ComboBox-д тохируулах
                        FlightStatusComboBox.SelectedItem = updatedFlight.Status;
                        
                        // Нислэгийн мэдээллийг шинэчлэх
                        int index = flights.FindIndex(f => f.Id == updatedFlight.Id);
                        if (index >= 0)
                        {
                            flights[index] = updatedFlight;
                        }
                    }
                    
                    // Load seats and passengers in parallel
                    var seatsTask = LoadAvailableSeatsAsync();
                    var passengersTask = LoadPassengersAsync();
                    
                    await Task.WhenAll(seatsTask, passengersTask);
                    
                    GenerateSeatMap();
                    ShowToast($"Нислэг сонгогдлоо: {selectedFlight.FlightNumber}, Төлөв: {updatedFlight.Status}");
                }
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

        private void ShowFlightInfo(Flight flight)
        {
            FlightInfoNumber.Text = $"Нислэг: {flight.FlightNumber}";
            FlightInfoRoute.Text = $"{flight.DepartureCity} → {flight.ArrivalCity}";
            FlightInfoTime.Text = $"Явах: {flight.DepartureTime:yyyy-MM-dd HH:mm}  Ирэх: {flight.ArrivalTime:yyyy-MM-dd HH:mm}";
            FlightInfoStatus.Text = $"Төлөв: {flight.Status}";
        }

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

        private string? GetSeatNumberForPassenger(Passenger passenger)
        {
            try
            {
                var seat = seatList.FirstOrDefault(s => s.IsOccupied && 
                    seatList.Where(seat => seat.Id == s.Id)
                    .Any(seat => seat.PassengerId == passenger.Id));
                    
                return seat?.SeatNumber;
            }
            catch
            {
                return null;
            }
        }

        private async Task LoadAvailableSeatsAsync()
        {
            try
            {
                seatList = (await _airplaneService.GetFlightSeatsAsync(currentFlightId)).ToList(); // Get all seats with occupancy info
                if (seatList == null)
                {
                    ShowToast("Суудлын мэдээлэл ачаалахад алдаа гарлаа. Сервер ажиллаж байгаа эсэхийг шалгана уу.", "error");
                    return;
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
                    // Parse row and column
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
            }
            catch (Exception ex)
            {
                ShowToast($"Суудлын газрын зураг үүсгэхэд алдаа гарлаа: {ex.Message}", "error");
            }
        }

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
            catch (HttpRequestException ex)
            {
                ShowToast($"Зорчигч хайхад алдаа гарлаа: {ex.Message}", "error");
            }
            catch (Exception ex)
            {
                ShowToast($"Error searching for passenger: {ex.Message}", "error");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private void DisplayPassengerInfo(Passenger passenger)
        {
            PassengerNameText.Text = $"{passenger.FirstName} {passenger.LastName}";
            PassengerPassportText.Text = $"Паспортын дугаар: {passenger.PassportNumber}";
            PassengerContactText.Text = $"Утас: {passenger.PhoneNumber}";
        }

        private void ClearPassengerInfo()
        {
            PassengerNameText.Text = string.Empty;
            PassengerPassportText.Text = string.Empty;
            PassengerContactText.Text = string.Empty;
        }

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
                    // Эзэмшигдсэн суудлыг сонгох үед
                    var seatOwner = passengers.FirstOrDefault(p => seat.PassengerId == p.Id);
                    if (seatOwner != null)
                    {
                        _selectedSeatOwner = seatOwner;
                        _selectedSeat = seat;
                        
                        if (_selectedSeatButton != null)
                        {
                            // Өмнө сонгогдсон суудлын өнгийг буцаах
                            var prevSeat = seatList.FirstOrDefault(s => s.SeatNumber == _selectedSeatButton.Content.ToString());
                            _selectedSeatButton.Background = prevSeat != null && prevSeat.IsOccupied ? Brushes.Red : Brushes.Green;
                        }
                        
                        _selectedSeatButton = button;
                        button.Background = Brushes.Purple; // Сонгогдсон эзэмшигдсэн суудлыг ялгах өнгө
                        
                        ShowToast($"Суудал {seatNumber} ({seatOwner.FirstName} {seatOwner.LastName})-д захиалагдсан. Сул суудал сонгож солино уу.", "info");
                        
                        // Хэрэв өмнө чөлөөт суудал сонгогдсон бол суудал солих боломжтой
                        if (_selectedNewSeat != null && !_selectedNewSeat.IsOccupied)
                        {
                            ShowSwapButton();
                        }
                        return;
                    }
                    
                    ShowToast("Энэ суудал захиалагдсан байна.", "error");
                    return;
                }
                else
                {
                    // Чөлөөт суудлыг сонгох үед
                    if (_selectedNewSeatButton != null)
                    {
                        // Өмнө сонгогдсон шинэ суудлын өнгийг буцаах
                        _selectedNewSeatButton.Background = Brushes.Green;
                    }
                    
                    _selectedNewSeatButton = button;
                    _selectedNewSeat = seat;
                    button.Background = Brushes.Yellow;
                    
                    if (_selectedSeat != null && _selectedSeat.IsOccupied)
                    {
                        // Хэрэв эзэмшигдсэн суудал өмнө сонгогдсон бол суудал солих боломжтой
                        ShowSwapButton();
                        ShowToast($"Суудлыг {_selectedSeat.SeatNumber}-с {seat.SeatNumber} руу солих боломжтой.", "info");
                    }
                    else
                    {
                        // Энгийн суудал захиалах үйлдэл
                        _selectedSeat = seat;
                        SubmitButton.IsEnabled = true;
                        
                        // Шилжүүлэх товчийг нуух
                        TransferButton.Visibility = Visibility.Collapsed;
                        TransferButtonInfo.Text = string.Empty;
                    }
                }
            }
        }

        private void ShowSwapButton()
        {
            if (_selectedSeat != null && _selectedSeat.IsOccupied && _selectedNewSeat != null && !_selectedNewSeat.IsOccupied)
            {
                // Зорчигчийн мэдээллийг авч суудал солих товчийг харуулах
                var passenger = passengers.FirstOrDefault(p => p.Id == _selectedSeat.PassengerId);
                if (passenger != null)
                {
                    TransferButton.Visibility = Visibility.Visible;
                    TransferButton.Content = "Суудал солих";
                    TransferButton.Tag = new { OwnerId = passenger.Id, SeatId = _selectedSeat.Id, NewSeatId = _selectedNewSeat.Id };
                    TransferButtonInfo.Text = $"{passenger.FirstName} {passenger.LastName}-г {_selectedSeat.SeatNumber}-с {_selectedNewSeat.SeatNumber} руу шилжүүлэх";
                }
            }
        }

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
                // Assign seat using the correct API
                var success = await _airplaneService.AssignSeatAsync(currentFlightId, currentPassenger.Id, _selectedSeat.Id);
                Console.WriteLine($"ssssdfsdfss,{currentPassenger.Id}{_selectedSeat.Id}");
                if (!success)
                {
                    ShowToast("Суудал оноох үед алдаа гарлаа (API).(EX.)", "error");
                    return;
                }

                ShowToast($"Суудал {_selectedSeat.SeatNumber} {currentPassenger.FirstName} {currentPassenger.LastName} дээр оноогдлоо");
                await LoadAvailableSeatsAsync();
                UpdatePassengerList();

                // Reset selection
                _selectedSeatButton = null;
                _selectedSeat = null;
                SubmitButton.IsEnabled = false;
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

        private void PassengerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PassengerList.SelectedItem != null)
            {
                var selectedPassenger = (dynamic)PassengerList.SelectedItem;
                var passenger = passengers.FirstOrDefault(p => p.PassportNumber == selectedPassenger.PassportNumber);
                if (passenger != null)
                {
                    // Шинэ зорчигч сонгох
                    currentPassenger = passenger;
                    DisplayPassengerInfo(passenger);

                    // Хэрэв зорчигч суудалтай бол (_selectedSeatOwner) болон шинэ зорчигч сонгогдсон бол
                    // суудлыг шилжүүлэх боломжтой болно
                    string seatNumber = selectedPassenger.SeatNumber;
                    if (seatNumber != "Not assigned" && _selectedSeatOwner != null &&
                        _selectedSeatOwner.Id != passenger.Id)
                    {
                        TransferButton.Visibility = Visibility.Visible;
                        TransferButton.Tag = new { OwnerId = _selectedSeatOwner.Id, NewPassengerId = passenger.Id };
                        TransferButtonInfo.Text = $"{_selectedSeatOwner.FirstName} {_selectedSeatOwner.LastName} -с {passenger.FirstName} {passenger.LastName} руу шилжүүлэх";
                    }
                    else
                    {
                        TransferButton.Visibility = Visibility.Collapsed;
                        TransferButtonInfo.Text = string.Empty;
                    }
                }
            }
        }

        private async void TransferButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        // Шилжүүлэх зорчигчдын мэдээллийг TransferButton.Tag-с авах
        var tagData = (dynamic)TransferButton.Tag;
        int ownerId = tagData.OwnerId;
        int newPassengerId = tagData.NewPassengerId;
        int seatId = 0;
        
        // SeatId олж авах 
        if (tagData.GetType().GetProperty("SeatId") != null)
        {
            seatId = tagData.SeatId;
        }
        else
        {
            var ownerSeats = await _airplaneService.GetPassengerSeatsAsync(currentFlightId, ownerId);
            var seat = ownerSeats.FirstOrDefault();
            if (seat == null)
            {
                ShowToast("Суудлын мэдээлэл олдсонгүй", "error");
                return;
            }
            seatId = seat.Id;
        }

        ShowLoading(true);
        
        // Суудал шилжүүлэх API дуудах
        var success = await _airplaneService.ReleaseSeatAsync(currentFlightId, seatId, newPassengerId);
        
        if (success)
        {
            ShowToast("Суудал амжилттай шилжүүллээ");
            // Суудал, зорчигчдын мэдээллийг шинэчлэх
            await LoadAvailableSeatsAsync();
            await LoadPassengersAsync();
            GenerateSeatMap();
            
            // Цэвэрлэх
            TransferButton.Visibility = Visibility.Collapsed;
            TransferButtonInfo.Text = string.Empty;
            _selectedSeatOwner = null;
        }
        else
        {
            ShowToast("Суудал шилжүүлэх үед алдаа гарлаа", "error");
        }
    }
    catch (Exception ex)
    {
        ShowToast($"Суудал шилжүүлэх үед алдаа гарлаа: {ex.Message}", "error");
    }
    finally
    {
        ShowLoading(false);
    }
}

private async void FlightStatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Хэрэв програм руу орж ирэх үед авааматаар дуудагдаж байгаа бол алгасах
            if (e.AddedItems.Count == 0 || !IsLoaded)
                return;
                
            if (FlightStatusComboBox.SelectedItem is FlightStatus status)
            {
                var selectedFlight = flights.FirstOrDefault(f => f.Id == currentFlightId);
                if (selectedFlight != null)
                {
                    try
                    {
                        ShowLoading(true);
                        
                        // Өмнөх төлөвийг хадгалах
                        var previousStatus = selectedFlight.Status;
                        
                        // Төлөв өөрчлөгдсөн эсэхийг шалгах
                        if (previousStatus == status)
                        {
                            ShowToast($"Нислэгийн төлөв өөрчлөгдөөгүй: {status}");
                            return;
                        }
                        
                        // Өгөгдлийн сан дээр төлөв шинэчлэх
                        var updatedStatus = await _airplaneService.UpdateFlightStatusAsync(selectedFlight.Id, status);
                        
                        // Өгөгдлийн сангаас буцаж ирсэн төлөвийг хадгалах
                        selectedFlight.Status = updatedStatus;
                        
                        // Хэрэв төлөв тэнцүү бол амжилттай шинэчлэгдсэн
                        if (updatedStatus == status)
                        {
                            ShowToast($"Нислэгийн төлөв шинэчлэгдлээ: {previousStatus} -> {updatedStatus}");
                            
                            // Нислэгийн дэлгэрэнгүй мэдээллийг дахин авах
                            Flight updatedFlight = await _airplaneService.GetFlightAsync(selectedFlight.Id);
                            if (updatedFlight != null)
                            {
                                // Нислэгийн мэдээллийг шинэчлэх
                                int index = flights.FindIndex(f => f.Id == updatedFlight.Id);
                                if (index >= 0)
                                {
                                    flights[index] = updatedFlight;
                                }
                            }
                        }
                        else
                        {
                            // Хэрэв төлөв өөр байвал ComboBox-г шинээр сонгогдсон төлөвөөр тохируулах
                            FlightStatusComboBox.SelectedItem = updatedStatus;
                            ShowToast($"Нислэгийн төлөв {updatedStatus} болж өөрчлөгдлөө", "info");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Алдаа гарсан тохиолдолд хуучин төлөвт буцаах
                        FlightStatusComboBox.SelectedItem = selectedFlight.Status;
                        ShowToast($"Нислэгийн төлөв шинэчлэхэд алдаа гарлаа: {ex.Message}", "error");
                    }
                    finally
                    {
                        ShowLoading(false);
                    }
                }
            }
        }
    }
}