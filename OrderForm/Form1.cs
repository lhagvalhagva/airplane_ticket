using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderForm.Models;
using OrderForm.Pages;
using OrderForm.Services;
using OrderForm.Controls;
using System.Configuration;

namespace OrderForm
{
    public partial class Form1 : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;

        private BindingList<PassengerDisplayDto> _passengerBindingList;

        private List<FlightDto> _flights = new List<FlightDto>();
        private FlightDto _selectedFlight;
        private PassengerDto _currentPassenger;

        private SimpleSeatMapControl _seatMapControl;

        public Form1()
        {
            InitializeComponent();

            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(_apiBaseUrl);

            this.Text = "Нислэгийн бүртгэлийн систем";
            this.WindowState = FormWindowState.Maximized;
            this.AutoScroll = true;

            // SimpleSeatMapControl-ыг Panel дээрээ байрлуулах
            _seatMapControl = new SimpleSeatMapControl
            {
                Dock = DockStyle.Fill
            };
            panelSeatMapContainer.Controls.Add(_seatMapControl);
            _seatMapControl.SeatSelected += SeatMapControl_SeatSelected;

            InitializePassengerGrid();

            this.Load += Form1_Load;
            cmbFlights.SelectedIndexChanged += CmbFlights_SelectedIndexChanged;
            btnSearch.Click += BtnSearch_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            dgvPassengers.CellClick += DgvPassengers_CellClick;
            btnChangeStatus.Click += BtnChangeStatus_Click;
            passengerRegistrationMenuItem.Click += PassengerRegistrationMenuItem_Click;
            flightRegistrationMenuItem.Click += FlightRegistrationMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;

            btnCheckIn.Enabled = false;
        }

        #region --- Initialize & Passenger Grid ---

        private void Form1_Load(object sender, EventArgs e)
        {
            _ = LoadFlightsAsync();
        }

        private void InitializePassengerGrid()
        {
            _passengerBindingList = new BindingList<PassengerDisplayDto>();
            dgvPassengers.AutoGenerateColumns = false;
            dgvPassengers.DataSource = _passengerBindingList;

            var colId = new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PassengerDisplayDto.Id),
                Name = "PassengerId",
                Visible = false
            };
            dgvPassengers.Columns.Add(colId);

            dgvPassengers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PassengerDisplayDto.FirstName),
                HeaderText = "Нэр",
                Width = 135
            });

            dgvPassengers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PassengerDisplayDto.LastName),
                HeaderText = "Овог",
                Width = 135
            });

            dgvPassengers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PassengerDisplayDto.PassportNumber),
                HeaderText = "Паспортын дугаар",
                Width = 155
            });

            dgvPassengers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(PassengerDisplayDto.SeatNumber),
                HeaderText = "Суудал",
                Width = 135
            });
        }

        #endregion

        #region --- Load Flights & Display ---

        private async Task LoadFlightsAsync()
        {
            try
            {
                lblStatus.Text = "Нислэгүүдийг ачааллаж байна...";
                _flights = await _apiService.GetAllFlightsAsync();

                if (_flights.Count > 0)
                {
                    cmbFlights.DataSource = new BindingList<FlightDto>(_flights);
                    cmbFlights.DisplayMember = nameof(FlightDto.FlightNumber);
                    cmbFlights.ValueMember = nameof(FlightDto.Id);

                    cmbFlights.SelectedIndex = 0;
                    lblStatus.Text = $"{_flights.Count} нислэг олдлоо";
                }
                else
                {
                    cmbFlights.DataSource = null;
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

        private void CmbFlights_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ = FlightSelectedAsync();
        }

        private async Task FlightSelectedAsync()
        {
            if (cmbFlights.SelectedItem is not FlightDto flight)
            {
                lblFlightInfo.Text = "Нислэг сонгоно уу";
                _seatMapControl.ClearSeatMap();
                _passengerBindingList.Clear();
                lblPassengerCount.Text = string.Empty;
                return;
            }
            _selectedFlight = flight;

            lblFlightInfo.Text = $"Нислэг: {_selectedFlight.FlightNumber} - {_selectedFlight.DepartureCity} → {_selectedFlight.ArrivalCity}";

            await LoadFlightPassengersAsync(_selectedFlight.Id);
            await LoadAllSeatsAsync(_selectedFlight.Id);
            await UpdateSeatMapAsync(_selectedFlight.Id);
        }

        #endregion

        #region --- Load & Search Passenger ---

        private async Task LoadFlightPassengersAsync(int flightId)
        {
            try
            {
                lblStatus.Text = "Зорчигчдыг ачааллаж байна...";
                var passengers = await _apiService.GetPassengersByFlightIdAsync(flightId);

                _passengerBindingList.Clear();
                foreach (var passenger in passengers)
                {
                    var boardingPass = await _apiService.GetBoardingPassByFlightAndPassengerAsync(flightId, passenger.Id);
                    _passengerBindingList.Add(new PassengerDisplayDto
                    {
                        Id = passenger.Id,
                        FirstName = passenger.FirstName,
                        LastName = passenger.LastName,
                        PassportNumber = passenger.PassportNumber,
                        SeatNumber = boardingPass?.SeatNumber ?? "Суудалгүй"
                    });
                }

                lblPassengerCount.Text = $"Нийт: {_passengerBindingList.Count} зорчигч";
                lblStatus.Text = "Зорчигчдыг амжилттай ачааллалаа";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигчдыг ачааллах үед алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
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

                    int index = _passengerBindingList.ToList()
                        .FindIndex(p => p.PassportNumber.Equals(passportNumber, StringComparison.OrdinalIgnoreCase));
                    if (index >= 0)
                    {
                        dgvPassengers.ClearSelection();
                        dgvPassengers.Rows[index].Selected = true;
                        dgvPassengers.FirstDisplayedScrollingRowIndex = index;
                    }
                }
                else
                {
                    txtPassengerInfo.Text = "Зорчигч олдсонгүй";
                    lblStatus.Text = "Зорчигч олдсонгүй";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч хайх үед алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }

        private void DgvPassengers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedRow = _passengerBindingList[e.RowIndex];
            txtPassportSearch.Text = selectedRow.PassportNumber;
            _ = SearchPassengerAsync(selectedRow.PassportNumber);
        }

        #endregion

        #region --- Load & Update Seat Map ---

        private async Task LoadAllSeatsAsync(int flightId)
        {
            try
            {
                lblStatus.Text = "Суудлуудыг ачааллаж байна...";
                var allSeats = await _apiService.GetAllSeatsForFlightAsync(flightId);
                var availableSeats = await _apiService.GetAvailableSeatsAsync(flightId);

                lblStatus.Text = $"Нийт: {allSeats.Count} суудал, Боломжтой: {availableSeats.Count} суудал";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлуудыг ачааллах үед алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }

        private async Task UpdateSeatMapAsync(int flightId)
        {
            try
            {
                lblStatus.Text = "Суудлын мэдээллийг ачааллаж байна...";
                var seats = await _apiService.GetAllSeatsForFlightAsync(flightId);
                _seatMapControl.CreateSeatMap(seats);
                lblStatus.Text = $"{seats.Count} суудал олдлоо";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Суудлын мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }

        private void SeatMapControl_SeatSelected(object sender, SeatEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.SeatNumber))
            {
                txtSeatNumber.Text = e.SeatNumber;
                btnCheckIn.Enabled = (_currentPassenger != null && _selectedFlight != null);
            }
            else
            {
                txtSeatNumber.Clear();
                btnCheckIn.Enabled = false;
            }
        }

        #endregion

        #region --- Check-In Бүртгэх ---

        private async void BtnCheckIn_Click(object sender, EventArgs e)
        {
            if (_selectedFlight == null || _currentPassenger == null || string.IsNullOrEmpty(txtSeatNumber.Text))
            {
                MessageBox.Show("Нислэг, зорчигч болон суудал сонгоно уу.", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await CheckInPassengerAsync(_selectedFlight.Id, _currentPassenger.PassportNumber, txtSeatNumber.Text.Trim());
        }

        private async Task CheckInPassengerAsync(int flightId, string passportNumber, string seatNumber)
        {
            try
            {
                lblStatus.Text = "Зорчигчийг бүртгэж байна...";
                var boardingPass = await _apiService.CheckInPassengerAsync(flightId, passportNumber, seatNumber);

                if (boardingPass != null)
                {
                    MessageBox.Show("Зорчигч амжилттай бүртгэгдлээ!", "Амжилттай",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    lblStatus.Text = "Зорчигч амжилттай бүртгэгдлээ";

                    ShowBoardingPass(boardingPass);

                    await LoadFlightPassengersAsync(flightId);
                    await UpdateSeatMapAsync(flightId);
                }
                else
                {
                    MessageBox.Show("Бүртгэхэд алдаа гарлаа. Дахин оролдоно уу.", "Алдаа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Бүртгэхэд алдаа гарлаа";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэх үед алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Алдаа гарлаа";
            }
        }

        private void ShowBoardingPass(BoardingPassDto boardingPass)
        {
            try
            {
                if (_currentPassenger == null || _selectedFlight == null) return;

                string passengerName = $"{_currentPassenger.FirstName} {_currentPassenger.LastName}";
                string flightInfo = $"{_selectedFlight.FlightNumber} ({_selectedFlight.DepartureCity} → {_selectedFlight.ArrivalCity})";
                string departureTime = _selectedFlight.DepartureTime.ToString("yyyy-MM-dd HH:mm");
                string seatNo = boardingPass.SeatNumber;

                string boardingPassInfo =
                    $"========== НИСЛЭГИЙН СУУДЛЫН ТАСАЛБАР ==========\n\n" +
                    $"Зорчигчийн нэр: {passengerName}\n" +
                    $"Чек-ин хийсэн цаг: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n" +
                    $"Нислэг: {flightInfo}\n" +
                    $"Хөөрөх цаг: {departureTime}\n" +
                    $"Суудал: {seatNo}\n\n" +
                    $"=======================================\n" +
                    $"Таны нислэг амжилттай болохыг хүсье!";

                MessageBox.Show(boardingPassInfo, "Суудлын тасалбар", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Тасалбар харуулахад алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region --- Flight Status Update ---

        private async void BtnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_selectedFlight == null)
            {
                MessageBox.Show("Нислэг сонгоно уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var statusForm = new FlightStatusForm(_selectedFlight.Status);
            if (statusForm.ShowDialog() == DialogResult.OK)
            {
                await UpdateFlightStatusAsync(_selectedFlight.Id, statusForm.SelectedStatus);
            }
        }

        private async Task UpdateFlightStatusAsync(int flightId, FlightStatus newStatus)
        {
            try
            {
                bool success = await _apiService.UpdateFlightStatusAsync(flightId, newStatus);
                if (success)
                {
                    await LoadFlightsAsync();
                    MessageBox.Show($"Нислэгийн төлөв '{GetStatusText(newStatus)}' болж шинэчлэгдлээ", "Амжилттай",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Нислэгийн төлөв шинэчлэхэд алдаа гарлаа.", "Алдаа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн төлөв шинэчлэхэд алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region --- Menu Actions ---

        private void PassengerRegistrationMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new PassengerRegistrationForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэх хуудас нээхэд алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FlightRegistrationMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using var form = new FlightRegistrationForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэг бүртгэх хуудас нээхэд алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Та системээс гарахдаа итгэлтэй байна уу?", "Гарах",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        #endregion

        #region --- Helper Classes ---

        private class PassengerDisplayDto
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string PassportNumber { get; set; } = string.Empty;
            public string SeatNumber { get; set; } = string.Empty;
        }
        #endregion

        #region --- Status Text Helper ---

        private string GetStatusText(FlightStatus status)
        {
            return status switch
            {
                FlightStatus.CheckingIn => "Бүртгэж байна",
                FlightStatus.Boarding   => "Суулгаж байгаа",
                FlightStatus.Departed   => "Хөөрсөн",
                FlightStatus.Delayed    => "Хойшлогдсон",
                FlightStatus.Cancelled  => "Цуцлагдсан",
                _                      => status.ToString()
            };
        }
        #endregion
    }
}
