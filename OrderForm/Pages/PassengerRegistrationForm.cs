using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using OrderForm.Models;
using OrderForm.Services;
using System.Configuration;

namespace OrderForm.Pages
{
    public class PassengerRegistrationForm : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;

        // UI элементийн талбарууд
        private Label lblTitle, lblPassportNumber, lblFirstName, lblLastName, lblEmail, lblPhoneNumber, lblNationality;
        private TextBox txtPassportNumber, txtFirstName, txtLastName, txtEmail, txtPhoneNumber, txtNationality;
        private Button btnSave, btnCancel, btnClear;
        private DataGridView dgvPassengers;

        public PassengerRegistrationForm()
        {
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(_apiBaseUrl);

            InitializeComponents();

            // Form Load үед зорчигчдын жагсаалт ачаалж харуулна
            this.Load += async (s, e) =>
            {
                try
                {
                    await LoadPassengersAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Зорчигчдын мэдээлэл авахад алдаа гарлаа: {ex.Message}", "Алдаа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void InitializeComponents()
        {
            // Form тохиргоо
            this.Text = "Зорчигч бүртгэх";
            this.Size = new Size(1500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Гарчиг
            lblTitle = new Label
            {
                Text = "ЗОРЧИГЧ БҮРТГЭХ",
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblTitle);

            // Паспорт
            lblPassportNumber = new Label { Text = "Паспорт:", AutoSize = true, Location = new Point(20, 70) };
            this.Controls.Add(lblPassportNumber);
            txtPassportNumber = new TextBox { Size = new Size(200, 25), Location = new Point(150, 70) };
            this.Controls.Add(txtPassportNumber);

            // Нэр
            lblFirstName = new Label { Text = "Нэр:", AutoSize = true, Location = new Point(20, 110) };
            this.Controls.Add(lblFirstName);
            txtFirstName = new TextBox { Size = new Size(200, 25), Location = new Point(150, 110) };
            this.Controls.Add(txtFirstName);

            // Овог
            lblLastName = new Label { Text = "Овог:", AutoSize = true, Location = new Point(20, 150) };
            this.Controls.Add(lblLastName);
            txtLastName = new TextBox { Size = new Size(200, 25), Location = new Point(150, 150) };
            this.Controls.Add(txtLastName);

            // Имэйл
            lblEmail = new Label { Text = "Имэйл:", AutoSize = true, Location = new Point(20, 190) };
            this.Controls.Add(lblEmail);
            txtEmail = new TextBox { Size = new Size(200, 25), Location = new Point(150, 190) };
            this.Controls.Add(txtEmail);

            // Утас
            lblPhoneNumber = new Label { Text = "Утас:", AutoSize = true, Location = new Point(20, 230) };
            this.Controls.Add(lblPhoneNumber);
            txtPhoneNumber = new TextBox { Size = new Size(200, 25), Location = new Point(150, 230) };
            this.Controls.Add(txtPhoneNumber);

            // Иргэншил
            lblNationality = new Label { Text = "Иргэншил:", AutoSize = true, Location = new Point(20, 270) };
            this.Controls.Add(lblNationality);
            txtNationality = new TextBox { Size = new Size(200, 25), Location = new Point(150, 270) };
            this.Controls.Add(txtNationality);

            // Товчнууд
            btnSave = new Button
            {
                Text = "Хадгалах",
                Size = new Size(300, 35),
                Location = new Point(50, 340),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnClear = new Button
            {
                Text = "Цэвэрлэх",
                Size = new Size(300, 35),
                Location = new Point(50, 390),
                BackColor = Color.Gray,
                ForeColor = Color.White
            };
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);

            btnCancel = new Button
            {
                Text = "Хаах",
                Size = new Size(300, 35),
                Location = new Point(50, 440),
                BackColor = Color.IndianRed,
                ForeColor = Color.White
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // Зорчигчдын жагсаалт харуулах DataGridView
            dgvPassengers = new DataGridView
            {
                Location = new Point(450, 70),
                Size = new Size(900, 480),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White
            };
            dgvPassengers.CellDoubleClick += DgvPassengers_CellDoubleClick;
            this.Controls.Add(dgvPassengers);
        }

        /// <summary>
        /// Зорчигчдын жагсаалт ачаалах
        /// </summary>
        private async Task LoadPassengersAsync()
        {
            try
            {
                var passengers = await _apiService.GetAllPassengersAsync();

                dgvPassengers.DataSource = null;
                dgvPassengers.Columns.Clear();
                dgvPassengers.DataSource = passengers;

                // Баганын гарчгийг тохируулах
                if (dgvPassengers.Columns["Id"] != null)
                    dgvPassengers.Columns["Id"].Visible = false;
                if (dgvPassengers.Columns["FirstName"] != null)
                    dgvPassengers.Columns["FirstName"].HeaderText = "Нэр";
                if (dgvPassengers.Columns["LastName"] != null)
                    dgvPassengers.Columns["LastName"].HeaderText = "Овог";
                if (dgvPassengers.Columns["PassportNumber"] != null)
                    dgvPassengers.Columns["PassportNumber"].HeaderText = "Паспортын дугаар";
                if (dgvPassengers.Columns["Nationality"] != null)
                    dgvPassengers.Columns["Nationality"].HeaderText = "Иргэншил";
                if (dgvPassengers.Columns["Email"] != null)
                    dgvPassengers.Columns["Email"].HeaderText = "Имэйл";
                if (dgvPassengers.Columns["PhoneNumber"] != null)
                    dgvPassengers.Columns["PhoneNumber"].HeaderText = "Утас";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигчдын жагсаалт авахад алдаа гарлаа: {ex.Message}", "Алдаа",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPassengers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var passengers = dgvPassengers.DataSource as List<PassengerDto>;
            if (passengers != null && passengers.Count > e.RowIndex)
            {
                var selectedPassenger = passengers[e.RowIndex];
                txtPassportNumber.Text = selectedPassenger.PassportNumber;
                txtFirstName.Text = selectedPassenger.FirstName;
                txtLastName.Text = selectedPassenger.LastName;
                txtEmail.Text = selectedPassenger.Email;
                txtPhoneNumber.Text = selectedPassenger.PhoneNumber;
                txtNationality.Text = selectedPassenger.Nationality;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPassportNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Паспорт, нэр, овог заавал оруулна уу!", "Анхааруулга",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var passenger = new PassengerDto
                {
                    PassportNumber = txtPassportNumber.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhoneNumber.Text.Trim(),
                    Nationality = txtNationality.Text.Trim()
                };

                bool success = await _apiService.RegisterPassengerAsync(passenger);
                if (success)
                {
                    MessageBox.Show("Зорчигч амжилттай бүртгэгдлээ.", "Амжилттай",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    await LoadPassengersAsync();
                }
                else
                {
                    MessageBox.Show("Зорчигч бүртгэхэд алдаа гарлаа.", "Алдаа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Зорчигч бүртгэхэд алдаа гарлаа: {ex.Message}", "Алдаа",
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

        private void ClearFields()
        {
            txtPassportNumber.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();
            txtNationality.Clear();
            txtPassportNumber.Focus();
        }
    }
}
