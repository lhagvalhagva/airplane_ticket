using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using OrderForm.Models;
using OrderForm.Services;

namespace OrderForm.Pages
{
    public class FlightRegistrationForm : Form
    {
        private TextBox txtFlightNumber, txtDepartureCity, txtArrivalCity, txtCapacity;
        private DateTimePicker dtpDepartureTime, dtpArrivalTime;
        private Label lblFlightNumber, lblDepartureCity, lblArrivalCity, lblDepartureTime, lblArrivalTime, lblCapacity;
        private Button btnSave, btnCancel, btnClear, btnRefresh;
        private DataGridView dgvFlights;
        private List<FlightDto> _flights;
        private readonly ApiService _apiService;

        public FlightRegistrationForm()
        {
            string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(apiBaseUrl);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form тохиргоо
            this.Text = "Нислэг бүртгэх";
            this.Size = new Size(1900, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Гарчиг
            Label lblTitle = new()
            {
                Text = "Нислэг бүртгэх",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            // Нислэгийн дугаар
            lblFlightNumber = new Label
            {
                Text = "Нислэгийн дугаар:",
                AutoSize = true,
                Location = new Point(20, 70)
            };
            this.Controls.Add(lblFlightNumber);

            txtFlightNumber = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(150, 70)
            };
            this.Controls.Add(txtFlightNumber);

            // Хөдлөх хот
            lblDepartureCity = new Label
            {
                Text = "Хөдлөх хот:",
                AutoSize = true,
                Location = new Point(20, 110)
            };
            this.Controls.Add(lblDepartureCity);

            txtDepartureCity = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(150, 110)
            };
            this.Controls.Add(txtDepartureCity);

            // Очих хот
            lblArrivalCity = new Label
            {
                Text = "Очих хот:",
                AutoSize = true,
                Location = new Point(20, 150)
            };
            this.Controls.Add(lblArrivalCity);

            txtArrivalCity = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(150, 150)
            };
            this.Controls.Add(txtArrivalCity);

            // Хөдлөх цаг
            lblDepartureTime = new Label
            {
                Text = "Хөдлөх цаг:",
                AutoSize = true,
                Location = new Point(20, 190)
            };
            this.Controls.Add(lblDepartureTime);

            dtpDepartureTime = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm",
                Size = new Size(200, 25),
                Location = new Point(150, 190)
            };
            this.Controls.Add(dtpDepartureTime);

            // Очих цаг
            lblArrivalTime = new Label
            {
                Text = "Очих цаг:",
                AutoSize = true,
                Location = new Point(20, 230)
            };
            this.Controls.Add(lblArrivalTime);

            dtpArrivalTime = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm",
                Size = new Size(200, 25),
                Location = new Point(150, 230)
            };
            this.Controls.Add(dtpArrivalTime);

            // Суудлын тоо
            lblCapacity = new Label
            {
                Text = "Суудлын тоо:",
                AutoSize = true,
                Location = new Point(20, 270)
            };
            this.Controls.Add(lblCapacity);

            txtCapacity = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(150, 270)
            };
            this.Controls.Add(txtCapacity);

            // Товчнууд: Save, Clear, Cancel
            btnSave = new Button
            {
                Text = "Хадгалах",
                Size = new Size(100, 35),
                Location = new Point(50, 320),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnClear = new Button
            {
                Text = "Цэвэрлэх",
                Size = new Size(100, 35),
                Location = new Point(160, 320),
                BackColor = Color.Gray,
                ForeColor = Color.White
            };
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            btnCancel = new Button
            {
                Text = "Хаах",
                Size = new Size(100, 35),
                Location = new Point(270, 320),
                BackColor = Color.IndianRed,
                ForeColor = Color.White
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // Нислэгийн жагсаалт харуулах хэсэг
            Label lblFlightsList = new Label
            {
                Text = "Нислэгийн жагсаалт",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(500, 20)
            };
            this.Controls.Add(lblFlightsList);

            // Шинэчлэх товч
            btnRefresh = new Button
            {
                Text = "Шинэчлэх",
                Size = new Size(100, 35),
                Location = new Point(700, 20),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(btnRefresh);

            // Нислэгийн хүснэгт
            dgvFlights = new DataGridView
            {
                Location = new Point(500, 70),
                Size = new Size(1350, 800),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = SystemColors.Control
            };

            dgvFlights.Columns.Add("Id", "ID");
            dgvFlights.Columns.Add("FlightNumber", "Нислэгийн дугаар");
            dgvFlights.Columns.Add("DepartureCity", "Хөдлөх хот");
            dgvFlights.Columns.Add("ArrivalCity", "Очих хот");
            dgvFlights.Columns.Add("DepartureTime", "Хөдлөх цаг");
            dgvFlights.Columns.Add("ArrivalTime", "Очих цаг");
            dgvFlights.Columns.Add("Status", "Төлөв");
            dgvFlights.CellClick += DgvFlights_CellClick;
            this.Controls.Add(dgvFlights);

            // Form Load үед нислэгийн жагсаалт ачаална
            this.Load += async (s, e) => await LoadFlightsAsync();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var flight = new FlightDto
                {
                    FlightNumber = txtFlightNumber.Text.Trim(),
                    DepartureCity = txtDepartureCity.Text.Trim(),
                    ArrivalCity = txtArrivalCity.Text.Trim(),
                    DepartureTime = dtpDepartureTime.Value,
                    ArrivalTime = dtpArrivalTime.Value,
                    Status = FlightStatus.CheckingIn // Эхлэх үед default-д CheckingIn гэж авъя
                };

                bool success = await _apiService.RegisterFlightAsync(flight);
                if (success)
                {
                    MessageBox.Show("Нислэг амжилттай бүртгэгдлээ", "Амжилттай",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    await LoadFlightsAsync();
                }
                else
                {
                    MessageBox.Show("Нислэг бүртгэхэд алдаа гарлаа", "Алдаа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Алдаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            await LoadFlightsAsync();
        }

        private async Task LoadFlightsAsync()
        {
            try
            {
                _flights = await _apiService.GetAllFlightsAsync();
                dgvFlights.Rows.Clear();

                foreach (var flight in _flights)
                {
                    string statusText = GetFlightStatusText(flight.Status);
                    dgvFlights.Rows.Add(
                        flight.Id,
                        flight.FlightNumber,
                        flight.DepartureCity,
                        flight.ArrivalCity,
                        flight.DepartureTime.ToString("yyyy-MM-dd HH:mm"),
                        flight.ArrivalTime.ToString("yyyy-MM-dd HH:mm"),
                        statusText
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгүүдийг ачаалж чадсангүй: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvFlights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _flights.Count) return;

            var selectedFlight = _flights[e.RowIndex];
            txtFlightNumber.Text = selectedFlight.FlightNumber;
            txtDepartureCity.Text = selectedFlight.DepartureCity;
            txtArrivalCity.Text = selectedFlight.ArrivalCity;
            dtpDepartureTime.Value = selectedFlight.DepartureTime;
            dtpArrivalTime.Value = selectedFlight.ArrivalTime;
            txtCapacity.Text = ""; // Capacity API-д байхгүй тул хоосон үлдээе
        }

        private string GetFlightStatusText(FlightStatus status)
        {
            return status switch
            {
                FlightStatus.CheckingIn => "Бүртгэж байна",
                FlightStatus.Boarding   => "Онгоцонд сууж байна",
                FlightStatus.Departed   => "Хөөрсөн",
                FlightStatus.Delayed    => "Хойшлогдсон",
                FlightStatus.Cancelled  => "Цуцалсан",
                _ => "Үл мэдэгдэх"
            };
        }

        private void ClearFields()
        {
            txtFlightNumber.Clear();
            txtDepartureCity.Clear();
            txtArrivalCity.Clear();
            dtpDepartureTime.Value = DateTime.Now;
            dtpArrivalTime.Value = DateTime.Now.AddHours(2);
            txtCapacity.Clear();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtFlightNumber.Text.Trim()))
            {
                MessageBox.Show("Нислэгийн дугаар оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtDepartureCity.Text.Trim()))
            {
                MessageBox.Show("Хөдлөх хот оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtArrivalCity.Text.Trim()))
            {
                MessageBox.Show("Очих хот оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (dtpDepartureTime.Value >= dtpArrivalTime.Value)
            {
                MessageBox.Show("Очих цаг хөдлөх цагаас хойш байх ёстой", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(txtCapacity.Text.Trim()) ||
                !int.TryParse(txtCapacity.Text.Trim(), out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Суудлын тоо зөв оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
