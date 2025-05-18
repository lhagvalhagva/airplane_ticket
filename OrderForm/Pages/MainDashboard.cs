using System;
using System.Drawing;
using System.Windows.Forms;
using OrderForm.Services;
using System.Configuration;

namespace OrderForm.Pages
{
    public class MainDashboard : Form
    {
        private readonly ApiService _apiService;
        private readonly string _apiBaseUrl;
        
        // UI Элементүүд
        private Label lblTitle;
        private Button btnPassengerRegistration;
        private Button btnFlightRegistration;
        private Button btnCheckIn;
        private Button btnExit;
        
        public MainDashboard()
        {
            // API Service үүсгэх
            _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:5027/api";
            _apiService = new ApiService(_apiBaseUrl);
            
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            // Формын тохиргоо
            this.Text = "Нислэгийн Систем - Үндсэн Цэс";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            
            // Гарчиг
            lblTitle = new Label();
            lblTitle.Text = "НИСЛЭГИЙН ЗАХИАЛГА СИСТЕМ";
            lblTitle.Font = new Font("Arial", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.SteelBlue;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(170, 40);
            this.Controls.Add(lblTitle);
            
            // Зорчигч бүртгэх товч
            btnPassengerRegistration = new Button();
            btnPassengerRegistration.Text = "Зорчигч Бүртгэх";
            btnPassengerRegistration.Font = new Font("Arial", 14, FontStyle.Bold);
            btnPassengerRegistration.Size = new Size(300, 80);
            btnPassengerRegistration.Location = new Point(250, 120);
            btnPassengerRegistration.BackColor = Color.SteelBlue;
            btnPassengerRegistration.ForeColor = Color.White;
            btnPassengerRegistration.FlatStyle = FlatStyle.Flat;
            btnPassengerRegistration.FlatAppearance.BorderSize = 0;
            btnPassengerRegistration.Click += btnPassengerRegistration_Click;
            this.Controls.Add(btnPassengerRegistration);
            
            // Нислэг бүртгэх товч
            btnFlightRegistration = new Button();
            btnFlightRegistration.Text = "Нислэг Бүртгэх";
            btnFlightRegistration.Font = new Font("Arial", 14, FontStyle.Bold);
            btnFlightRegistration.Size = new Size(300, 80);
            btnFlightRegistration.Location = new Point(250, 220);
            btnFlightRegistration.BackColor = Color.SteelBlue;
            btnFlightRegistration.ForeColor = Color.White;
            btnFlightRegistration.FlatStyle = FlatStyle.Flat;
            btnFlightRegistration.FlatAppearance.BorderSize = 0;
            btnFlightRegistration.Click += btnFlightRegistration_Click;
            this.Controls.Add(btnFlightRegistration);
            
            // Check-in хийх товч
            btnCheckIn = new Button();
            btnCheckIn.Text = "Check-in хийх";
            btnCheckIn.Font = new Font("Arial", 14, FontStyle.Bold);
            btnCheckIn.Size = new Size(300, 80);
            btnCheckIn.Location = new Point(250, 320);
            btnCheckIn.BackColor = Color.SteelBlue;
            btnCheckIn.ForeColor = Color.White;
            btnCheckIn.FlatStyle = FlatStyle.Flat;
            btnCheckIn.FlatAppearance.BorderSize = 0;
            btnCheckIn.Click += btnCheckIn_Click;
            this.Controls.Add(btnCheckIn);
            
            // Гарах товч
            btnExit = new Button();
            btnExit.Text = "Гарах";
            btnExit.Font = new Font("Arial", 14, FontStyle.Bold);
            btnExit.Size = new Size(300, 80);
            btnExit.Location = new Point(250, 420);
            btnExit.BackColor = Color.IndianRed;
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += btnExit_Click;
            this.Controls.Add(btnExit);
        }
        
        private void btnPassengerRegistration_Click(object sender, EventArgs e)
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
                MessageBox.Show($"Зорчигч бүртгэх хуудас нээхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnFlightRegistration_Click(object sender, EventArgs e)
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
                MessageBox.Show($"Нислэг бүртгэх хуудас нээхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new CheckInForm())
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Check-in хийх хуудас нээхэд алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Та системээс гарахдаа итгэлтэй байна уу?", "Гарах", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
