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
    public class PassengerRegistrationForm : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;

        // UI элементүүд
        private Label lblTitle;
        private Label lblPassportNumber;
        private Label lblFirstName;
        private Label lblLastName;
        private Label lblEmail;
        private Label lblPhoneNumber;
        private Label lblNationality;
        
        private TextBox txtPassportNumber;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private TextBox txtPhoneNumber;
        private TextBox txtNationality;
        
        private Button btnSave;
        private Button btnCancel;
        private Button btnClear;
        
        private DataGridView dgvPassengers;
        
        public PassengerRegistrationForm()
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
                    var loadTask = LoadPassengersAsync();
                    loadTask.Wait(); // Task дуусахыг хүлээх (байгаа алдааг засахын тулд await хэрэглэхгүй)
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Зорчигчдын мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        
        private void InitializeComponents()
        {
            // Формын тохиргоо
            this.Text = "Зорчигч бүртгэх";
            this.Size = new Size(1500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            
            // Гарчиг
            lblTitle = new Label();
            lblTitle.Text = "ЗОРЧИГЧ БҮРТГЭХ";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);
            this.Controls.Add(lblTitle);
            
            // Паспорт
            lblPassportNumber = new Label();
            lblPassportNumber.Text = "Паспорт:";
            lblPassportNumber.AutoSize = true;
            lblPassportNumber.Location = new Point(20, 70);
            this.Controls.Add(lblPassportNumber);
            
            txtPassportNumber = new TextBox();
            txtPassportNumber.Size = new Size(200, 25);
            txtPassportNumber.Location = new Point(150, 70);
            this.Controls.Add(txtPassportNumber);
            
            // Нэр
            lblFirstName = new Label();
            lblFirstName.Text = "Нэр:";
            lblFirstName.AutoSize = true;
            lblFirstName.Location = new Point(20, 110);
            this.Controls.Add(lblFirstName);
            
            txtFirstName = new TextBox();
            txtFirstName.Size = new Size(200, 25);
            txtFirstName.Location = new Point(150, 110);
            this.Controls.Add(txtFirstName);
            
            // Овог
            lblLastName = new Label();
            lblLastName.Text = "Овог:";
            lblLastName.AutoSize = true;
            lblLastName.Location = new Point(20, 150);
            this.Controls.Add(lblLastName);
            
            txtLastName = new TextBox();
            txtLastName.Size = new Size(200, 25);
            txtLastName.Location = new Point(150, 150);
            this.Controls.Add(txtLastName);
            
            // Имэйл
            lblEmail = new Label();
            lblEmail.Text = "Имэйл:";
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(20, 190);
            this.Controls.Add(lblEmail);
            
            txtEmail = new TextBox();
            txtEmail.Size = new Size(200, 25);
            txtEmail.Location = new Point(150, 190);
            this.Controls.Add(txtEmail);
            
            // Утас
            lblPhoneNumber = new Label();
            lblPhoneNumber.Text = "Утас:";
            lblPhoneNumber.AutoSize = true;
            lblPhoneNumber.Location = new Point(20, 230);
            this.Controls.Add(lblPhoneNumber);
            
            txtPhoneNumber = new TextBox();
            txtPhoneNumber.Size = new Size(200, 25);
            txtPhoneNumber.Location = new Point(150, 230);
            this.Controls.Add(txtPhoneNumber);
            
            // Товчнууд
            btnSave = new Button();
            btnSave.Text = "Хадгалах";
            btnSave.Size = new Size(300, 35);
            btnSave.Location = new Point(50, 320);
            btnSave.BackColor = Color.SteelBlue;
            btnSave.ForeColor = Color.White;
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);
            
            btnClear = new Button();
            btnClear.Text = "Цэвэрлэх";
            btnClear.Size = new Size(300, 35);
            btnClear.Location = new Point(50, 370);
            btnClear.BackColor = Color.Gray;
            btnClear.ForeColor = Color.White;
            btnClear.Click += btnClear_Click;
            this.Controls.Add(btnClear);
            
            btnCancel = new Button();
            btnCancel.Text = "Хаах";
            btnCancel.Size = new Size(300, 35);
            btnCancel.Location = new Point(50, 420);
            btnCancel.BackColor = Color.IndianRed;
            btnCancel.ForeColor = Color.White;
            btnCancel.Click += btnCancel_Click;
            this.Controls.Add(btnCancel);
            
            // Зорчигчийн жагсаалт харуулах DataGridView
            dgvPassengers = new DataGridView();
            dgvPassengers.Location = new Point(450, 70);
            dgvPassengers.Size = new Size(900, 480);
            dgvPassengers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPassengers.AllowUserToAddRows = false;
            dgvPassengers.AllowUserToDeleteRows = false;
            dgvPassengers.ReadOnly = true;
            dgvPassengers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPassengers.MultiSelect = false;
            dgvPassengers.BackgroundColor = Color.White;
            dgvPassengers.CellDoubleClick += dgvPassengers_CellDoubleClick;
            this.Controls.Add(dgvPassengers);
        }
        
        private async Task LoadPassengersAsync()
        {
            try
            {
                // Зорчигчдын жагсаалт авах
                var passengers = await _apiService.GetAllPassengersAsync();
                
                dgvPassengers.DataSource = null;
                dgvPassengers.Columns.Clear();
                
                dgvPassengers.DataSource = passengers;
                
                // Харуулах багануудыг тохируулах
                if (dgvPassengers.Columns["Id"] != null)
                    dgvPassengers.Columns["Id"].Visible = false;
                
                if (dgvPassengers.Columns["CheckedIn"] != null)
                    dgvPassengers.Columns["CheckedIn"].HeaderText = "Бүртгүүлсэн";
                
                if (dgvPassengers.Columns["FirstName"] != null)
                    dgvPassengers.Columns["FirstName"].HeaderText = "Нэр";
                
                if (dgvPassengers.Columns["LastName"] != null)
                    dgvPassengers.Columns["LastName"].HeaderText = "Овог";
                
                if (dgvPassengers.Columns["PassportNumber"] != null)
                    dgvPassengers.Columns["PassportNumber"].HeaderText = "Паспортын дугаар";
                
                if (dgvPassengers.Columns["Nationality"] != null)
                    dgvPassengers.Columns["Nationality"].HeaderText = "Иргэншил";
                
                if (dgvPassengers.Columns["SeatNumber"] != null)
                    dgvPassengers.Columns["SeatNumber"].HeaderText = "Суудлын дугаар";
                
                if (dgvPassengers.Columns["Email"] != null)
                    dgvPassengers.Columns["Email"].HeaderText = "Имэйл";
                
                if (dgvPassengers.Columns["PhoneNumber"] != null)
                    dgvPassengers.Columns["PhoneNumber"].HeaderText = "Утасны дугаар";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигчдын жагсаалт авахад алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void dgvPassengers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var passenger = dgvPassengers.DataSource as List<PassengerDto>;
                if (passenger != null && passenger.Count > e.RowIndex)
                {
                    var selectedPassenger = passenger[e.RowIndex];
                    
                    // Зорчигчийн мэдээллийг оролтын талбарт нөхөх
                    txtPassportNumber.Text = selectedPassenger.PassportNumber;
                    txtFirstName.Text = selectedPassenger.FirstName;
                    txtLastName.Text = selectedPassenger.LastName;
                    txtEmail.Text = selectedPassenger.Email;
                    txtPhoneNumber.Text = selectedPassenger.PhoneNumber;
                    txtNationality.Text = selectedPassenger.Nationality;
                }
            }
        }
        
        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Талбаруудын хоосон эсэхийг шалгах
                if (string.IsNullOrWhiteSpace(txtPassportNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Паспортын дугаар, нэр, овог заавал оруулна уу!", "Анхааруулга", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Зорчигчийн мэдээлэл
                var passenger = new PassengerDto
                {
                    PassportNumber = txtPassportNumber.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Nationality = txtNationality.Text.Trim()
                };
                
                // Зорчигч бүртгэх
                bool success = await _apiService.RegisterPassengerAsync(passenger);
                
                if (success)
                {
                    MessageBox.Show("Зорчигч амжилттай бүртгэгдлээ.", "Амжилттай", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    // Task.Run ашиглах нь зөв боловч энд await хийх боломжтой болгохын тулд Task.Run хэрэглэхгүй
                    try {
                        await LoadPassengersAsync(); // Жагсаалтыг шинэчлэх
                    } catch (Exception ex) {
                        MessageBox.Show($"Жагсаалт шинэчлэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Зорчигч бүртгэхэд алдаа гарлаа.", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtPassportNumber.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtNationality.Text = string.Empty;
            
            txtPassportNumber.Focus();
        }
    }
}
