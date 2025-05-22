using Microsoft.AspNetCore.SignalR.Client;

namespace WinFormsApp31;

public partial class Form1 : Form
{
    private readonly string _url = "http://localhost:5000/flightstatushub";
    HubConnection _hubConnection;
    public Form1()
    {
        InitializeComponent();


    }

    private async Task ConnectToHub()
    {
        // Create a new HubConnection
        _hubConnection = new HubConnectionBuilder()
        .WithUrl(new Uri(textBox_HubURL.Text ?? _url))
        .WithAutomaticReconnect()
        .Build();

        label_ConnectionStatus.Text = "Connecting...";
        await _hubConnection.StartAsync();
        label_ConnectionStatus.Text = _hubConnection.State.ToString();
        _hubConnection.Reconnected += async (connectionId) =>
        {
            label_ConnectionStatus.Text = "Reconnected to the server";
        };

        _hubConnection.Reconnecting += async (exception) =>
        {
            label_ConnectionStatus.Text = "Reconnecting to the server...";
        };

        _hubConnection.Closed += async (exception) =>
        {
            label_ConnectionStatus.Text = "Disconnected from the server";
        };

        
    }
    private void button1_Click(object sender, EventArgs e)
    {
        blazorWebView1.WebView.Source = new Uri(textBox1.Text);
    }

    private void button2_Click(object sender, EventArgs e)
    {
        // Send a message to the server
        _hubConnection.InvokeAsync("SendFlightStatus", textBox_FlightNumber.Text, textBox_Status.Text);
    }

    private void Form1_Load(object sender, EventArgs e)
    {

        blazorWebView1.WebView.Source = new Uri(textBox1.Text);
        //label_ConnectionStatus.Text = _hubConnection.State.ToString();
    }

    private async void button3_Click(object sender, EventArgs e)
    {
        await ConnectToHub();
    }
}
