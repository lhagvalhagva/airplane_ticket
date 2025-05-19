using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

using OrderForm.Models;
using OrderForm.Pages;
using OrderForm.Services;
using OrderForm.Controls;
using System.Configuration;
using System.Linq;

namespace OrderForm
{
    public partial class Form1 : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;
        private List<FlightDto> _flights = new List<FlightDto>();
        private List<SeatDto> _availableSeats = new List<SeatDto>();
        private List<SeatDto> _allSeats = new List<SeatDto>();
        private PassengerDto? _currentPassenger;
        private FlightDto? _selectedFlight;
        private List<PassengerDto> _flightPassengers = new List<PassengerDto>();
        
        // Добавляем контрол для отображения карты мест
        private SimpleSeatMap _seatMap;

        public Form1()
        {
            InitializeComponent();
            
            // App.config файлаас URL-г уншина
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(_apiBaseUrl);

            // Формын тохиргоо
            this.Text = "Нислэгийн бүртгэлийн систем";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.AutoScroll = true;
            
            // Инициализируем контрол для отображения карты мест
            _seatMap = new SimpleSeatMap();
            _seatMap.Location = new Point(40, 250);
            _seatMap.Size = new Size(700, 450);
            _seatMap.SeatSelected += SeatMap_SeatSelected;
            this.Controls.Add(_seatMap);
            
            // Добавляем лейбл с информацией о выбранном месте
            Label lblSeatMapHeader = new Label();
            lblSeatMapHeader.Text = "Суудлын зураг";
            lblSeatMapHeader.Font = new Font("Arial", 12, FontStyle.Bold);
            lblSeatMapHeader.AutoSize = true;
            lblSeatMapHeader.Location = new Point(450, 190);
            this.Controls.Add(lblSeatMapHeader);
            
            // Формын уншиж эхлэх үйл явц
            this.Load += async (s, e) => await LoadFlightsAsync();
            
            // Суудал бүртгэх товчны эвентийг холбох
            this.btnCheckIn.Click += AsyncCheckInPassenger;
        }
        

        
        // Өгөгдлийн сан үүсгэх функц хассан
        
        private async Task LoadFlightsAsync()
        {
            try
            {
                lblStatus.Text = "Нислэгүүдийг ачааллаж байна...";
                _flights = await _apiService.GetAllFlightsAsync();
                
                if (_flights.Count > 0)
                {
                    cmbFlights.Items.Clear();
                    foreach (var flight in _flights)
                    {
                        string status = GetStatusText(flight.Status);
                        string item = $"{flight.FlightNumber} - {flight.DepartureCity} -> {flight.ArrivalCity} - {flight.DepartureTime.ToString("yyyy-MM-dd HH:mm")} - {status}";
                        cmbFlights.Items.Add(item);
                    }
                    
                    if (cmbFlights.Items.Count > 0)
                    {
                        cmbFlights.SelectedIndex = 0;
                    }
                    
                    lblStatus.Text = $"{_flights.Count} нислэг олдлоо";
                }
                else
                {
                    lblStatus.Text = "Нислэг олдсонгүй";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгүүдийг ачааллах үед алдаа гарлаа: {ex.Message}", "Алдаа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }
        
        private string GetStatusText(FlightStatus status)
        {
            switch (status)
            {
                case FlightStatus.CheckingIn:
                    return "Бүртгэж байна";
                case FlightStatus.Boarding:
                    return "Суулгаж байгаа";
                case FlightStatus.Departed:
                    return "Хөөрсөн";
                case FlightStatus.Cancelled:
                    return "Цуцлагдсан";
                case FlightStatus.Delayed:
                    return "Хойшлогдсон";
                default:
                    return status.ToString();
            }
        }
        
        private FlightDto? GetSelectedFlight()
        {
            int selectedIndex = cmbFlights.SelectedIndex;
            return selectedIndex >= 0 && selectedIndex < _flights.Count ? _flights[selectedIndex] : null;
        }
        
        private async void cmbFlights_SelectedIndexChanged(object sender, EventArgs e)
        {
            await FlightSelectedAsync();
        }
        
        private async Task FlightSelectedAsync()
        {
            _selectedFlight = GetSelectedFlight();
            
            if (_selectedFlight != null)
            {
                lblFlightInfo.Text = $"Нислэг: {_selectedFlight.FlightNumber} - {_selectedFlight.DepartureCity} -> {_selectedFlight.ArrivalCity}";
                
                // Нислэгийн зорчигчид болон суудлыг авах
                await LoadFlightPassengersAsync(_selectedFlight.Id);
                await LoadAllSeatsAsync(_selectedFlight.Id); // загрузка оригинальных данных о местах
                await UpdateSeatMapAsync(_selectedFlight.Id); // обновление карты мест
            }
            else
            {
                // Нислэг сонгогдоогүй - сбрасываем карту мест
                lblFlightInfo.Text = "Нислэг сонгоно уу";
                _seatMap.Controls.Clear();
                _allSeats.Clear();
            }
        }
        
        // Метод для обновления карты мест
        private async Task UpdateSeatMapAsync(int flightId)
        {
            try
            {
                lblStatus.Text = "Суудлын мэдээллийг ачааллаж байна...";
                
                // Получаем места с API
                var seats = await _apiService.GetAllSeatsAsync(flightId);
                
                // Обновляем карту мест
                _seatMap.CreateSeatMap(seats);
                
                lblStatus.Text = $"{seats.Count} суудал олдлоо";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлын мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Суудлын мэдээлэл авахад алдаа гарлаа";
            }
        }
        
        // Суудал сонгосон үед дуудагдах үйл явдал
        private void SeatMap_SeatSelected(object sender, SeatEventArgs e)
        {
            // Хэрэв суудал сонгогдсон бол
            if (!string.IsNullOrEmpty(e.SeatNumber))
            {
                // Суудлын дугаарыг хадгалах
                txtSeatNumber.Text = e.SeatNumber;
                
                // Бүртгэх товчийг идэвхжүүлэх
                btnCheckIn.Enabled = (_currentPassenger != null);
            }
            else
            {
                // Суудал сонгогдоогүй (сонголт цуцлагдсан)
                txtSeatNumber.Text = "";
                btnCheckIn.Enabled = false;
            }
        }
        
        private async Task LoadFlightPassengersAsync(int flightId)
        {
            try
            {
                lblStatus.Text = "Зорчигчдыг ачааллаж байна...";
                
                _flightPassengers = await _apiService.GetPassengersByFlightIdAsync(flightId);
                
                dgvPassengers.Rows.Clear();
                
                foreach (var passenger in _flightPassengers)
                {
                    var boardingPass = await _apiService.GetBoardingPassByFlightAndPassengerAsync(flightId, passenger.Id);
                    string seatNumber = boardingPass != null ? boardingPass.SeatNumber : "Суудалгүй";
                    
                    dgvPassengers.Rows.Add(
                        passenger.Id,
                        passenger.FirstName,
                        passenger.LastName,
                        passenger.PassportNumber,
                        seatNumber
                    );
                }
                
                lblPassengerCount.Text = $"Нийт: {_flightPassengers.Count} зорчигч";
                lblStatus.Text = "Зорчигчдыг амжилттай ачааллалаа";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигчдыг ачааллах үед алдаа гарлаа: {ex.Message}", "Алдаа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }
        
        private async Task LoadAllSeatsAsync(int flightId)
        {
            try
            {
                lblStatus.Text = "Суудлуудыг ачааллаж байна...";
                
                _availableSeats = await _apiService.GetAvailableSeatsAsync(flightId);
                _allSeats = await _apiService.GetAllSeatsForFlightAsync(flightId);
                
                if (_allSeats.Count > 0)
                {
                    // Суудлын мэдээллийг харуулах статус хэсэгт
                    lblStatus.Text = $"Нийт: {_allSeats.Count} суудал, Боломжтой: {_availableSeats.Count} суудал";
                }
                else
                {
                    lblStatus.Text = "Суудал олдсонгүй";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлуудыг ачааллах үед алдаа гарлаа: {ex.Message}", "Алдаа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }
        
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string passportNumber = txtPassportSearch.Text.Trim();
            
            if (!string.IsNullOrEmpty(passportNumber))
            {
                await SearchPassengerAsync(passportNumber);
            }
        }
        
        private async Task SearchPassengerAsync(string passportNumber)
        {
            try
            {
                lblStatus.Text = "Зорчигч хайж байна...";
                
                _currentPassenger = await _apiService.GetPassengerByPassportNumberAsync(passportNumber);
                
                if (_currentPassenger != null)
                {
                    txtPassengerInfo.Text = $"{_currentPassenger.FirstName} {_currentPassenger.LastName} - {_currentPassenger.PassportNumber}";
                    lblStatus.Text = "Зорчигч олдлоо";
                    
                    // Зорчигчийг хүснэгтэн дээр олж тэмдэглэх
                    for (int i = 0; i < dgvPassengers.Rows.Count; i++)
                    {
                        if (dgvPassengers.Rows[i].Cells[3].Value.ToString() == passportNumber)
                        {
                            dgvPassengers.ClearSelection();
                            dgvPassengers.Rows[i].Selected = true;
                            dgvPassengers.FirstDisplayedScrollingRowIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    txtPassengerInfo.Text = "Зорчигч олдсонгүй";
                    lblStatus.Text = "Зорчигч олдсонгүй";
                    _currentPassenger = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч хайх үед алдаа гарлаа: {ex.Message}", "Алдаа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
                _currentPassenger = null;
            }
        }
        
        private void dgvPassengers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvPassengers.Rows.Count)
            {
                string passportNumber = dgvPassengers.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtPassportSearch.Text = passportNumber;
                btnSearch.PerformClick();
            }
        }
        
        private async void btnCheckIn_Click(object sender, EventArgs e)
        {
            if (_selectedFlight != null && _currentPassenger != null)
            {
                string seatNumber = txtSeatNumber.Text.Trim();
                
                if (!string.IsNullOrEmpty(seatNumber))
                {
                    await CheckInPassengerAsync(_selectedFlight.Id, _currentPassenger.PassportNumber, seatNumber);
                }
                else
                {
                    MessageBox.Show("Суудлын дугаар оруулна уу", "Анхааруулга", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Нислэг болон зорчигчийг сонгоно уу", "Анхааруулга", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private async Task CheckInPassengerAsync(int flightId, string passportNumber, string seatNumber)
        {
            try
            {
                lblStatus.Text = "Зорчигчийг бүртгэж байнашдээ...";
                
                var boardingPass = await _apiService.CheckInPassengerAsync(flightId, passportNumber, seatNumber);
                
                // Боардинг пасс үүссэн бол явсан гэж үзэх
                if (boardingPass != null)
                {
                    MessageBox.Show("Зорчигч амжилттай бүртгэгдлээ!", "Амжилттай", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    lblStatus.Text = "Зорчигч амжилттай бүртгэгдлээ";
                    
                    // Тасалбар хэвлэх
                    ShowBoardingPass(boardingPass);
                    
                    // Одоо аппликацийг дахин дуудаж хэрэглэгчийн интерфейсийг шинэчлэх
                    var result = MessageBox.Show("Одоо суудал амжилттай оноогдсон. Өөрчлөлтүүдийг харахын тулд програмыг дахин ачаалах шаардлагатай.", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Програмыг дахин ачаална (оператор return-г ашиглан функцийг дуусгана)
                    Application.Restart();
                    return; // Методоос гарна, дараах код хэзээ ч ажиллахгүй
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэх үед алдаа гарлаа: {ex.Message}", "Алдаа", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }
        
        private void PrintBoardingPass(BoardingPassDto boardingPass)
        {
            // Энгийн тасалбар харуулах
            var passenger = _flightPassengers.FirstOrDefault(p => p.Id == boardingPass.PassengerId);
            var flight = _flights.FirstOrDefault(f => f.Id == boardingPass.FlightId);
            
            if (passenger != null && flight != null)
            {
                string message = $"ТАСАЛБАР\n\n" +
                                $"Нислэгийн дугаар: {flight.FlightNumber}\n" +
                                $"Хөөрөх: {flight.DepartureCity} - {flight.DepartureTime.ToString("yyyy-MM-dd HH:mm")}\n" +
                                $"Очих: {flight.ArrivalCity}\n\n" +
                                $"Зорчигч: {passenger.FirstName} {passenger.LastName}\n" +
                                $"Паспорт: {passenger.PassportNumber}\n" +
                                $"Суудал: {boardingPass.SeatNumber}\n\n" +
                                $"Бүртгэсэн огноо: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                
                MessageBox.Show(message, "Тасалбар", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        // Бүртгэх товч дарагдах үед ажиллах асинхрон метод
        private async void AsyncCheckInPassenger(object sender, EventArgs e)
        {
            try
            {
                // Шаардлагатай мэдээллүүдийг шалгах
                if (_selectedFlight == null || _currentPassenger == null || string.IsNullOrEmpty(_seatMap.SelectedSeatNumber))
                {
                    MessageBox.Show("Нислэг, зорчигч болон суудал сонгоно уу.", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Зорчигчийг бүртгэх үйл явц
                lblStatus.Text = "Зорчигчийг бүртгэж байна...";
                
                // API-д хүсэлт илгээх
                var boardingPass = await _apiService.CheckInPassengerAsync(_selectedFlight.Id, _currentPassenger.PassportNumber, _seatMap.SelectedSeatNumber);
                
                if (boardingPass != null)
                {
                    // Амжилттай бүртгэгдсэн вестай
                    MessageBox.Show($"Зорчигч {_currentPassenger.FirstName} {_currentPassenger.LastName} амжилттай бүртгэгдлээ.\n" +
                                      $"Нислэг: {_selectedFlight.FlightNumber}\n" +
                                      $"Чиглэл: {_selectedFlight.DepartureCity} - {_selectedFlight.ArrivalCity}\n" +
                                      $"Суудал: {_seatMap.SelectedSeatNumber}",
                                      "Бүртгэл амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Тасалбар хэвлэх
                    ShowBoardingPass(boardingPass);
                    
                    // Одоо аппликацийг дахин дуудаж хэрэглэгчийн интерфейсийг шинэчлэх
                    var result = MessageBox.Show("Одоо суудал амжилттай оноогдсон. Өөрчлөлтүүдийг харахын тулд програмыг дахин ачаалах шаардлагатай.", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Програмыг дахин ачаална (оператор return-г ашиглан функцийг дуусгана)
                    Application.Restart();
                    return; // Методоос гарна, дараах код хэзээ ч ажиллахгүй
                    
                    lblStatus.Text = "Бүртгэл амжилттай хийгдсэн";
                }
                else
                {
                    MessageBox.Show("Бүртгэхэд алдаа гарлаа. Дахин оролдоно уу.", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Бүртгэхэд алдаа гарлаа";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Бүртгэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Бүртгэхэд алдаа гарлаа";
            }
        }
        
        // Тасалбарыг харуулах функц
        private void ShowBoardingPass(BoardingPassDto boardingPass)
        {
            try
            {
                // Тасалбарын мэдээллийг бэлтгэх
                string passengerName = _currentPassenger != null ? $"{_currentPassenger.FirstName} {_currentPassenger.LastName}" : "Unknown";
                string flightInfo = _selectedFlight != null ? $"{_selectedFlight.FlightNumber} ({_selectedFlight.DepartureCity} - {_selectedFlight.ArrivalCity})" : "Unknown";
                string departureTime = _selectedFlight != null ? _selectedFlight.DepartureTime.ToString("yyyy-MM-dd HH:mm") : "Unknown";
                string seatNumber = boardingPass.SeatNumber;
                
                // Хэвлэх эсвэл харуулах шаардлагатай мэдээллийг бэлтгэх
                string boardingPassInfo = 
                    $"========== НИСЛЭГИЙН СУУДЛЫН ТАСАЛБАР ==========\n\n" +
                    $"Зорчигчийн нэр: {passengerName}\n" +
                    $"Чек-ин хийсэн цаг: {DateTime.Now}\n\n" +
                    $"Нислэг: {flightInfo}\n" +
                    $"Хөөрөх цаг: {departureTime}\n" +
                    $"Суудал: {seatNumber}\n\n" +
                    $"=======================================\n" +
                    $"Таны нислэг амжилттай болохыг хүсье!";
                
                // Хэвлэх эсвэл тасалбарын дэлгэцэнд харуулах
                MessageBox.Show(boardingPassInfo, "Суудлын тасалбар", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Тасалбар хэвлэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ResetFormForNextPassenger()
        {
            // Зорчигчийн мэдээллийг цэвэрлэх
            txtPassportSearch.Clear();
            txtPassengerInfo.Clear();
            txtSeatNumber.Clear();
            _currentPassenger = null;
            
            // Суудлын сонголтыг цэвэрлэх
            _seatMap.ClearSelectedSeat();
            
            // Бүртгэх товчийг идэвхгүй болгох
            btnCheckIn.Enabled = false;
            
            // Хүснэгтэн дээрх сонголтыг цэвэрлэх
            dgvPassengers.ClearSelection();
        }
        
        // Дублирующийся метод PrintBoardingPass удален, теперь используется ShowBoardingPass
        
        private async void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_selectedFlight == null)
            {
                MessageBox.Show("Нислэг сонгоно уу", "Анхааруулга", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            using (FlightStatusForm statusForm = new FlightStatusForm(_selectedFlight.Status))
            {
                if (statusForm.ShowDialog() == DialogResult.OK)
                {
                    await UpdateFlightStatusAsync(_selectedFlight.Id, statusForm.SelectedStatus);
                }
            }
        }
        
        private async Task UpdateFlightStatusAsync(int flightId, FlightStatus newStatus)
        {
            try
            {
                bool success = await _apiService.UpdateFlightStatusAsync(flightId, newStatus);
                
                if (success)
                {
                    // Нислэгийн жагсаалтыг шинэчлэх
                    await LoadFlightsAsync();
                    
                    MessageBox.Show($"Нислэгийн төлөв {GetStatusText(newStatus)} болж шинэчлэгдлээ", 
                        "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Нислэгийн төлөв шинэчлэхэд алдаа гарлаа.", 
                        "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн төлөв шинэчлэхэд алдаа гарлаа: {ex.Message}", 
                    "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void OpenPassengerRegistration()
        {
            try
            {
                using (var form = new PassengerRegistrationForm())
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэх хуудас нээхэд алдаа гарлаа: {ex.Message}", 
                    "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void OpenFlightRegistration()
        {
            try
            {
                using (var form = new FlightRegistrationForm())
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэг бүртгэх хуудас нээхэд алдаа гарлаа: {ex.Message}", 
                    "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void passengerRegistrationMenuItem_Click(object sender, EventArgs e)
        {
            OpenPassengerRegistration();
        }
        
        private void flightRegistrationMenuItem_Click(object sender, EventArgs e)
        {
            OpenFlightRegistration();
        }
        
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Та системээс гарахдаа итгэлтэй байна уу?", "Гарах", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        
        // private async void btnRefresh_Click(object sender, EventArgs e)
        // {
        //     await LoadFlightsAsync();
            
        //     if (_selectedFlight != null)
        //     {
        //         await LoadFlightPassengersAsync(_selectedFlight.Id);
        //         await LoadAllSeatsAsync(_selectedFlight.Id);
        //     }
        // }
    }
    
    // Нислэгийн төлөв сонгох form
    public class FlightStatusForm : Form
    {
        private ComboBox cmbStatus;
        private Button btnOK;
        private Button btnCancel;
        
        public FlightStatus SelectedStatus { get; private set; }
        
        public FlightStatusForm(FlightStatus currentStatus)
        {
            SelectedStatus = currentStatus;
            InitializeComponents();
            SetupForm();
        }
        
        private void InitializeComponents()
        {
            cmbStatus = new ComboBox();
            btnOK = new Button();
            btnCancel = new Button();
            
            // ComboBox
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Location = new Point(12, 12);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(260, 23);
            foreach (var status in Enum.GetValues(typeof(FlightStatus)))
            {
                cmbStatus.Items.Add(status);
            }
            cmbStatus.SelectedItem = SelectedStatus;
            
            // OK Button
            btnOK.Location = new Point(116, 60);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 40);
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            
            // Cancel Button
            btnCancel.Location = new Point(200, 60);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(175, 40);
            btnCancel.Text = "Цуцлах";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            
            // Form
            Controls.Add(cmbStatus);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
        }
        
        private void SetupForm()
        {
            ClientSize = new Size(400, 300);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FlightStatusForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Нислэгийн төлөв өөрчлөх";
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cmbStatus.SelectedItem != null)
            {
                SelectedStatus = (FlightStatus)cmbStatus.SelectedItem;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Төлөв сонгоно уу", "Анхааруулга", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}