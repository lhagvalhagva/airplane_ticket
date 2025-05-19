using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Text;
using OrderForm.Models;
using OrderForm.Services;

namespace OrderForm.Pages
{
    public class FlightRegistrationForm : Form
    {
        private TextBox txtFlightNumber;
        private Label lblFlightNumber;
        private TextBox txtDepartureCity;
        private Label lblDepartureCity;
        private TextBox txtArrivalCity;
        private Label lblArrivalCity;
        private DateTimePicker dtpDepartureTime;
        private Label lblDepartureTime;
        private DateTimePicker dtpArrivalTime;
        private Label lblArrivalTime;
        private TextBox txtCapacity;
        private Label lblCapacity;
        private Button btnSave;
        private Button btnCancel;
        private Button btnClear;
        private DataGridView dgvFlights;
        private Button btnRefresh;
        private List<FlightDto> _flights;
        private ApiService _apiService;

        public FlightRegistrationForm()
        {
            // Получаем базовый URL из App.config
            string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];
            _apiService = new ApiService(apiBaseUrl);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Формын тохиргоо
            this.Text = "Нислэг бүртгэх";
            this.Size = new Size(1900, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            // Бүртгэлтийн хэсэг, зүүн хэсэг

            // Гарчиг текст
            Label lblTitle = new Label();
            lblTitle.Text = "Нислэг бүртгэх";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);
            this.Controls.Add(lblTitle);

            // Нислэгийн дугаар
            lblFlightNumber = new Label();
            lblFlightNumber.Text = "Нислэгийн дугаар:";
            lblFlightNumber.AutoSize = true;
            lblFlightNumber.Location = new Point(20, 70);
            this.Controls.Add(lblFlightNumber);

            txtFlightNumber = new TextBox();
            txtFlightNumber.Size = new Size(200, 25);
            txtFlightNumber.Location = new Point(150, 70);
            this.Controls.Add(txtFlightNumber);

            // Хөдөлөх хот
            lblDepartureCity = new Label();
            lblDepartureCity.Text = "Хөдөлөх хот:";
            lblDepartureCity.AutoSize = true;
            lblDepartureCity.Location = new Point(20, 110);
            this.Controls.Add(lblDepartureCity);

            txtDepartureCity = new TextBox();
            txtDepartureCity.Size = new Size(200, 25);
            txtDepartureCity.Location = new Point(150, 110);
            this.Controls.Add(txtDepartureCity);

            // Очих хот
            lblArrivalCity = new Label();
            lblArrivalCity.Text = "Очих хот:";
            lblArrivalCity.AutoSize = true;
            lblArrivalCity.Location = new Point(20, 150);
            this.Controls.Add(lblArrivalCity);

            txtArrivalCity = new TextBox();
            txtArrivalCity.Size = new Size(200, 25);
            txtArrivalCity.Location = new Point(150, 150);
            this.Controls.Add(txtArrivalCity);

            // Хөдлөх цаг
            lblDepartureTime = new Label();
            lblDepartureTime.Text = "Хөдлөх цаг:";
            lblDepartureTime.AutoSize = true;
            lblDepartureTime.Location = new Point(20, 190);
            this.Controls.Add(lblDepartureTime);

            dtpDepartureTime = new DateTimePicker();
            dtpDepartureTime.Format = DateTimePickerFormat.Custom;
            dtpDepartureTime.CustomFormat = "yyyy-MM-dd HH:mm";
            dtpDepartureTime.Size = new Size(200, 25);
            dtpDepartureTime.Location = new Point(150, 190);
            this.Controls.Add(dtpDepartureTime);

            // Очих цаг
            lblArrivalTime = new Label();
            lblArrivalTime.Text = "Очих цаг:";
            lblArrivalTime.AutoSize = true;
            lblArrivalTime.Location = new Point(20, 230);
            this.Controls.Add(lblArrivalTime);

            dtpArrivalTime = new DateTimePicker();
            dtpArrivalTime.Format = DateTimePickerFormat.Custom;
            dtpArrivalTime.CustomFormat = "yyyy-MM-dd HH:mm";
            dtpArrivalTime.Size = new Size(200, 25);
            dtpArrivalTime.Location = new Point(150, 230);
            this.Controls.Add(dtpArrivalTime);

            // Суудлын тоо
            lblCapacity = new Label();
            lblCapacity.Text = "Суудлын тоо:";
            lblCapacity.AutoSize = true;
            lblCapacity.Location = new Point(20, 270);
            this.Controls.Add(lblCapacity);

            txtCapacity = new TextBox();
            txtCapacity.Size = new Size(200, 25);
            txtCapacity.Location = new Point(150, 270);
            this.Controls.Add(txtCapacity);

            // Товчнууд
            btnSave = new Button();
            btnSave.Text = "Хадгалах";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(50, 320);
            btnSave.BackColor = Color.SteelBlue;
            btnSave.ForeColor = Color.White;
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);

            btnClear = new Button();
            btnClear.Text = "Цэвэрлэх";
            btnClear.Size = new Size(100, 35);
            btnClear.Location = new Point(160, 320);
            btnClear.BackColor = Color.Gray;
            btnClear.ForeColor = Color.White;
            btnClear.Click += btnClear_Click;
            this.Controls.Add(btnClear);

            btnCancel = new Button();
            btnCancel.Text = "Хаах";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(270, 320);
            btnCancel.BackColor = Color.IndianRed;
            btnCancel.ForeColor = Color.White;
            btnCancel.Click += btnCancel_Click;
            this.Controls.Add(btnCancel);
            
            // Нислэгийн жагсаалтын хэсэг, баруун талд
            Label lblFlightsList = new Label();
            lblFlightsList.Text = "Нислэгийн жагсаалт";
            lblFlightsList.Font = new Font("Arial", 16, FontStyle.Bold);
            lblFlightsList.AutoSize = true;
            lblFlightsList.Location = new Point(500, 20);
            this.Controls.Add(lblFlightsList);
            
            // Шинэчлэх товч
            btnRefresh = new Button();
            btnRefresh.Text = "Шинэчлэх";
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.Location = new Point(700, 20);
            btnRefresh.BackColor = Color.SteelBlue;
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Click += btnRefresh_Click;
            this.Controls.Add(btnRefresh);
            
            // Нислэгийн хүснэгт
            dgvFlights = new DataGridView();
            dgvFlights.Location = new Point(500, 70);
            dgvFlights.Size = new Size(1350, 800);
            dgvFlights.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFlights.AllowUserToAddRows = false;
            dgvFlights.AllowUserToDeleteRows = false;
            dgvFlights.ReadOnly = true;
            dgvFlights.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFlights.MultiSelect = false;
            dgvFlights.BackgroundColor = SystemColors.Control;
            dgvFlights.CellClick += dgvFlights_CellClick;
            
            // Баганууд
            dgvFlights.Columns.Add("Id", "ID");
            dgvFlights.Columns.Add("FlightNumber", "Нислэгийн дугаар");
            dgvFlights.Columns.Add("DepartureCity", "Хөдлөх хот");
            dgvFlights.Columns.Add("ArrivalCity", "Очих хот");
            dgvFlights.Columns.Add("DepartureTime", "Хөдлөх цаг");
            dgvFlights.Columns.Add("ArrivalTime", "Очих цаг");
            dgvFlights.Columns.Add("Status", "Төлөв");
            this.Controls.Add(dgvFlights);
            
            // Форм ачаалагдахад рейсүүдийг авах
            this.Load += async (s, e) => await LoadFlightsAsync();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                try
                {
                    // Создаваем класс для отправки данных о рейсе
                    var flight = new FlightDto
                    {
                        FlightNumber = txtFlightNumber.Text,
                        DepartureCity = txtDepartureCity.Text,
                        ArrivalCity = txtArrivalCity.Text,
                        DepartureTime = dtpDepartureTime.Value,
                        ArrivalTime = dtpArrivalTime.Value
                        // Свойство Capacity передаем через дополнительный параметр
                    };
                    
                    // Добавляем количество мест в запрос
                    int capacity = int.Parse(txtCapacity.Text);
                    
                    // Так как в API нет прямой передачи параметра capacity,
                    // мы можем добавить его в новое поле в URL или реализовать генерацию мест на стороне формы
                    // Регистрируем рейс через API
                    bool success = await _apiService.RegisterFlightAsync(flight);

                    if (success)
                    {
                        MessageBox.Show("Нислэг амжилттай бүртгэгдлээ", "Амжилттай",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        
                        // Нислэгийн жагсаалтыг шинэчлэх
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
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadFlightsAsync();
        }
        
        private async Task LoadFlightsAsync()
        {
            try
            {
                // Загрузка списка рейсов через API
                _flights = await _apiService.GetAllFlightsAsync();
                
                // Очистка таблицы
                dgvFlights.Rows.Clear();
                
                // Заполнение таблицы данными
                foreach (var flight in _flights)
                {
                    string status = GetFlightStatusText(flight.Status);
                    
                    dgvFlights.Rows.Add(
                        flight.Id,
                        flight.FlightNumber,
                        flight.DepartureCity,
                        flight.ArrivalCity,
                        flight.DepartureTime.ToString("yyyy-MM-dd HH:mm"),
                        flight.ArrivalTime.ToString("yyyy-MM-dd HH:mm"),
                        status
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгүүдийг ачаалж чадсангүй: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void dgvFlights_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _flights.Count)
            {
                // Сонгогдсон нислэгийн мэдээллийг авах
                var selectedFlight = _flights[e.RowIndex];
                
                // Талбаруудыг бөглөх
                txtFlightNumber.Text = selectedFlight.FlightNumber;
                txtDepartureCity.Text = selectedFlight.DepartureCity;
                txtArrivalCity.Text = selectedFlight.ArrivalCity;
                dtpDepartureTime.Value = selectedFlight.DepartureTime;
                dtpArrivalTime.Value = selectedFlight.ArrivalTime;
                
                // Суудлын тоог харуулах гэж оролдох
                // Хэрэв АРИ-д суудлын тоог авах арга байвал энд авна
                txtCapacity.Text = "";
            }
        }

        private string GetFlightStatusText(FlightStatus status)
        {
            switch (status)
            {
                case FlightStatus.CheckingIn:
                    return "Бүртгэж байна";
                case FlightStatus.Boarding:
                    return "Онгоцонд сууж байна";
                case FlightStatus.Departed:
                    return "Ниссэн";
                case FlightStatus.Delayed:
                    return "Хойшилсон";
                case FlightStatus.Cancelled:
                    return "Цуцалсан";
                default:
                    return "Үл мэдэгдэх";
            }
        }

        private void ClearFields()
        {
            txtFlightNumber.Text = string.Empty;
            txtDepartureCity.Text = string.Empty;
            txtArrivalCity.Text = string.Empty;
            dtpDepartureTime.Value = DateTime.Now;
            dtpArrivalTime.Value = DateTime.Now.AddHours(2);
            txtCapacity.Text = string.Empty;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtFlightNumber.Text))
            {
                MessageBox.Show("Нислэгийн дугаар оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtDepartureCity.Text))
            {
                MessageBox.Show("Хөдөлөх хот оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtArrivalCity.Text))
            {
                MessageBox.Show("Очих хот оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpDepartureTime.Value >= dtpArrivalTime.Value)
            {
                MessageBox.Show("Очих цаг нь хөдлөх цагаас хойш байх ёстой", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtCapacity.Text) || !int.TryParse(txtCapacity.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Суудлын тоо зөв оруулна уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }
}