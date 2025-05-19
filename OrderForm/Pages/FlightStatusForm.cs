using System;
using System.Drawing;
using System.Windows.Forms;
using OrderForm.Models;
namespace OrderForm.Pages
{
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
            cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(12, 12),
                Name = "cmbStatus",
                Size = new Size(260, 23)
            };
            foreach (var status in Enum.GetValues(typeof(FlightStatus)))
            {
                cmbStatus.Items.Add(status);
            }
            cmbStatus.SelectedItem = SelectedStatus;

            btnOK = new Button
            {
                Location = new Point(116, 60),
                Name = "btnOK",
                Size = new Size(75, 40),
                Text = "OK",
                UseVisualStyleBackColor = true
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Location = new Point(200, 60),
                Name = "btnCancel",
                Size = new Size(75, 40),
                Text = "Цуцлах",
                UseVisualStyleBackColor = true
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(cmbStatus);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        private void SetupForm()
        {
            this.ClientSize = new Size(300, 130);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Нислэгийн төлөв өөрчлөх";
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (cmbStatus.SelectedItem != null)
            {
                SelectedStatus = (FlightStatus)cmbStatus.SelectedItem;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Төлөв сонгоно уу", "Анхааруулга",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
