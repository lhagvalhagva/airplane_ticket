using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OrderForm.Models;
using OrderForm.Services;
using System.Configuration;
using System.Threading.Tasks;

namespace OrderForm.Pages
{
    public class FlightRegistrationForm : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;

        // UI элементүүд
        private Label lblTitle;
        private Label lblFlightNumber;
        private Label lblDepartureCity;
        private Label lblArrivalCity;
        private Label lblDepartureTime;
        private Label lblArrivalTime;
        private Label lblStatus;
        
        private TextBox txtFlightNumber;
        private TextBox txtDepartureCity;
        private TextBox txtArrivalCity;
        private ComboBox cmbStatus;
        private DateTimePicker dtpDepartureDate;
        private DateTimePicker dtpDepartureTime;
        private DateTimePicker dtpArrivalDate;
        private DateTimePicker dtpArrivalTime;
        
        private Button btnSave;
        private Button btnCancel;
        private Button btnClear;
        
        private DataGridView dgvFlights;
        
        public FlightRegistrationForm()
        {
            // API Service үүсгэх
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(_apiBaseUrl);
            
            InitializeComponents();
            // Асинхрон кодыг хэрэгжүүлэхдээ Task.Run ашиглах нь илүү зөв
            Task.Run(() => {
                try
                {
                    // async/await ашиглалгүйгээр хэрэгжүүлэх
                    var loadTask = LoadFlightsAsync();
                    loadTask.Wait(); // Task дуусахыг хүлээх
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Нислэгийн мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        
        private void InitializeComponents()
        {
            // Формын тохиргоо
            this.Text = "Нислэг бүртгэх";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            // Гарчиг
            lblTitle = new Label();
            lblTitle.Text = "НИСЛЭГ БҮРТГЭХ";
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
            
            // Хөдлөх хот
            lblDepartureCity = new Label();
            lblDepartureCity.Text = "Хөдлөх хот:";
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
            
            // Хөдлөх огноо цаг
            lblDepartureTime = new Label();
            lblDepartureTime.Text = "Хөдлөх огноо цаг:";
            lblDepartureTime.AutoSize = true;
            lblDepartureTime.Location = new Point(20, 190);
            this.Controls.Add(lblDepartureTime);
            
            dtpDepartureDate = new DateTimePicker();
            dtpDepartureDate.Format = DateTimePickerFormat.Short;
            dtpDepartureDate.Size = new Size(120, 25);
            dtpDepartureDate.Location = new Point(150, 190);
            this.Controls.Add(dtpDepartureDate);
            
            dtpDepartureTime = new DateTimePicker();
            dtpDepartureTime.Format = DateTimePickerFormat.Time;
            dtpDepartureTime.ShowUpDown = true;
            dtpDepartureTime.Size = new Size(80, 25);
            dtpDepartureTime.Location = new Point(280, 190);
            this.Controls.Add(dtpDepartureTime);
            
            // Ирэх огноо цаг
            lblArrivalTime = new Label();
            lblArrivalTime.Text = "Ирэх огноо цаг:";
            lblArrivalTime.AutoSize = true;
            lblArrivalTime.Location = new Point(20, 230);
            this.Controls.Add(lblArrivalTime);
            
            dtpArrivalDate = new DateTimePicker();
            dtpArrivalDate.Format = DateTimePickerFormat.Short;
            dtpArrivalDate.Size = new Size(120, 25);
            dtpArrivalDate.Location = new Point(150, 230);
            this.Controls.Add(dtpArrivalDate);
            
            dtpArrivalTime = new DateTimePicker();
            dtpArrivalTime.Format = DateTimePickerFormat.Time;
            dtpArrivalTime.ShowUpDown = true;
            dtpArrivalTime.Size = new Size(80, 25);
            dtpArrivalTime.Location = new Point(280, 230);
            this.Controls.Add(dtpArrivalTime);
            
            // Онгоцны мэдээлэл хассан
            
            // Нислэгийн төлөв
            lblStatus = new Label();
            lblStatus.Text = "Төлөв:";
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(20, 310);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Size = new Size(200, 25);
            cmbStatus.Location = new Point(150, 310);
            
            // Нислэгийн төлөвүүд
            foreach (FlightStatus status in Enum.GetValues(typeof(FlightStatus)))
            {
                cmbStatus.Items.Add(status);
            }
            
            cmbStatus.SelectedIndex = 0; // Default to "Scheduled"
            this.Controls.Add(cmbStatus);
            
            // Товчнууд
            btnSave = new Button();
            btnSave.Text = "Хадгалах";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(50, 360);
            btnSave.BackColor = Color.SteelBlue;
            btnSave.ForeColor = Color.White;
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);
            
            btnClear = new Button();
            btnClear.Text = "Цэвэрлэх";
            btnClear.Size = new Size(100, 35);
            btnClear.Location = new Point(160, 360);
            btnClear.BackColor = Color.Gray;
            btnClear.ForeColor = Color.White;
            btnClear.Click += btnClear_Click;
            this.Controls.Add(btnClear);
            
            btnCancel = new Button();
            btnCancel.Text = "Хаах";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(270, 360);
            btnCancel.BackColor = Color.IndianRed;
            btnCancel.ForeColor = Color.White;
            btnCancel.Click += btnCancel_Click;
            this.Controls.Add(btnCancel);
            
            // Нислэгийн жагсаалт харуулах DataGridView
            dgvFlights = new DataGridView();
            dgvFlights.Location = new Point(380, 70);
            dgvFlights.Size = new Size(700, 520);
            dgvFlights.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFlights.AllowUserToAddRows = false;
            dgvFlights.AllowUserToDeleteRows = false;
            dgvFlights.ReadOnly = true;
            dgvFlights.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFlights.MultiSelect = false;
            dgvFlights.BackgroundColor = Color.White;
            dgvFlights.CellDoubleClick += dgvFlights_CellDoubleClick;
            this.Controls.Add(dgvFlights);
        }
        
        private async Task LoadFlightsAsync()
        {
            try
            {
                // Нислэгийн жагсаалт авах
                var flights = await _apiService.GetAllFlightsAsync();
                
                dgvFlights.DataSource = null;
                dgvFlights.Columns.Clear();
                
                dgvFlights.DataSource = flights;
                
                // Харуулах багануудыг тохируулах
                if (dgvFlights.Columns["Id"] != null)
                    dgvFlights.Columns["Id"].Visible = false;
                
                if (dgvFlights.Columns["FlightNumber"] != null)
                    dgvFlights.Columns["FlightNumber"].HeaderText = "Нислэгийн дугаар";
                
                if (dgvFlights.Columns["DepartureCity"] != null)
                    dgvFlights.Columns["DepartureCity"].HeaderText = "Хөдлөх хот";
                
                if (dgvFlights.Columns["ArrivalCity"] != null)
                    dgvFlights.Columns["ArrivalCity"].HeaderText = "Очих хот";
                
                if (dgvFlights.Columns["DepartureTime"] != null)
                    dgvFlights.Columns["DepartureTime"].HeaderText = "Хөдлөх цаг";
                
                if (dgvFlights.Columns["ArrivalTime"] != null)
                    dgvFlights.Columns["ArrivalTime"].HeaderText = "Ирэх цаг";
                
                // Aircraft багана хассан
                
                if (dgvFlights.Columns["Status"] != null)
                    dgvFlights.Columns["Status"].HeaderText = "Төлөв";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэгийн жагсаалт авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void dgvFlights_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var flights = dgvFlights.DataSource as List<FlightDto>;
                if (flights != null && flights.Count > e.RowIndex)
                {
                    var selectedFlight = flights[e.RowIndex];
                    
                    // Нислэгийн мэдээллийг оролтын талбарт нөхөх
                    txtFlightNumber.Text = selectedFlight.FlightNumber;
                    txtDepartureCity.Text = selectedFlight.DepartureCity;
                    txtArrivalCity.Text = selectedFlight.ArrivalCity;
                    cmbStatus.SelectedItem = selectedFlight.Status;
                    
                    dtpDepartureDate.Value = selectedFlight.DepartureTime.Date;
                    dtpDepartureTime.Value = selectedFlight.DepartureTime;
                    
                    dtpArrivalDate.Value = selectedFlight.ArrivalTime.Date;
                    dtpArrivalTime.Value = selectedFlight.ArrivalTime;
                }
            }
        }
        
        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Талбаруудын хоосон эсэхийг шалгах
                if (string.IsNullOrWhiteSpace(txtFlightNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtDepartureCity.Text) ||
                    string.IsNullOrWhiteSpace(txtArrivalCity.Text))
                {
                    MessageBox.Show("Нислэгийн дугаар, хөдлөх болон очих цэг заавал оруулна уу!", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Нислэгийн хөдлөх огноо цаг нэгтгэх
                DateTime departureDateTime = dtpDepartureDate.Value.Date.Add(dtpDepartureTime.Value.TimeOfDay);
                
                // Нислэгийн ирэх огноо цаг нэгтгэх
                DateTime arrivalDateTime = dtpArrivalDate.Value.Date.Add(dtpArrivalTime.Value.TimeOfDay);
                
                // Огноо цагийн хяналт
                if (departureDateTime >= arrivalDateTime)
                {
                    MessageBox.Show("Нислэгийн ирэх цаг нь хөдлөх цагаас хойш байх ёстой!", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Нислэгийн мэдээлэл
                var flight = new FlightDto
                {
                    FlightNumber = txtFlightNumber.Text.Trim(),
                    DepartureCity = txtDepartureCity.Text.Trim(),
                    ArrivalCity = txtArrivalCity.Text.Trim(),
                    DepartureTime = departureDateTime,
                    ArrivalTime = arrivalDateTime,
                    Status = (FlightStatus)cmbStatus.SelectedItem
                };
                
                // Нислэг бүртгэх
                bool success = await _apiService.RegisterFlightAsync(flight);
                
                if (success)
                {
                    MessageBox.Show("Нислэг амжилттай бүртгэгдлээ.", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    
                    // Жагсаалтыг шинэчлэх
                    try {
                        await LoadFlightsAsync();
                    } catch (Exception ex) {
                        MessageBox.Show($"Нислэгийн жагсаалт шинэчлэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Нислэг бүртгэхэд алдаа гарлаа.", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нислэг бүртгэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        
        private void ClearFields()
        {
            txtFlightNumber.Text = string.Empty;
            txtDepartureCity.Text = string.Empty;
            txtArrivalCity.Text = string.Empty;
            cmbStatus.SelectedIndex = 0;
            
            // Огноо цагийг одоо байгаагаар тохируулах
            dtpDepartureDate.Value = DateTime.Now;
            dtpDepartureTime.Value = DateTime.Now;
            dtpArrivalDate.Value = DateTime.Now;
            dtpArrivalTime.Value = DateTime.Now.AddHours(2);
            
            txtFlightNumber.Focus();
        }
    }
}
