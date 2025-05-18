using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrderForm.Models;

namespace OrderForm.Controls
{
    // Суудал сонгогдоход ашиглагдах эвентийн объект
    public class SeatSelectedEventArgs : EventArgs
    {
        public SeatDto SelectedSeat { get; set; }
    }
    
    public partial class SeatMapUserControl : UserControl
    {
        private const int SEAT_WIDTH = 40;
        private const int SEAT_HEIGHT = 40;
        private const int SEAT_MARGIN = 5;
        private const int AISLE_WIDTH = 20;
        
        private List<SeatButton> _seatButtons = new List<SeatButton>();
        private SeatButton _selectedSeatButton = null;
        private List<SeatDto> _seats = new List<SeatDto>();
        
        // Суудал сонгогдох үеийн эвент
        public event EventHandler<SeatSelectedEventArgs> SeatSelected;
        
        // Сонгогдсон суудлыг авах, олонгүй бол null
        public SeatDto SelectedSeat 
        { 
            get 
            { 
                return _selectedSeatButton?.SeatData; 
            } 
        }

        public SeatMapUserControl()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.Paint += SeatMapUserControl_Paint;
            this.Resize += SeatMapUserControl_Resize;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SeatMapUserControl
            // 
            this.Name = "SeatMapUserControl";
            this.Size = new System.Drawing.Size(500, 400);
            this.ResumeLayout(false);
        }

        // Суудлын төлөвийг шинэчлэх
        public void UpdateSeatMap(List<SeatDto> seats)
        {
            _seats = seats;
            CreateSeatButtons();
            this.Invalidate(); // Дахин зурах
        }
        
        // Компиляцийн алдааг засахад зориулсан хуучин нэртэй функц 
        public void InitializeSeats(List<SeatDto> allSeats, List<SeatDto> availableSeats)
        {
            // Нийт суудлын мэдээллийг ашиглана
            // Боломжтой суудлуудын дагуу IsOccupied утгыг шинэчилнэ
            
            // Эхлээд бүх суудлыг эзэлсэн гэж тэмдэглэнэ
            foreach (var seat in allSeats)
            {
                seat.IsOccupied = true;
            }
            
            // Дараа нь боломжтой суудлыг авах боломжтой гэж тэмдэглэнэ
            foreach (var availableSeat in availableSeats)
            {
                var seat = allSeats.FirstOrDefault(s => s.Id == availableSeat.Id || s.SeatNumber == availableSeat.SeatNumber);
                if (seat != null)
                {
                    seat.IsOccupied = false;
                }
            }
            
            // Суудлын зураглалыг шинэчлэх
            UpdateSeatMap(allSeats);
        }

        // Сонгогдсон суудлыг цэвэрлэх
        public void ClearSelectedSeat()
        {
            if (_selectedSeatButton != null)
            {
                _selectedSeatButton.IsSelected = false;
                _selectedSeatButton = null;
                this.Invalidate();
                
                // Сонгогдсон суудал өөрчлөгдөх үед эвент үүсгэх
                OnSeatSelected(null);
            }
        }

        private void CreateSeatButtons()
        {
            // Хуучин товчлууруудыг устгах
            foreach (var btn in _seatButtons)
            {
                this.Controls.Remove(btn);
            }
            _seatButtons.Clear();

            if (_seats == null || _seats.Count == 0)
            {
                Console.WriteLine("No seats available to display");
                return;
            }
            
            Console.WriteLine($"Creating seat buttons for {_seats.Count} seats");
            
            // Тухайн суудлын мөр баганыг олох
            // Хэрэв Row ба Column хоосон байвал суудлын дугаараас эдгээрийг олох
            foreach (var seat in _seats)
            {
                if (seat.Row == 0 || string.IsNullOrEmpty(seat.Column))
                {
                    // Суудлын дугаараас мөр баганыг олох
                    if (int.TryParse(seat.SeatNumber.Substring(0, seat.SeatNumber.Length - 1), out int rowNum))
                    {
                        seat.Row = rowNum;
                        seat.Column = seat.SeatNumber.Substring(seat.SeatNumber.Length - 1, 1);
                        Console.WriteLine($"Parsed seat {seat.SeatNumber} as Row: {seat.Row}, Column: {seat.Column}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to parse seat number: {seat.SeatNumber}");
                        continue; // Энэ суудлыг алгасах
                    }
                }
            }

            // Сүүлийн мөрний дугаар олох
            int maxRow = _seats.Max(s => s.Row);
            
            // Багануудыг үсгээр эрэмбэлэх (A, B, C, D, E, F)
            var columns = _seats.Select(s => s.Column).Distinct().OrderBy(c => c).ToList();
            
            Console.WriteLine($"Seat map with {maxRow} rows and {columns.Count} columns");
            foreach (var col in columns)
            {
                Console.WriteLine($"Column: {col}");
            }
            
            // Хөндлөн гараш хаана байхыг тодорхойлох
            int aisleAfterColumn = columns.Count / 2 - 1;
            if (aisleAfterColumn < 0) aisleAfterColumn = 0;
            
            // Суудал бүрийг үүсгэх
            foreach (var seat in _seats)
            {
                int columnIndex = columns.IndexOf(seat.Column);
                
                if (columnIndex == -1)
                {
                    Console.WriteLine($"Failed to find column index for seat {seat.SeatNumber}, Column: {seat.Column}");
                    continue; // Энэ суудлыг алгасах
                }
                
                // Гараш нэмэх
                int xOffset = 0;
                if (columnIndex > aisleAfterColumn)
                {
                    xOffset = AISLE_WIDTH;
                }
                
                // Суудлын товчийг байрлуулах
                int x = 40 + columnIndex * (SEAT_WIDTH + SEAT_MARGIN) + xOffset;
                int y = 40 + (seat.Row - 1) * (SEAT_HEIGHT + SEAT_MARGIN);
                
                var seatButton = new SeatButton
                {
                    SeatData = seat,
                    Location = new Point(x, y),
                    Size = new Size(SEAT_WIDTH, SEAT_HEIGHT),
                    Text = seat.SeatNumber,
                    IsOccupied = seat.IsOccupied,
                    IsSelected = false
                };
                
                seatButton.Click += SeatButton_Click;
                
                _seatButtons.Add(seatButton);
                this.Controls.Add(seatButton);
                Console.WriteLine($"Added seat button: {seat.SeatNumber}, Row: {seat.Row}, Column: {seat.Column}, IsOccupied: {seat.IsOccupied}");
            }
        }

        private void SeatButton_Click(object sender, EventArgs e)
        {
            if (sender is SeatButton seatButton)
            {
                // Хэрэв суудал эзэлсэн бол сонгох боломжгүй
                if (seatButton.IsOccupied)
                    return;
                    
                // Хуучин сонгогдсон суудлыг цэвэрлэх
                if (_selectedSeatButton != null)
                {
                    _selectedSeatButton.IsSelected = false;
                }
                
                // Шинэ суудал сонгох
                _selectedSeatButton = seatButton;
                _selectedSeatButton.IsSelected = true;
                
                // Сонгогдсон суудал өөрчлөгдөх үед эвент үүсгэх
                OnSeatSelected(seatButton.SeatData);
                
                this.Invalidate();
            }
        }

        protected virtual void OnSeatSelected(SeatDto seat)
        {
            SeatSelected?.Invoke(this, new SeatSelectedEventArgs { SelectedSeat = seat });
        }

        private void SeatMapUserControl_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            
            // Онгоцны их биеийн хэлбэр зурах
            DrawAirplaneBody(g);
            
            // Мөр, баганын нэрийг харуулах
            DrawRowColumnLabels(g);
        }
        
        private void DrawAirplaneBody(Graphics g)
        {
            if (_seats == null || _seats.Count == 0)
                return;
                
            // Суудлын мөрийн мэдээлэл
            int maxRow = _seats.Max(s => s.Row);
            var columns = _seats.Select(s => s.Column).Distinct().OrderBy(c => c).ToList();
            
            // Онгоцны хэмжээ тооцоолох
            int planeWidth = columns.Count * (SEAT_WIDTH + SEAT_MARGIN) + AISLE_WIDTH + 80;
            int planeHeight = maxRow * (SEAT_HEIGHT + SEAT_MARGIN) + 80;
            
            // Онгоцны гадна хэлбэр зурах
            using (var pen = new Pen(Color.SteelBlue, 2))
            {
                // Онгоцны хүрээ
                int bodyLeft = 20;
                int bodyTop = 20;
                int bodyRight = planeWidth - 20;
                int bodyBottom = planeHeight + 20;
                
                // Онгоцны хамрын хэсэг
                g.DrawLine(pen, bodyLeft, bodyTop + planeHeight/2, bodyLeft + 30, bodyTop);
                g.DrawLine(pen, bodyLeft + 30, bodyTop, bodyRight, bodyTop);
                
                // Онгоцны баруун тал
                g.DrawLine(pen, bodyRight, bodyTop, bodyRight, bodyBottom);
                
                // Онгоцны суул
                g.DrawLine(pen, bodyRight, bodyBottom, bodyLeft, bodyBottom);
                
                // Онгоцны зүүн тал
                g.DrawLine(pen, bodyLeft, bodyBottom, bodyLeft, bodyTop + planeHeight/2);
                
                // Онгоцны далавч
                int wingY = bodyTop + planeHeight/3;
                g.DrawLine(pen, bodyLeft + 30, wingY, bodyLeft - 30, wingY + 30);
                g.DrawLine(pen, bodyRight - 30, wingY, bodyRight + 30, wingY + 30);
            }
            
            // "Нислэгийн суудлын зураглал" гарчиг
            using (var font = new Font("Arial", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Navy))
            {
                g.DrawString("Нислэгийн суудлын зураглал", font, brush, new PointF(planeWidth / 2 - 100, 5));
            }
        }
        
        private void DrawRowColumnLabels(Graphics g)
        {
            if (_seats == null || _seats.Count == 0)
                return;
                
            int maxRow = _seats.Max(s => s.Row);
            var columns = _seats.Select(s => s.Column).Distinct().OrderBy(c => c).ToList();
            
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                // Мөрийн дугаар
                for (int row = 1; row <= maxRow; row++)
                {
                    g.DrawString(row.ToString(), font, brush, new PointF(20, 40 + (row - 1) * (SEAT_HEIGHT + SEAT_MARGIN) + SEAT_HEIGHT/2 - 10));
                }
                
                // Багануудын нэр
                int aisleAfterColumn = columns.Count / 2 - 1;
                if (aisleAfterColumn < 0) aisleAfterColumn = 0;
                
                for (int col = 0; col < columns.Count; col++)
                {
                    int xOffset = 0;
                    if (col > aisleAfterColumn)
                    {
                        xOffset = AISLE_WIDTH;
                    }
                    
                    g.DrawString(columns[col], font, brush, new PointF(40 + col * (SEAT_WIDTH + SEAT_MARGIN) + xOffset + SEAT_WIDTH/2 - 5, 20));
                }
            }
        }
        
        private void SeatMapUserControl_Resize(object sender, EventArgs e)
        {
            // Хэмжээ өөрчлөгдөхөд дахин зурах
            this.Invalidate();
        }
    }

    // Суудлын товч
    public class SeatButton : Button
    {
        public SeatDto SeatData { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsSelected { get; set; }
        
        public SeatButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.Font = new Font("Arial", 10, FontStyle.Bold);
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = Color.Gray;
            this.Click += SeatButton_Click;
            UpdateAppearance();
        }
        
        private void SeatButton_Click(object sender, EventArgs e)
        {
            if (!IsOccupied)
            {
                IsSelected = !IsSelected;
                UpdateAppearance();
            }
        }
        
        public void UpdateAppearance()
        {
            if (IsOccupied)
            {
                this.BackColor = Color.Gray;
                this.ForeColor = Color.White;
                this.Enabled = false;
            }
            else if (IsSelected)
            {
                this.BackColor = Color.Green;
                this.ForeColor = Color.White;
                this.Enabled = true;
            }
            else
            {
                this.BackColor = Color.SkyBlue;
                this.ForeColor = Color.Black;
                this.Enabled = true;
            }
        }
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            
            // Суудлын дүрс нэмж зурах
            Graphics g = pevent.Graphics;
            
            // Суудлын "бүс" зурах
            using (var pen = new Pen(Color.DarkGray, 1))
            {
                g.DrawLine(pen, 5, this.Height / 2, this.Width - 5, this.Height / 2);
            }
            
            // Суудлын дугаар зурах
            if (IsOccupied)
            {
                // Эзэлсэн суудал дээр "X" тэмдэг зурах
                using (var pen = new Pen(Color.White, 2))
                {
                    g.DrawLine(pen, 10, 10, this.Width - 10, this.Height - 10);
                    g.DrawLine(pen, 10, this.Height - 10, this.Width - 10, 10);
                }
            }
        }
    }
}
