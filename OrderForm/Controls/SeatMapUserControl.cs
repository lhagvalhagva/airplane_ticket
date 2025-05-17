using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DataAccess.Models;

namespace OrderForm.Controls
{
    public partial class SeatMapUserControl : UserControl
    {
        private FlowLayoutPanel _seatMapPanel;
        private Dictionary<string, Button> _seatButtons = new Dictionary<string, Button>();
        private Label _lblSelectedSeat;
        private Label _lblFlightInfo;

        private readonly int _rows = 6;  
        private readonly int _cols = 7;  
        private readonly string[] _seatLetters = { "A", "B", "C", "", "D", "E", "F" };

        private List<Seat> _availableSeats = new List<Seat>();
        private Seat? _selectedSeat;
        private Flight? _selectedFlight;

        public event EventHandler<SeatSelectedEventArgs> SeatSelected;

        public SeatMapUserControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            // UserControl тохиргоо
            this.Size = new Size(700, 500);
            this.BackColor = Color.White;
            
            // Нислэгийн дэлгэрэнгүй мэдээлэл харуулах
            _lblFlightInfo = new Label();
            _lblFlightInfo.AutoSize = false;
            _lblFlightInfo.TextAlign = ContentAlignment.MiddleCenter;
            _lblFlightInfo.Size = new Size(650, 40);
            _lblFlightInfo.Location = new Point(25, 10);
            _lblFlightInfo.Text = "Нислэгийн мэдээлэл";
            _lblFlightInfo.Font = new Font(_lblFlightInfo.Font.FontFamily, 12, FontStyle.Bold);
            _lblFlightInfo.ForeColor = Color.FromArgb(0, 122, 204);
            _lblFlightInfo.BorderStyle = BorderStyle.FixedSingle;
            _lblFlightInfo.BackColor = Color.FromArgb(240, 240, 240);
            this.Controls.Add(_lblFlightInfo);
            
            // Суудлын газрын зураг үүсгэх
            _seatMapPanel = new FlowLayoutPanel();
            _seatMapPanel.FlowDirection = FlowDirection.LeftToRight;
            _seatMapPanel.WrapContents = true;
            _seatMapPanel.Size = new Size(650, 380);
            _seatMapPanel.Location = new Point(25, 60);
            _seatMapPanel.BackColor = Color.White;
            _seatMapPanel.BorderStyle = BorderStyle.FixedSingle;
            _seatMapPanel.Padding = new Padding(10);
            this.Controls.Add(_seatMapPanel);

            // Суудал сонгосон үеийн мэдээлэл харуулах текст
            _lblSelectedSeat = new Label();
            _lblSelectedSeat.AutoSize = true;
            _lblSelectedSeat.Location = new Point(25, 450);
            _lblSelectedSeat.Text = "Сонгосон суудал: [Сонгоогүй]";
            _lblSelectedSeat.Font = new Font(_lblSelectedSeat.Font.FontFamily, 10, FontStyle.Bold);
            _lblSelectedSeat.ForeColor = Color.FromArgb(0, 122, 204);
            this.Controls.Add(_lblSelectedSeat);
        }

        // Боломжтой суудлын жагсаалт болон нислэгийн мэдээллийг шинэчлэх
        public void UpdateSeatMap(List<Seat> availableSeats, Flight selectedFlight)
        {
            _availableSeats = availableSeats;
            _selectedFlight = selectedFlight;
            _selectedSeat = null;
            
            // Нислэгийн мэдээллийн тэмдэглэгээг шинэчлэх
            if (_selectedFlight != null)
            {
                var departureTime = _selectedFlight.DepartureTime.ToString("yyyy-MM-dd HH:mm");
                var arrivalTime = _selectedFlight.ArrivalTime.ToString("yyyy-MM-dd HH:mm");
                _lblFlightInfo.Text = $"Нислэг: {_selectedFlight.FlightNumber} | {_selectedFlight.DepartureCity} → {_selectedFlight.ArrivalCity} | {departureTime}";
            }
            
            // Суудлын газрын зургийг шинэчилнэ
            RenderSeatMap();
            
            // Сонгосон суудлын мэдээллийг шинэчлэх
            UpdateSelectedSeatLabel();
        }

        // Суудлын газрын зургийг үүсгэх
        private void RenderSeatMap()
        {
            // Хуучин суудлуудыг арилгана
            _seatMapPanel.Controls.Clear();
            _seatButtons.Clear();

            // Онгоцны урд хэсгийн зураг
            Panel cockpitPanel = new Panel();
            cockpitPanel.Size = new Size(620, 50);
            cockpitPanel.Margin = new Padding(5, 5, 5, 20);
            cockpitPanel.Paint += (s, e) => {
                Graphics g = e.Graphics;
                // Онгоцны урд хэсгийг дүрслэх
                Point[] cockpitShape = {
                    new Point(310, 0),
                    new Point(200, 49),
                    new Point(420, 49)
                };
                g.FillPolygon(Brushes.LightGray, cockpitShape);
                g.DrawPolygon(Pens.Gray, cockpitShape);
                
                // Текст нэмэх
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString("ОНГОЦ", new Font("Arial", 12, FontStyle.Bold), Brushes.DimGray, new RectangleF(250, 15, 120, 25), sf);
            };
            
            _seatMapPanel.Controls.Add(cockpitPanel);

            // Дээд хэсэгт багануудын нэрийг харуулах
            for (int col = 0; col < _cols; col++)
            {
                Label colLabel = new Label();
                colLabel.Text = _seatLetters[col];
                colLabel.TextAlign = ContentAlignment.MiddleCenter;
                colLabel.Width = 50;
                colLabel.Height = 25;
                colLabel.Margin = new Padding(10, 0, 10, 10);
                colLabel.Font = new Font(colLabel.Font.FontFamily, 10, FontStyle.Bold);
                _seatMapPanel.Controls.Add(colLabel);
            }

            // Суудлуудыг мөр/багананд зохион байгуулж харуулах
            for (int row = 1; row <= _rows; row++)
            {
                // Мөрийн дугаар
                Label rowLabel = new Label();
                rowLabel.Text = row.ToString();
                rowLabel.TextAlign = ContentAlignment.MiddleRight;
                rowLabel.Width = 25;
                rowLabel.Height = 44;
                rowLabel.Margin = new Padding(0, 5, 5, 10);
                rowLabel.Font = new Font(rowLabel.Font.FontFamily, 10, FontStyle.Bold);
                _seatMapPanel.Controls.Add(rowLabel);

                // Мөр тус бүрт дэх суудлууд
                for (int col = 0; col < _cols; col++)
                {
                    // Хэрэв гол зай байвал хоосон зай үүсгэнэ
                    if (string.IsNullOrEmpty(_seatLetters[col]))
                    {
                        Panel emptyPanel = new Panel();
                        emptyPanel.Width = 55;
                        emptyPanel.Height = 50;
                        emptyPanel.Margin = new Padding(10, 5, 10, 10);
                        _seatMapPanel.Controls.Add(emptyPanel);
                        continue;
                    }

                    string seatNumber = $"{row}{_seatLetters[col]}";

                    Button seatButton = new Button();
                    seatButton.Text = seatNumber;
                    seatButton.Width = 55;
                    seatButton.Height = 50;
                    seatButton.Margin = new Padding(10, 5, 10, 10);
                    seatButton.Tag = seatNumber;
                    seatButton.FlatStyle = FlatStyle.Flat;
                    seatButton.Font = new Font(seatButton.Font.FontFamily, 11, FontStyle.Bold);
                    seatButton.Cursor = Cursors.Hand;

                    // Эзэлсэн суудал эсэхийг шалгах
                    bool isAvailable = _availableSeats.Any(s => s.SeatNumber == seatNumber);
                    
                    if (isAvailable)
                    {
                        // Боломжтой суудал ногоон өнгөөр
                        seatButton.BackColor = Color.FromArgb(200, 255, 200);
                        seatButton.ForeColor = Color.FromArgb(0, 100, 0);
                        seatButton.FlatAppearance.BorderColor = Color.FromArgb(0, 153, 76);
                        seatButton.Click += SeatButton_Click;
                        seatButton.Enabled = true;
                    }
                    else
                    {
                        // Эзэлсэн суудал улаан өнгөөр
                        seatButton.BackColor = Color.FromArgb(255, 200, 200);
                        seatButton.ForeColor = Color.FromArgb(153, 0, 0);
                        seatButton.FlatAppearance.BorderColor = Color.FromArgb(153, 0, 0);
                        seatButton.Enabled = false;
                    }

                    _seatButtons[seatNumber] = seatButton;
                    _seatMapPanel.Controls.Add(seatButton);
                }
            }

            // Тайлбар
            Panel legendPanel = new Panel();
            legendPanel.Width = 620;
            legendPanel.Height = 50;
            legendPanel.Margin = new Padding(10, 20, 5, 5);
            legendPanel.BackColor = Color.FromArgb(240, 240, 240);
            legendPanel.BorderStyle = BorderStyle.FixedSingle;

            Panel availableSeat = new Panel();
            availableSeat.BackColor = Color.FromArgb(200, 255, 200);
            availableSeat.Size = new Size(30, 30);
            availableSeat.Location = new Point(20, 10);
            availableSeat.BorderStyle = BorderStyle.FixedSingle;
            legendPanel.Controls.Add(availableSeat);

            Label availableLabel = new Label();
            availableLabel.Text = "Боломжтой суудал";
            availableLabel.AutoSize = true;
            availableLabel.Location = new Point(55, 16);
            availableLabel.Font = new Font(availableLabel.Font.FontFamily, 9);
            legendPanel.Controls.Add(availableLabel);

            Panel occupiedSeat = new Panel();
            occupiedSeat.BackColor = Color.FromArgb(255, 200, 200);
            occupiedSeat.Size = new Size(30, 30);
            occupiedSeat.Location = new Point(190, 10);
            occupiedSeat.BorderStyle = BorderStyle.FixedSingle;
            legendPanel.Controls.Add(occupiedSeat);

            Label occupiedLabel = new Label();
            occupiedLabel.Text = "Эзэлсэн суудал";
            occupiedLabel.AutoSize = true;
            occupiedLabel.Location = new Point(225, 16);
            occupiedLabel.Font = new Font(occupiedLabel.Font.FontFamily, 9);
            legendPanel.Controls.Add(occupiedLabel);

            Panel selectedSeat = new Panel();
            selectedSeat.BackColor = Color.FromArgb(255, 255, 150);
            selectedSeat.Size = new Size(30, 30);
            selectedSeat.Location = new Point(360, 10);
            selectedSeat.BorderStyle = BorderStyle.FixedSingle;
            legendPanel.Controls.Add(selectedSeat);

            Label selectedLabel = new Label();
            selectedLabel.Text = "Сонгосон суудал";
            selectedLabel.AutoSize = true;
            selectedLabel.Location = new Point(395, 16);
            selectedLabel.Font = new Font(selectedLabel.Font.FontFamily, 9);
            legendPanel.Controls.Add(selectedLabel);

            _seatMapPanel.Controls.Add(legendPanel);
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string seatNumber = clickedButton.Tag.ToString();

            // Суудал сонгосон үед бусад сонгосон суудлыг цэвэрлэнэ
            foreach (var button in _seatButtons.Values)
            {
                if (button.BackColor == Color.FromArgb(255, 255, 150))
                {
                    button.BackColor = Color.FromArgb(200, 255, 200);
                    button.ForeColor = Color.FromArgb(0, 100, 0);
                }
            }

            // Шинээр сонгосон суудлыг тэмдэглэнэ
            clickedButton.BackColor = Color.FromArgb(255, 255, 150);
            clickedButton.ForeColor = Color.FromArgb(153, 76, 0);
            
            // Сонгосон суудлыг олох
            _selectedSeat = _availableSeats.FirstOrDefault(s => s.SeatNumber == seatNumber);
            
            // Сонгосон суудлын мэдээллийг шинэчлэх
            UpdateSelectedSeatLabel();

            // Сонгосон суудал өөрчлөгдсөн event дуудах
            OnSeatSelected();
        }

        private void UpdateSelectedSeatLabel()
        {
            _lblSelectedSeat.Text = _selectedSeat != null 
                ? $"Сонгосон суудал: {_selectedSeat.SeatNumber}" 
                : "Сонгосон суудал: [Сонгоогүй]";
        }

        // Сонгосон суудлыг авах property
        public Seat? SelectedSeat 
        { 
            get { return _selectedSeat; }
        }

        // Сонгосон суудал өөрчлөгдсөн event дуудах
        protected virtual void OnSeatSelected()
        {
            SeatSelected?.Invoke(this, new SeatSelectedEventArgs(_selectedSeat));
        }

        // Сонгосон суудлыг арилгах
        public void ClearSelectedSeat()
        {
            foreach (var button in _seatButtons.Values)
            {
                if (button.BackColor == Color.FromArgb(255, 255, 150))
                {
                    button.BackColor = Color.FromArgb(200, 255, 200);
                    button.ForeColor = Color.FromArgb(0, 100, 0);
                }
            }
            
            _selectedSeat = null;
            UpdateSelectedSeatLabel();
        }
    }

    public class SeatSelectedEventArgs : EventArgs
    {
        public Seat? SelectedSeat { get; private set; }

        public SeatSelectedEventArgs(Seat? selectedSeat)
        {
            SelectedSeat = selectedSeat;
        }
    }
} 