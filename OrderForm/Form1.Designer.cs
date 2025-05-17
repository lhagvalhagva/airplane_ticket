namespace OrderForm
{
    partial class CheckInForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFlightSelection = new System.Windows.Forms.Label();
            this.cmbFlights = new System.Windows.Forms.ComboBox();
            this.lblPassportNumber = new System.Windows.Forms.Label();
            this.txtPassportNumber = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblPassengerInfo = new System.Windows.Forms.Label();
            this.txtPassengerName = new System.Windows.Forms.TextBox();
            this.lblSeatSelection = new System.Windows.Forms.Label();
            this.cmbSeats = new System.Windows.Forms.ComboBox();
            this.btnCheckIn = new System.Windows.Forms.Button();
            this.lblFlightStatus = new System.Windows.Forms.Label();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.lblAvailableSeats = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFlightSelection
            // 
            this.lblFlightSelection.AutoSize = true;
            this.lblFlightSelection.Location = new System.Drawing.Point(15, 23);
            this.lblFlightSelection.Name = "lblFlightSelection";
            this.lblFlightSelection.Size = new System.Drawing.Size(104, 15);
            this.lblFlightSelection.TabIndex = 0;
            this.lblFlightSelection.Text = "Нислэг сонгох:";
            // 
            // cmbFlights
            // 
            this.cmbFlights.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFlights.FormattingEnabled = true;
            this.cmbFlights.Location = new System.Drawing.Point(125, 20);
            this.cmbFlights.Name = "cmbFlights";
            this.cmbFlights.Size = new System.Drawing.Size(312, 23);
            this.cmbFlights.TabIndex = 1;
            this.cmbFlights.SelectedIndexChanged += new System.EventHandler(this.cmbFlights_SelectedIndexChanged);
            // 
            // lblPassportNumber
            // 
            this.lblPassportNumber.AutoSize = true;
            this.lblPassportNumber.Location = new System.Drawing.Point(15, 30);
            this.lblPassportNumber.Name = "lblPassportNumber";
            this.lblPassportNumber.Size = new System.Drawing.Size(111, 15);
            this.lblPassportNumber.TabIndex = 2;
            this.lblPassportNumber.Text = "Паспортын дугаар:";
            // 
            // txtPassportNumber
            // 
            this.txtPassportNumber.Location = new System.Drawing.Point(132, 27);
            this.txtPassportNumber.Name = "txtPassportNumber";
            this.txtPassportNumber.Size = new System.Drawing.Size(211, 23);
            this.txtPassportNumber.TabIndex = 3;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(345, 26);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 25);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Хайх";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblPassengerInfo
            // 
            this.lblPassengerInfo.AutoSize = true;
            this.lblPassengerInfo.Location = new System.Drawing.Point(15, 65);
            this.lblPassengerInfo.Name = "lblPassengerInfo";
            this.lblPassengerInfo.Size = new System.Drawing.Size(108, 15);
            this.lblPassengerInfo.TabIndex = 5;
            this.lblPassengerInfo.Text = "Зорчигчийн нэр:";
            // 
            // txtPassengerName
            // 
            this.txtPassengerName.Location = new System.Drawing.Point(132, 62);
            this.txtPassengerName.Name = "txtPassengerName";
            this.txtPassengerName.ReadOnly = true;
            this.txtPassengerName.Size = new System.Drawing.Size(292, 23);
            this.txtPassengerName.TabIndex = 6;
            // 
            // lblSeatSelection
            // 
            this.lblSeatSelection.AutoSize = true;
            this.lblSeatSelection.Location = new System.Drawing.Point(15, 100);
            this.lblSeatSelection.Name = "lblSeatSelection";
            this.lblSeatSelection.Size = new System.Drawing.Size(102, 15);
            this.lblSeatSelection.TabIndex = 7;
            this.lblSeatSelection.Text = "Суудал сонгох:";
            // 
            // cmbSeats
            // 
            this.cmbSeats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeats.FormattingEnabled = true;
            this.cmbSeats.Location = new System.Drawing.Point(132, 97);
            this.cmbSeats.Name = "cmbSeats";
            this.cmbSeats.Size = new System.Drawing.Size(121, 23);
            this.cmbSeats.TabIndex = 8;
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(153)))), ((int)(((byte)(76)))));
            this.btnCheckIn.Enabled = false;
            this.btnCheckIn.FlatAppearance.BorderSize = 0;
            this.btnCheckIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckIn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCheckIn.ForeColor = System.Drawing.Color.White;
            this.btnCheckIn.Location = new System.Drawing.Point(186, 139);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(120, 32);
            this.btnCheckIn.TabIndex = 9;
            this.btnCheckIn.Text = "Бүртгэх";
            this.btnCheckIn.UseVisualStyleBackColor = false;
            this.btnCheckIn.Click += new System.EventHandler(this.btnCheckIn_Click);
            // 
            // lblFlightStatus
            // 
            this.lblFlightStatus.AutoSize = true;
            this.lblFlightStatus.Location = new System.Drawing.Point(15, 55);
            this.lblFlightStatus.Name = "lblFlightStatus";
            this.lblFlightStatus.Size = new System.Drawing.Size(104, 15);
            this.lblFlightStatus.TabIndex = 10;
            this.lblFlightStatus.Text = "Нислэгийн төлөв:";
            // 
            // btnChangeStatus
            //  
            this.btnChangeStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.btnChangeStatus.FlatAppearance.BorderSize = 0;
            this.btnChangeStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeStatus.ForeColor = System.Drawing.Color.White;
            this.btnChangeStatus.Location = new System.Drawing.Point(247, 51);
            this.btnChangeStatus.Name = "btnChangeStatus";
            this.btnChangeStatus.Size = new System.Drawing.Size(173, 23);
            this.btnChangeStatus.TabIndex = 11;
            this.btnChangeStatus.Text = "Төлөв өөрчлөх";
            this.btnChangeStatus.UseVisualStyleBackColor = false;
            this.btnChangeStatus.Click += new System.EventHandler(this.btnChangeStatus_Click);
            // 
            // lblAvailableSeats
            // 
            this.lblAvailableSeats.AutoSize = true;
            this.lblAvailableSeats.Location = new System.Drawing.Point(259, 100);
            this.lblAvailableSeats.Name = "lblAvailableSeats";
            this.lblAvailableSeats.Size = new System.Drawing.Size(116, 15);
            this.lblAvailableSeats.TabIndex = 12;
            this.lblAvailableSeats.Text = "Боломжит суудал: 0";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblFlightSelection);
            this.groupBox1.Controls.Add(this.cmbFlights);
            this.groupBox1.Controls.Add(this.btnChangeStatus);
            this.groupBox1.Controls.Add(this.lblFlightStatus);
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 85);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Нислэгийн мэдээлэл";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblPassportNumber);
            this.groupBox2.Controls.Add(this.txtPassportNumber);
            this.groupBox2.Controls.Add(this.lblAvailableSeats);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Controls.Add(this.lblPassengerInfo);
            this.groupBox2.Controls.Add(this.btnCheckIn);
            this.groupBox2.Controls.Add(this.txtPassengerName);
            this.groupBox2.Controls.Add(this.cmbSeats);
            this.groupBox2.Controls.Add(this.lblSeatSelection);
            this.groupBox2.Location = new System.Drawing.Point(12, 157);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(440, 185);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Зорчигч бүртгэх";
            // 
            // groupBox3
            // 
            this.groupBox3.Location = new System.Drawing.Point(458, 66);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(700, 500);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Суудлын зураг";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1200, 60);
            this.panel1.TabIndex = 15;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(231, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Нислэгийн бүртгэлийн систем";
            // 
            // CheckInForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 650);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(1000, 650);
            this.Name = "CheckInForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Нислэгийн бүртгэлийн систем";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Label lblFlightSelection;
        private ComboBox cmbFlights;
        private Label lblPassportNumber;
        private TextBox txtPassportNumber;
        private Button btnSearch;
        private Label lblPassengerInfo;
        private TextBox txtPassengerName;
        private Label lblSeatSelection;
        private ComboBox cmbSeats;
        private Button btnCheckIn;
        private Label lblFlightStatus;
        private Button btnChangeStatus;
        private Label lblAvailableSeats;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Panel panel1;
        private Label lblTitle;
        private Button buttonUpdateSeatStatus;
        private ComboBox comboBoxSeatStatus;
        private Label label8;
    }
}
