using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using DataAccess.Models;
using Newtonsoft.Json;
using OrderForm.Controls;

namespace OrderForm
{
    public partial class CheckInForm : Form
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private List<Flight> _flights = new List<Flight>();
        private List<Seat> _availableSeats = new List<Seat>();
        private List<Seat> _allSeats = new List<Seat>();
        private Passenger? _currentPassenger;
        private Flight? _selectedFlight;
        private SeatMapUserControl _seatMapControl;

        public CheckInForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            
            // App.config файлаас URL-г уншина
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";

            // Бүтэн дэлгэцээр харуулах
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            
            // Суудлын газрын зураг үүсгэх
            InitializeSeatMap();
            
            InitializeForm();
        }

        private void InitializeSeatMap()
        {
            // Суудлын зураг харуулах UserControl үүсгэх
            _seatMapControl = new SeatMapUserControl();
            _seatMapControl.Dock = DockStyle.Fill;
            _seatMapControl.SeatSelected += SeatMapControl_SeatSelected;
            
            // GroupBox-д нэмэх
            groupBox3.Controls.Add(_seatMapControl);
        }

        private void SeatMapControl_SeatSelected(object sender, SeatSelectedEventArgs e)
        {
            // Суудал сонгогдсон үед Check-in товчийг идэвхжүүлэх
            if (_currentPassenger != null && e.SelectedSeat != null)
            {
                btnCheckIn.Enabled = true;
            }
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
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/DbInitializer/initialize", null);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Database initialized successfully");
                }
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
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Flights");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? new List<Flight>();
                    
                    cmbFlights.Items.Clear();
                    foreach (var flight in _flights)
                    {
                        // Нислэгийн дэлгэрэнгүй мэдээллийг харуулах
                        var departureTime = flight.DepartureTime.ToString("yyyy-MM-dd HH:mm");
                        var arrivalTime = flight.ArrivalTime.ToString("yyyy-MM-dd HH:mm");
                        var statusText = GetStatusText(flight.Status);
                        
                        cmbFlights.Items.Add($"{flight.FlightNumber} - {flight.DepartureCity} → {flight.ArrivalCity} ({departureTime})");
                    }
                    
                    if (cmbFlights.Items.Count > 0)
                    {
                        cmbFlights.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("Одоогоор нислэг олдсонгүй", "Мэдээлэл", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн мэдээллийг ачаалахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Нислэгийн төлөвийг монгол хэл дээр хөрвүүлэх
        private string GetStatusText(FlightStatus status)
        {
            switch (status)
            {
                case FlightStatus.CheckingIn:
                    return "Бүртгэл хийгдэж байна";
                case FlightStatus.Boarding:
                    return "Суулгаж байна";
                case FlightStatus.Departed:
                    return "Хөөрсөн";
                case FlightStatus.Delayed:
                    return "Хойшилсон";
                case FlightStatus.Cancelled:
                    return "Цуцлагдсан";
                default:
                    return status.ToString();
            }
        }

        private void UpdateFlightStatus(FlightStatus status)
        {
            lblFlightStatus.Text = $"Нислэгийн төлөв: {GetStatusText(status)}";
            
            // Нислэгийн төлөвөөс хамаарч өөр өөр өнгөтэй харуулах
            switch (status)
            {
                case FlightStatus.CheckingIn:
                    lblFlightStatus.ForeColor = Color.Green;
                    break;
                case FlightStatus.Boarding:
                    lblFlightStatus.ForeColor = Color.Blue;
                    break;
                case FlightStatus.Departed:
                    lblFlightStatus.ForeColor = Color.Black;
                    break;
                case FlightStatus.Delayed:
                    lblFlightStatus.ForeColor = Color.Orange;
                    break;
                case FlightStatus.Cancelled:
                    lblFlightStatus.ForeColor = Color.Red;
                    break;
            }
        }

        private async Task LoadAllSeatsAsync(int flightId)
        {
            try
            {
                // Эхлээд бүх суудлуудыг авна
                var allSeatsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/flights/{flightId}/seats");
                if (allSeatsResponse.IsSuccessStatusCode)
                {
                    var allSeatsContent = await allSeatsResponse.Content.ReadAsStringAsync();
                    _allSeats = JsonConvert.DeserializeObject<List<Seat>>(allSeatsContent) ?? new List<Seat>();
                }
                
                // Боломжтой суудлуудыг авна
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/flights/{flightId}/seats/available");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _availableSeats = JsonConvert.DeserializeObject<List<Seat>>(content) ?? new List<Seat>();
                    
                    lblAvailableSeats.Text = $"Боломжит суудал: {_availableSeats.Count}";
                    
                    // Суудлын газрын зургийг шинэчилнэ - UserControl рүү дамжуулна
                    _seatMapControl.UpdateSeatMap(_availableSeats, _selectedFlight);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлыг ачаалахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void cmbFlights_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFlights.SelectedIndex >= 0)
            {
                _selectedFlight = _flights[cmbFlights.SelectedIndex];
                UpdateFlightStatus(_selectedFlight.Status);
                await LoadAllSeatsAsync(_selectedFlight.Id);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassportNumber.Text))
            {
                MessageBox.Show("Паспортын дугаар оруулна уу", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await SearchPassengerAsync(txtPassportNumber.Text);
        }

        private async Task SearchPassengerAsync(string passportNumber)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers/passport/{passportNumber}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _currentPassenger = JsonConvert.DeserializeObject<Passenger>(content);
                    
                    if (_currentPassenger != null)
                    {
                        txtPassengerName.Text = $"{_currentPassenger.FirstName} {_currentPassenger.LastName}";
                        // Суудал сонгогдсон тохиолдолд Check-in товч идэвхжүүлнэ
                        btnCheckIn.Enabled = _seatMapControl.SelectedSeat != null;
                    }
                }
                else
                {
                    MessageBox.Show($"{passportNumber} дугаартай паспорт бүхий зорчигч олдсонгүй.", "Олдсонгүй", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPassengerName.Text = string.Empty;
                    btnCheckIn.Enabled = false;
                    _currentPassenger = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч хайхад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnCheckIn_Click(object sender, EventArgs e)
        {
            if (_currentPassenger == null || _selectedFlight == null || _seatMapControl.SelectedSeat == null)
            {
                MessageBox.Show("Нислэг, зорчигч болон суудал сонгоно уу", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await CheckInPassengerAsync();
        }

        private async Task CheckInPassengerAsync()
        {
            try
            {
                var checkinRequest = new
                {
                    FlightId = _selectedFlight!.Id,
                    PassportNumber = _currentPassenger!.PassportNumber,
                    SeatNumber = _seatMapControl.SelectedSeat!.SeatNumber
                };

                var jsonContent = JsonConvert.SerializeObject(checkinRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Boarding/checkin", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var boardingPass = JsonConvert.DeserializeObject<BoardingPass>(responseContent);
                    
                    MessageBox.Show($"Бүртгэл амжилттай хийгдлээ! {checkinRequest.SeatNumber} суудалд суух нислэгийн бүртгэлийн бичиг олгогдлоо", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Суудлын жагсаалтыг шинэчлэх
                    await LoadAllSeatsAsync(_selectedFlight.Id);
                    
                    // Boarding pass хэвлэлтийг симуляци хийх
                    PrintBoardingPass(boardingPass!, _currentPassenger, _selectedFlight);
                    
                    // Формыг дараагийн зорчигчид бэлтгэх
                    ResetFormForNextPassenger();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Бүртгэл хийх үед алдаа гарлаа: {errorContent}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintBoardingPass(BoardingPass boardingPass, Passenger passenger, Flight flight)
        {
            // Энд PrintDialog ашиглаж болно, эсвэл консоль дээр дарааллыг харуулж болно
            // Одоогоор консолд хэвлэнэ
            Console.WriteLine("===== НИСЛЭГИЙН БҮРТГЭЛИЙН БИЧИГ =====");
            Console.WriteLine($"Нислэг: {flight.FlightNumber}");
            Console.WriteLine($"Хаанаас: {flight.DepartureCity} Хаашаа: {flight.ArrivalCity}");
            Console.WriteLine($"Огноо: {flight.DepartureTime.ToShortDateString()}");
            Console.WriteLine($"Цаг: {flight.DepartureTime.ToShortTimeString()}");
            Console.WriteLine($"Зорчигч: {passenger.FirstName} {passenger.LastName}");
            Console.WriteLine($"Паспорт: {passenger.PassportNumber}");
            Console.WriteLine($"Суудал: {boardingPass.Seat?.SeatNumber}");
            Console.WriteLine($"Бүртгэсэн цаг: {boardingPass.CheckInTime}");
            Console.WriteLine("======================================");
        }

        private void ResetFormForNextPassenger()
        {
            txtPassportNumber.Text = string.Empty;
            txtPassengerName.Text = string.Empty;
            btnCheckIn.Enabled = false;
            _currentPassenger = null;
            
            // Сонгосон суудлыг арилгах
            _seatMapControl.ClearSelectedSeat();
        }

        private async void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_selectedFlight == null)
            {
                MessageBox.Show("Нислэг сонгоно уу", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FlightStatusForm statusForm = new FlightStatusForm(_selectedFlight.Status);
            if (statusForm.ShowDialog() == DialogResult.OK)
            {
                await UpdateFlightStatusAsync(_selectedFlight.Id, statusForm.SelectedStatus);
            }
        }

        private async Task UpdateFlightStatusAsync(int flightId, FlightStatus newStatus)
        {
            try
            {
                var statusRequest = new { Status = newStatus };
                var jsonContent = JsonConvert.SerializeObject(statusRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Flights/{flightId}/status", content);
                
                if (response.IsSuccessStatusCode)
                {
                    UpdateFlightStatus(newStatus);
                    MessageBox.Show($"Нислэгийн төлөв {GetStatusText(newStatus)} болж шинэчлэгдлээ", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Нислэгийн статус өөрчлөгдсөний дараа боломжит суудлуудыг шинэчлэх
                    await LoadAllSeatsAsync(flightId);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Нислэгийн төлөв шинэчлэхэд алдаа гарлаа: {errorContent}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн төлөв шинэчлэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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
            btnOK.Location = new Point(116, 50);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 40);
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;

            // Cancel Button
            btnCancel.Location = new Point(197, 50);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 40);
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
            ClientSize = new Size(300, 100);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FlightStatusForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Нислэгийн төлөв өөрчлөх"; // Form цонхны гарчиг
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
                MessageBox.Show("Төлөв сонгоно уу", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
