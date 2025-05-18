
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OrderForm.Models;
using OrderForm.Services;
using System.Configuration;
using System.Threading.Tasks;
using OrderForm.Controls;

namespace OrderForm.Pages
{
    public class CheckInForm : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;
        
        private List<FlightDto> _flights = new List<FlightDto>();
        private List<SeatDto> _allSeats = new List<SeatDto>();
        private PassengerDto? _currentPassenger;
        private FlightDto? _selectedFlight;
        private SeatMapUserControl _seatMapControl;
        private List<PassengerDto> _flightPassengers = new List<PassengerDto>();
        
        // UI Элементүүд
        private Label lblTitle;
        private Label lblFlightSelection;
        private ComboBox cmbFlights;
        private Button btnRefresh;
        
        private GroupBox grpPassengerSearch;
        private Label lblPassportNumber;
        private TextBox txtPassportNumber;
        private Button btnSearch;
        private Label lblPassengerInfo;
        
        private GroupBox grpSeatMap;
        private Button btnCheckIn;
        
        private GroupBox grpPassengers;
        private Label lblPassengerList;
        private ListBox lstPassengers;
        private Label lblPassengerCount;
        
        private Button btnBack;
        
        public CheckInForm()
        {
            // API Service үүсгэх
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(_apiBaseUrl);
            
            InitializeComponents();
            InitializeForm();
        }
        
        private void InitializeComponents()
        {
            // Формын тохиргоо
            this.Text = "Зорчигч Суудал Бүртгэх (Check-in)";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            
            // Гарчиг
            lblTitle = new Label();
            lblTitle.Text = "ЗОРЧИГЧ БҮРТГЭХ";
            lblTitle.Font = new Font("Arial", 18, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);
            this.Controls.Add(lblTitle);
            
            // Нислэг сонгох хэсэг
            lblFlightSelection = new Label();
            lblFlightSelection.Text = "Нислэг сонгох:";
            lblFlightSelection.AutoSize = true;
            lblFlightSelection.Font = new Font("Arial", 10, FontStyle.Bold);
            lblFlightSelection.Location = new Point(20, 70);
            this.Controls.Add(lblFlightSelection);
            
            cmbFlights = new ComboBox();
            cmbFlights.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFlights.Size = new Size(400, 30);
            cmbFlights.Location = new Point(140, 70);
            cmbFlights.SelectedIndexChanged += cmbFlights_SelectedIndexChanged;
            this.Controls.Add(cmbFlights);
            
            btnRefresh = new Button();
            btnRefresh.Text = "Шинэчлэх";
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Location = new Point(550, 70);
            btnRefresh.Click += btnRefresh_Click;
            this.Controls.Add(btnRefresh);
            
            // Зорчигчийн паспорт хайх хэсэг
            grpPassengerSearch = new GroupBox();
            grpPassengerSearch.Text = "Зорчигч хайх";
            grpPassengerSearch.Size = new Size(400, 180);
            grpPassengerSearch.Location = new Point(20, 120);
            
            lblPassportNumber = new Label();
            lblPassportNumber.Text = "Паспортын дугаар:";
            lblPassportNumber.AutoSize = true;
            lblPassportNumber.Location = new Point(20, 40);
            grpPassengerSearch.Controls.Add(lblPassportNumber);
            
            txtPassportNumber = new TextBox();
            txtPassportNumber.Size = new Size(200, 25);
            txtPassportNumber.Location = new Point(150, 40);
            grpPassengerSearch.Controls.Add(txtPassportNumber);
            
            btnSearch = new Button();
            btnSearch.Text = "Хайх";
            btnSearch.Size = new Size(80, 30);
            btnSearch.Location = new Point(360, 38);
            btnSearch.Click += btnSearch_Click;
            grpPassengerSearch.Controls.Add(btnSearch);
            
            lblPassengerInfo = new Label();
            lblPassengerInfo.Text = "Зорчигчийн мэдээлэл: ";
            lblPassengerInfo.Size = new Size(350, 80);
            lblPassengerInfo.Location = new Point(20, 80);
            grpPassengerSearch.Controls.Add(lblPassengerInfo);
            
            this.Controls.Add(grpPassengerSearch);
            
            // Зорчигчид харуулах хэсэг
            grpPassengers = new GroupBox();
            grpPassengers.Text = "Нислэгийн зорчигчид";
            grpPassengers.Size = new Size(400, 400);
            grpPassengers.Location = new Point(20, 310);
            
            lblPassengerList = new Label();
            lblPassengerList.Text = "Зорчигчдын жагсаалт:";
            lblPassengerList.AutoSize = true;
            lblPassengerList.Location = new Point(20, 30);
            grpPassengers.Controls.Add(lblPassengerList);
            
            lstPassengers = new ListBox();
            lstPassengers.Size = new Size(360, 300);
            lstPassengers.Location = new Point(20, 60);
            lstPassengers.SelectedIndexChanged += lstPassengers_SelectedIndexChanged;
            grpPassengers.Controls.Add(lstPassengers);
            
            lblPassengerCount = new Label();
            lblPassengerCount.Text = "Нийт зорчигчийн тоо: 0";
            lblPassengerCount.AutoSize = true;
            lblPassengerCount.Location = new Point(20, 370);
            grpPassengers.Controls.Add(lblPassengerCount);
            
            this.Controls.Add(grpPassengers);
            
            // Суудал сонгох хэсэг
            grpSeatMap = new GroupBox();
            grpSeatMap.Text = "Суудал сонгох";
            grpSeatMap.Size = new Size(730, 590);
            grpSeatMap.Location = new Point(450, 120);
            
            // Суудлын зураг үүсгэх
            _seatMapControl = new SeatMapUserControl();
            _seatMapControl.Size = new Size(700, 500);
            _seatMapControl.Location = new Point(15, 30);
            _seatMapControl.SeatSelected += SeatMapControl_SeatSelected;
            grpSeatMap.Controls.Add(_seatMapControl);
            
            btnCheckIn = new Button();
            btnCheckIn.Text = "Бүртгэх";
            btnCheckIn.Size = new Size(150, 40);
            btnCheckIn.Font = new Font("Arial", 12, FontStyle.Bold);
            btnCheckIn.Location = new Point(290, 540);
            btnCheckIn.BackColor = Color.SteelBlue;
            btnCheckIn.ForeColor = Color.White;
            btnCheckIn.Enabled = false;
            btnCheckIn.Click += btnCheckIn_Click;
            grpSeatMap.Controls.Add(btnCheckIn);
            
            this.Controls.Add(grpSeatMap);
            
            // Буцах товч
            btnBack = new Button();
            btnBack.Text = "Буцах";
            btnBack.Size = new Size(100, 30);
            btnBack.Location = new Point(20, 720);
            btnBack.Click += btnBack_Click;
            this.Controls.Add(btnBack);
        }
        
        private async void InitializeForm()
        {
            try
            {
                // Өгөгдлийн санг анхлуулах (хэрэгтэй бол)
                await InitializeDatabaseAsync();

                // Нислэгүүдийн жагсаалтыг авах
                await LoadFlightsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task InitializeDatabaseAsync()
        {
            try
            {
                await _apiService.InitializeDatabaseAsync();
                Console.WriteLine("Database initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
            }
        }
        
        private async Task LoadFlightsAsync()
        {
            try
            {
                _flights = await _apiService.GetAllFlightsAsync();
                
                cmbFlights.Items.Clear();
                foreach (var flight in _flights)
                {
                    // Нислэгийн дэлгэрэнгүй мэдээллийг харуулах
                    string flightInfo = $"{flight.FlightNumber} ({flight.DepartureCity}-{flight.ArrivalCity}) - {flight.DepartureTime.ToShortDateString()} {flight.DepartureTime.ToShortTimeString()}";
                    cmbFlights.Items.Add(flightInfo);
                }

                if (cmbFlights.Items.Count > 0)
                {
                    cmbFlights.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгүүдийн жагсаалт авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private string GetStatusText(FlightStatus status)
        {
            switch (status)
            {
                case FlightStatus.CheckingIn:
                    return "Бүртгэж байна";
                case FlightStatus.Boarding:
                    return "Сууж байна";
                case FlightStatus.Departed:
                    return "Хөдөлсөн";
                case FlightStatus.Cancelled:
                    return "Цуцлагдсан";
                case FlightStatus.Delayed:
                    return "Хойшлогдсон";
                default:
                    return status.ToString();
            }
        }
        
        private async Task<FlightDto?> GetSelectedFlight()
        {
            if (cmbFlights.SelectedIndex < 0 || _flights.Count == 0) return null;
            return _flights[cmbFlights.SelectedIndex];
        }
        
        private async void cmbFlights_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _selectedFlight = await GetSelectedFlight();
                if (_selectedFlight != null)
                {
                    // Суудлуудыг харуулах
                    await LoadAllSeatsAsync(_selectedFlight.Id);
                    
                    // Тухайн нислэгийн зорчигчдыг харуулах
                    await LoadFlightPassengersAsync(_selectedFlight.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн мэдээллийг авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task LoadAllSeatsAsync(int flightId)
        {
            try
            {
                // Бүх суудлыг авах
                _allSeats = await _apiService.GetAllSeatsAsync(flightId);
                
                // Суудлын зурагт шинэчлэх
                _seatMapControl.UpdateSeatMap(_allSeats);
                
                // Check-in товчийг идэвхгүй болгох
                btnCheckIn.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлын мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task LoadFlightPassengersAsync(int flightId)
        {
            try
            {
                // Нислэгийн зорчигчдыг авах
                _flightPassengers = await _apiService.GetFlightPassengersAsync(flightId);
                
                // Жагсаалт цэвэрлэх
                lstPassengers.Items.Clear();
                
                // Зорчигчдыг жагсаалтанд нэмэх
                foreach (var passenger in _flightPassengers)
                {
                    string passengerInfo = $"{passenger.FirstName} {passenger.LastName} ({passenger.PassportNumber})";
                    lstPassengers.Items.Add(passengerInfo);
                }
                
                // Зорчигчдын тоог лабелд харуулах
                lblPassengerCount.Text = $"Нийт зорчигчийн тоо: {_flightPassengers.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн зорчигчдын мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblPassengerCount.Text = "Нийт зорчигчийн тоо: 0";
            }
        }
        
        private void lstPassengers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPassengers.SelectedIndex >= 0 && lstPassengers.SelectedIndex < _flightPassengers.Count)
            {
                // Сонгогдсон зорчигчийг тохируулах
                _currentPassenger = _flightPassengers[lstPassengers.SelectedIndex];
                
                // Хэрэглэгчийн интерфэйсийг шинэчлэх
                UpdateUIForSelectedPassenger();
            }
            else
            {
                // Зорчигч сонгогдоогүй бол 
                _currentPassenger = null;
                txtPassportNumber.Text = string.Empty;
                lblPassengerInfo.Text = "Зорчигчийн мэдээлэл: ";
                btnCheckIn.Enabled = false;
            }
        }
        
        private void UpdateUIForSelectedPassenger()
        {
            if (_currentPassenger != null)
            {
                // Паспортын дугаарыг харуулах
                txtPassportNumber.Text = _currentPassenger.PassportNumber;
                
                // Зорчигчийн мэдээллийг харуулах
                lblPassengerInfo.Text = $"Зорчигчийн мэдээлэл: {_currentPassenger.FirstName} {_currentPassenger.LastName}\n" +
                    $"Паспортын дугаар: {_currentPassenger.PassportNumber}\n" +
                    $"Имэйл: {_currentPassenger.Email}\n" +
                    $"Утасны дугаар: {_currentPassenger.PhoneNumber}";
                
                // Check-in товчийг идэвхжүүлэх
                btnCheckIn.Enabled = _seatMapControl.SelectedSeat != null;
            }
        }
        
        private void SeatMapControl_SeatSelected(object sender, SeatSelectedEventArgs e)
        {
            // Суудал сонгогдсон үед Check-in товчийг идэвхжүүлэх
            if (_currentPassenger != null && e.SelectedSeat != null)
            {
                btnCheckIn.Enabled = true;
            }
            else
            {
                btnCheckIn.Enabled = false;
            }
        }
        
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassportNumber.Text))
            {
                MessageBox.Show("Паспортын дугаар оруулна уу.", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try 
            {
                await SearchPassengerAsync(txtPassportNumber.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч хайхад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task SearchPassengerAsync(string passportNumber)
        {
            try
            {
                // Зорчигчийн мэдээлэл авах
                var passenger = await _apiService.GetPassengerAsync(passportNumber);
                _currentPassenger = passenger;
                
                if (passenger != null)
                {
                    // Зорчигчийн мэдээллийг харуулах
                    lblPassengerInfo.Text = $"Зорчигчийн мэдээлэл: {passenger.FirstName} {passenger.LastName}\n" +
                        $"Паспортын дугаар: {passenger.PassportNumber}\n" +
                        $"Имэйл: {passenger.Email}\n" +
                        $"Утасны дугаар: {passenger.PhoneNumber}";
                    
                    // Check-in товчийг идэвхжүүлэх
                    btnCheckIn.Enabled = _seatMapControl.SelectedSeat != null;
                    
                    // Зорчигчдын жагсаалтаас сонгох
                    for (int i = 0; i < _flightPassengers.Count; i++)
                    {
                        if (_flightPassengers[i].PassportNumber == passportNumber)
                        {
                            lstPassengers.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"{passportNumber} дугаартай зорчигч олдсонгүй.", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblPassengerInfo.Text = "Зорчигчийн мэдээлэл: ";
                    btnCheckIn.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигчийн мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void btnCheckIn_Click(object sender, EventArgs e)
        {
            try
            {
                await CheckInPassengerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Чекин хийхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task CheckInPassengerAsync()
        {
            try
            {
                if (_selectedFlight == null) return;

                // Сонгосон суудлын дугаар харуулах
                string seatNumber = _seatMapControl.SelectedSeat.SeatNumber;
                Console.WriteLine($"Selected seat: {seatNumber}, FlightId: {_selectedFlight.Id}, PassportNumber: {_currentPassenger.PassportNumber}");
                
                // Check-in хийх
                var boardingPass = await _apiService.CheckInPassengerAsync(_selectedFlight.Id, _currentPassenger.PassportNumber, seatNumber);

                if (boardingPass != null)
                {
                    // Нислэгийн суудлуудыг шинэчлэх
                    await LoadAllSeatsAsync(_selectedFlight.Id);

                    // Амжилттай Check-in хийгдсэн тухай мэдээлэх
                    MessageBox.Show($"Зорчигч {_currentPassenger.FirstName} {_currentPassenger.LastName}-г амжилттай бүртгэлээ.\n" +
                        $"Нислэг: {_selectedFlight.FlightNumber}\n" +
                        $"Чиглэл: {_selectedFlight.DepartureCity} - {_selectedFlight.ArrivalCity}\n" +
                        $"Суудал: {_seatMapControl.SelectedSeat.SeatNumber}", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Суудлын тасалбар хэвлэх
                    PrintBoardingPass();

                    // Дараагийн зорчигчид бэлтгэх
                    ResetFormForNextPassenger();
                }
                else
                {
                    MessageBox.Show("Check-in хийхэд алдаа гарлаа. Та сонгосон суудал мэдээллийн санд олдсонгүй байна.", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Илүү дэлгэрэнгүй мэдээлэл
                string errorDetails = $"Check-in хийхэд алдаа гарлаа: {ex.Message}\n\n" +
                    $"Сонгосон суудал: {_seatMapControl?.SelectedSeat?.SeatNumber}\n" +
                    $"Нислэгийн ID: {_selectedFlight?.Id}\n" +
                    $"Нислэгийн дугаар: {_selectedFlight?.FlightNumber}\n" +
                    $"Паспортын дугаар: {_currentPassenger?.PassportNumber}";
                
                // Консоль дээр лог харуулах
                Console.WriteLine(errorDetails);
                
                // Хэрэглэгчид алдааны мэдээллийг харуулах
                MessageBox.Show(errorDetails, "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void PrintBoardingPass()
        {
            try
            {
                if (_currentPassenger == null || _selectedFlight == null || _seatMapControl.SelectedSeat == null)
                    return;

                // Энд суудлын тасалбар хэвлэх код бичигдэнэ
                MessageBox.Show("Суудлын тасалбар хэвлэгдэж байна...", "Мэдээлэл", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлын тасалбар хэвлэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ResetFormForNextPassenger()
        {
            _currentPassenger = null;
            txtPassportNumber.Text = string.Empty;
            lblPassengerInfo.Text = "Зорчигчийн мэдээлэл: ";
            _seatMapControl.ClearSelectedSeat();
            btnCheckIn.Enabled = false;
            lstPassengers.SelectedIndex = -1;
        }
        
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFlight != null)
                {
                    await LoadAllSeatsAsync(_selectedFlight.Id);
                    await LoadFlightPassengersAsync(_selectedFlight.Id);
                }
                else
                {
                    await LoadFlightsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Шинэчлэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
