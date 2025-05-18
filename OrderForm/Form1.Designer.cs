namespace OrderForm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainMenu = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem();
            passengerRegistrationMenuItem = new ToolStripMenuItem();
            flightRegistrationMenuItem = new ToolStripMenuItem();
            exitMenuItem = new ToolStripMenuItem();
            lblFlight = new Label();
            cmbFlights = new ComboBox();
            lblFlightInfo = new Label();
            lblSearch = new Label();
            txtPassportSearch = new TextBox();
            btnSearch = new Button();
            txtPassengerInfo = new TextBox();
            lblSeat = new Label();
            txtSeatNumber = new TextBox();
            dgvPassengers = new DataGridView();
            PassengerId = new DataGridViewTextBoxColumn();
            FirstName = new DataGridViewTextBoxColumn();
            LastName = new DataGridViewTextBoxColumn();
            PassportNumber = new DataGridViewTextBoxColumn();
            SeatNumber = new DataGridViewTextBoxColumn();
            lblPassengerCount = new Label();
            btnCheckIn = new Button();
            btnChangeStatus = new Button();
            lblStatus = new Label();
            mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPassengers).BeginInit();
            SuspendLayout();
            // 
            // mainMenu
            // 
            mainMenu.ImageScalingSize = new Size(32, 32);
            mainMenu.Items.AddRange(new ToolStripItem[] { fileMenuItem });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Padding = new Padding(13, 5, 0, 5);
            mainMenu.Size = new Size(2564, 46);
            mainMenu.TabIndex = 0;
            mainMenu.Text = "mainMenu";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { passengerRegistrationMenuItem, flightRegistrationMenuItem, exitMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(153, 36);
            fileMenuItem.Text = "Үндсэн цэс";
            // 
            // passengerRegistrationMenuItem
            // 
            passengerRegistrationMenuItem.Name = "passengerRegistrationMenuItem";
            passengerRegistrationMenuItem.Size = new Size(359, 44);
            passengerRegistrationMenuItem.Text = "Зорчигч бүртгэх";
            passengerRegistrationMenuItem.Click += passengerRegistrationMenuItem_Click;
            // 
            // flightRegistrationMenuItem
            // 
            flightRegistrationMenuItem.Name = "flightRegistrationMenuItem";
            flightRegistrationMenuItem.Size = new Size(359, 44);
            flightRegistrationMenuItem.Text = "Нислэг бүртгэх";
            flightRegistrationMenuItem.Click += flightRegistrationMenuItem_Click;
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(359, 44);
            exitMenuItem.Text = "Гарах";
            exitMenuItem.Click += exitMenuItem_Click;
            // 
            // lblFlight
            // 
            lblFlight.AutoSize = true;
            lblFlight.Font = new Font("Arial", 10F, FontStyle.Bold);
            lblFlight.Location = new Point(26, 98);
            lblFlight.Margin = new Padding(6, 0, 6, 0);
            lblFlight.Name = "lblFlight";
            lblFlight.Size = new Size(217, 32);
            lblFlight.TabIndex = 1;
            lblFlight.Text = "Нислэг сонгох:";
            // 
            // cmbFlights
            // 
            cmbFlights.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFlights.Font = new Font("Arial", 10F);
            cmbFlights.FormattingEnabled = true;
            cmbFlights.Location = new Point(299, 91);
            cmbFlights.Margin = new Padding(6, 7, 6, 7);
            cmbFlights.Name = "cmbFlights";
            cmbFlights.Size = new Size(970, 40);
            cmbFlights.TabIndex = 2;
            cmbFlights.SelectedIndexChanged += cmbFlights_SelectedIndexChanged;
            // 
            // lblFlightInfo
            // 
            lblFlightInfo.AutoSize = true;
            lblFlightInfo.Font = new Font("Arial", 10F, FontStyle.Bold);
            lblFlightInfo.ForeColor = Color.Navy;
            lblFlightInfo.Location = new Point(299, 172);
            lblFlightInfo.Margin = new Padding(6, 0, 6, 0);
            lblFlightInfo.Name = "lblFlightInfo";
            lblFlightInfo.Size = new Size(0, 32);
            lblFlightInfo.TabIndex = 4;
            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.Font = new Font("Arial", 10F, FontStyle.Bold);
            lblSearch.Location = new Point(1329, 95);
            lblSearch.Margin = new Padding(6, 0, 6, 0);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(205, 32);
            lblSearch.TabIndex = 5;
            lblSearch.Text = "Паспорт хайх:";
            // 
            // txtPassportSearch
            // 
            txtPassportSearch.Font = new Font("Arial", 10F);
            txtPassportSearch.Location = new Point(1534, 89);
            txtPassportSearch.Margin = new Padding(6, 7, 6, 7);
            txtPassportSearch.Name = "txtPassportSearch";
            txtPassportSearch.Size = new Size(429, 38);
            txtPassportSearch.TabIndex = 6;
            // 
            // btnSearch
            // 
            btnSearch.Font = new Font("Arial", 9F);
            btnSearch.Location = new Point(2015, 87);
            btnSearch.Margin = new Padding(6, 7, 6, 7);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(217, 57);
            btnSearch.TabIndex = 7;
            btnSearch.Text = "Хайх";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtPassengerInfo
            // 
            txtPassengerInfo.BackColor = SystemColors.Info;
            txtPassengerInfo.Font = new Font("Arial", 10F);
            txtPassengerInfo.Location = new Point(1329, 134);
            txtPassengerInfo.Margin = new Padding(6, 7, 6, 7);
            txtPassengerInfo.Name = "txtPassengerInfo";
            txtPassengerInfo.ReadOnly = true;
            txtPassengerInfo.Size = new Size(667, 38);
            txtPassengerInfo.TabIndex = 8;
            // 
            // lblSeat
            // 
            lblSeat.AutoSize = true;
            lblSeat.Font = new Font("Arial", 10F, FontStyle.Bold);
            lblSeat.Location = new Point(1343, 1502);
            lblSeat.Margin = new Padding(6, 0, 6, 0);
            lblSeat.Name = "lblSeat";
            lblSeat.Size = new Size(254, 32);
            lblSeat.TabIndex = 9;
            lblSeat.Text = "Сонгосон суудал:";
            // 
            // txtSeatNumber
            // 
            txtSeatNumber.Font = new Font("Arial", 10F, FontStyle.Bold);
            txtSeatNumber.Location = new Point(1647, 1494);
            txtSeatNumber.Margin = new Padding(6, 7, 6, 7);
            txtSeatNumber.Name = "txtSeatNumber";
            txtSeatNumber.Size = new Size(212, 38);
            txtSeatNumber.TabIndex = 10;
            // 
            // dgvPassengers
            // 
            dgvPassengers.AllowUserToAddRows = false;
            dgvPassengers.AllowUserToDeleteRows = false;
            dgvPassengers.BackgroundColor = SystemColors.Control;
            dgvPassengers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPassengers.Columns.AddRange(new DataGridViewColumn[] { PassengerId, FirstName, LastName, PassportNumber, SeatNumber });
            dgvPassengers.Location = new Point(1343, 207);
            dgvPassengers.Margin = new Padding(6, 7, 6, 7);
            dgvPassengers.MultiSelect = false;
            dgvPassengers.Name = "dgvPassengers";
            dgvPassengers.ReadOnly = true;
            dgvPassengers.RowHeadersWidth = 82;
            dgvPassengers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPassengers.Size = new Size(641, 1231);
            dgvPassengers.TabIndex = 11;
            dgvPassengers.CellClick += dgvPassengers_CellClick;
            // 
            // PassengerId
            // 
            PassengerId.HeaderText = "ID";
            PassengerId.MinimumWidth = 10;
            PassengerId.Name = "PassengerId";
            PassengerId.ReadOnly = true;
            PassengerId.Visible = false;
            PassengerId.Width = 50;
            // 
            // FirstName
            // 
            FirstName.HeaderText = "Нэр";
            FirstName.MinimumWidth = 10;
            FirstName.Name = "FirstName";
            FirstName.ReadOnly = true;
            FirstName.Width = 135;
            // 
            // LastName
            // 
            LastName.HeaderText = "Овог";
            LastName.MinimumWidth = 10;
            LastName.Name = "LastName";
            LastName.ReadOnly = true;
            LastName.Width = 135;
            // 
            // PassportNumber
            // 
            PassportNumber.HeaderText = "Паспортын дугаар";
            PassportNumber.MinimumWidth = 10;
            PassportNumber.Name = "PassportNumber";
            PassportNumber.ReadOnly = true;
            PassportNumber.Width = 155;
            // 
            // SeatNumber
            // 
            SeatNumber.HeaderText = "Суудал";
            SeatNumber.MinimumWidth = 10;
            SeatNumber.Name = "SeatNumber";
            SeatNumber.ReadOnly = true;
            SeatNumber.Width = 135;
            // 
            // lblPassengerCount
            // 
            lblPassengerCount.AutoSize = true;
            lblPassengerCount.Font = new Font("Arial", 9F);
            lblPassengerCount.Location = new Point(26, 1502);
            lblPassengerCount.Margin = new Padding(6, 0, 6, 0);
            lblPassengerCount.Name = "lblPassengerCount";
            lblPassengerCount.Size = new Size(0, 27);
            lblPassengerCount.TabIndex = 12;
            // 
            // btnCheckIn
            // 
            btnCheckIn.BackColor = Color.RoyalBlue;
            btnCheckIn.Font = new Font("Arial", 10F, FontStyle.Bold);
            btnCheckIn.ForeColor = Color.White;
            btnCheckIn.Location = new Point(1907, 1489);
            btnCheckIn.Margin = new Padding(6, 7, 6, 7);
            btnCheckIn.Name = "btnCheckIn";
            btnCheckIn.Size = new Size(325, 74);
            btnCheckIn.TabIndex = 13;
            btnCheckIn.Text = "Бүртгэх";
            btnCheckIn.UseVisualStyleBackColor = false;
            btnCheckIn.Click += btnCheckIn_Click;
            // 
            // btnChangeStatus
            // 
            btnChangeStatus.Font = new Font("Arial", 9F);
            btnChangeStatus.Location = new Point(26, 145);
            btnChangeStatus.Margin = new Padding(6, 7, 6, 7);
            btnChangeStatus.Name = "btnChangeStatus";
            btnChangeStatus.Size = new Size(261, 57);
            btnChangeStatus.TabIndex = 14;
            btnChangeStatus.Text = "Нислэгийн төлөв өөрчлөх";
            btnChangeStatus.UseVisualStyleBackColor = true;
            btnChangeStatus.Click += btnChangeStatus_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Arial", 9F);
            lblStatus.Location = new Point(26, 1575);
            lblStatus.Margin = new Padding(6, 0, 6, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 27);
            lblStatus.TabIndex = 16;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2564, 1559);
            Controls.Add(lblStatus);
            Controls.Add(btnChangeStatus);
            Controls.Add(btnCheckIn);
            Controls.Add(lblPassengerCount);
            Controls.Add(dgvPassengers);
            Controls.Add(txtSeatNumber);
            Controls.Add(lblSeat);
            Controls.Add(txtPassengerInfo);
            Controls.Add(btnSearch);
            Controls.Add(txtPassportSearch);
            Controls.Add(lblSearch);
            Controls.Add(lblFlightInfo);
            Controls.Add(cmbFlights);
            Controls.Add(lblFlight);
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            Margin = new Padding(6, 7, 6, 7);
            Name = "Form1";
            Text = "Нислэгийн бүртгэлийн систем";
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPassengers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem passengerRegistrationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flightRegistrationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.Label lblFlight;
        private System.Windows.Forms.ComboBox cmbFlights;
        private System.Windows.Forms.Label lblFlightInfo;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtPassportSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtPassengerInfo;
        private System.Windows.Forms.Label lblSeat;
        private System.Windows.Forms.TextBox txtSeatNumber;
        private System.Windows.Forms.DataGridView dgvPassengers;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassengerId;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassportNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn SeatNumber;
        private System.Windows.Forms.Label lblPassengerCount;
        private System.Windows.Forms.Button btnCheckIn;
        private System.Windows.Forms.Button btnChangeStatus;
        private System.Windows.Forms.Label lblStatus;
    }
}