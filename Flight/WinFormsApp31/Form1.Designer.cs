namespace WinFormsApp31;

partial class Form1
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
        blazorWebView1 = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
        textBox1 = new TextBox();
        button1 = new Button();
        button2 = new Button();
        textBox_Status = new TextBox();
        textBox_FlightNumber = new TextBox();
        label_ConnectionStatus = new Label();
        textBox_HubURL = new TextBox();
        label2 = new Label();
        groupBox1 = new GroupBox();
        button3 = new Button();
        groupBox1.SuspendLayout();
        SuspendLayout();
        // 
        // blazorWebView1
        // 
        blazorWebView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        blazorWebView1.Location = new Point(12, 114);
        blazorWebView1.Name = "blazorWebView1";
        blazorWebView1.Size = new Size(997, 664);
        blazorWebView1.TabIndex = 0;
        blazorWebView1.Text = "blazorWebView1";
        // 
        // textBox1
        // 
        textBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        textBox1.Location = new Point(12, 62);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(886, 39);
        textBox1.TabIndex = 1;
        textBox1.Text = "https://localhost:7227/flightstatusdisplay";
        // 
        // button1
        // 
        button1.Location = new Point(935, 59);
        button1.Name = "button1";
        button1.Size = new Size(74, 49);
        button1.TabIndex = 2;
        button1.Text = "button1";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // button2
        // 
        button2.Location = new Point(67, 371);
        button2.Name = "button2";
        button2.Size = new Size(153, 72);
        button2.TabIndex = 3;
        button2.Text = "Change status";
        button2.UseVisualStyleBackColor = true;
        button2.Click += button2_Click;
        // 
        // textBox_Status
        // 
        textBox_Status.Location = new Point(70, 314);
        textBox_Status.Name = "textBox_Status";
        textBox_Status.PlaceholderText = "Status";
        textBox_Status.Size = new Size(360, 31);
        textBox_Status.TabIndex = 4;
        // 
        // textBox_FlightNumber
        // 
        textBox_FlightNumber.Location = new Point(70, 259);
        textBox_FlightNumber.Name = "textBox_FlightNumber";
        textBox_FlightNumber.PlaceholderText = "Flight number";
        textBox_FlightNumber.Size = new Size(360, 31);
        textBox_FlightNumber.TabIndex = 5;
        // 
        // label_ConnectionStatus
        // 
        label_ConnectionStatus.AutoSize = true;
        label_ConnectionStatus.Location = new Point(70, 189);
        label_ConnectionStatus.Name = "label_ConnectionStatus";
        label_ConnectionStatus.Size = new Size(129, 25);
        label_ConnectionStatus.TabIndex = 6;
        label_ConnectionStatus.Text = "Not connected";
        // 
        // textBox_HubURL
        // 
        textBox_HubURL.Location = new Point(67, 133);
        textBox_HubURL.Name = "textBox_HubURL";
        textBox_HubURL.Size = new Size(363, 31);
        textBox_HubURL.TabIndex = 7;
        textBox_HubURL.Text = "http://localhost:5000/flightstatushub";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(66, 105);
        label2.Name = "label2";
        label2.Size = new Size(143, 25);
        label2.TabIndex = 8;
        label2.Text = "SignalR hub URL";
        // 
        // groupBox1
        // 
        groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        groupBox1.Controls.Add(button3);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(button2);
        groupBox1.Controls.Add(textBox_HubURL);
        groupBox1.Controls.Add(textBox_Status);
        groupBox1.Controls.Add(label_ConnectionStatus);
        groupBox1.Controls.Add(textBox_FlightNumber);
        groupBox1.Location = new Point(1050, 74);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(505, 704);
        groupBox1.TabIndex = 9;
        groupBox1.TabStop = false;
        groupBox1.Text = "Flight status";
        // 
        // button3
        // 
        button3.Location = new Point(217, 180);
        button3.Name = "button3";
        button3.Size = new Size(213, 34);
        button3.TabIndex = 9;
        button3.Text = "Connect";
        button3.UseVisualStyleBackColor = true;
        button3.Click += button3_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1587, 790);
        Controls.Add(groupBox1);
        Controls.Add(button1);
        Controls.Add(textBox1);
        Controls.Add(blazorWebView1);
        Name = "Form1";
        Text = "Form1";
        Load += Form1_Load;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView1;
    private TextBox textBox1;
    private Button button1;
    private Button button2;
    private TextBox textBox_Status;
    private TextBox textBox_FlightNumber;
    private Label label_ConnectionStatus;
    private TextBox textBox_HubURL;
    private Label label2;
    private GroupBox groupBox1;
    private Button button3;
}
