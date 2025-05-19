using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrderForm.Models;

namespace OrderForm.Controls
{
    public partial class SimpleSeatMapControl : UserControl
    {
        #region --- Configurable Properties ---

        [Category("Seat Map"), Description("Суудлын Button-ийн өргөн")]
        public int SeatWidth { get; set; } = 35;

        [Category("Seat Map"), Description("Суудлын Button-ийн өндөр")]
        public int SeatHeight { get; set; } = 35;

        [Category("Seat Map"), Description("Суудлын хоорондын зай")]
        public int Spacing { get; set; } = 5;

        [Category("Seat Map"), Description("Хагас талын зай")]
        public int AisleWidth { get; set; } = 20;

        [Category("Seat Map"), Description("Боломжтой суудлын өнгө")]
        public Color AvailableColor { get; set; } = Color.LightGreen;

        [Category("Seat Map"), Description("Зөвшөөрөгдөөгүй суудлын өнгө")]
        public Color OccupiedColor { get; set; } = Color.Red;

        [Category("Seat Map"), Description("Сонгогдсон суудлын өнгө")]
        public Color SelectedColor { get; set; } = Color.Blue;

        #endregion

        #region --- Internal Fields ---

        private readonly List<SeatButton> _seatButtons = new List<SeatButton>();
        private SeatButton _selectedSeatButton;
        private string _selectedSeatNumber;

        #endregion

        #region --- Public Events & Properties ---

        /// <summary>
        /// Суудлын сонголт хийгдэхэд дуудагдах event
        /// </summary>
        public event EventHandler<SeatEventArgs> SeatSelected;

        /// <summary>
        /// Сонгогдсон суудлын дугаар
        /// </summary>
        public string SelectedSeatNumber
        {
            get => _selectedSeatNumber;
            private set
            {
                _selectedSeatNumber = value;
                OnSeatSelected(value);
            }
        }

        #endregion

        #region --- Конструктор ---

        public SimpleSeatMapControl()
        {
            InitializeComponent();
            this.BackColor = Color.White;
            this.AutoScroll = true;
        }

        #endregion

        #region --- Public API ---

        /// <summary>
        /// Суудлын газрын зураг үүсгэх
        /// </summary>
        /// <param name="seats">SeatDto жагсаалт</param>
        public void CreateSeatMap(List<SeatDto> seats)
        {
            try
            {
                ClearSeatMap();

                if (seats == null || seats.Count == 0)
                    return;

                int maxRow = seats.Max(s => s.Row);
                var columns = seats.Select(s => s.Column)
                                   .Distinct()
                                   .OrderBy(c => c)
                                   .ToList();

                // Баганын толгой үүсгэх
                for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                {
                    AddColumnHeader(columns[colIdx], colIdx, columns.Count);
                }

                // Ряд болон суудлын Button үүсгэх
                for (int row = 1; row <= maxRow; row++)
                {
                    AddRowHeader(row);
                    for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                    {
                        var seat = seats.FirstOrDefault(s => s.Row == row && s.Column == columns[colIdx]);
                        if (seat == null)
                            continue;

                        int xPos = GetXPosition(colIdx, columns.Count);
                        int yPos = GetYPosition(row);

                        var btn = CreateSeatButton(seat, xPos, yPos);
                        this.Controls.Add(btn);
                        _seatButtons.Add(btn);
                    }
                }

                CreateLegend();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateSeatMap error: {ex}");
            }
        }

        /// <summary>
        /// Бүх суудлыг цэвэрлэх
        /// </summary>
        public void ClearSeatMap()
        {
            foreach (var btn in _seatButtons)
            {
                btn.Click -= SeatButton_Click;
            }
            this.Controls.Clear();
            _seatButtons.Clear();
            _selectedSeatButton = null;
            SelectedSeatNumber = null;
        }

        #endregion

        #region --- Layout Calculation ---

        private int GetXPosition(int colIndex, int totalColumns)
        {
            int xPos = 30 + colIndex * (SeatWidth + Spacing);
            if (colIndex >= totalColumns / 2)
                xPos += AisleWidth;
            return xPos;
        }

        private int GetYPosition(int rowIndex)
        {
            return rowIndex * (SeatHeight + Spacing) + 15;
        }

        #endregion

        #region --- Header Creation ---

        private void AddColumnHeader(string columnText, int colIndex, int totalColumns)
        {
            int xPos = GetXPosition(colIndex, totalColumns);
            var lbl = new Label
            {
                Text = columnText,
                AutoSize = true,
                Location = new Point(xPos + SeatWidth / 2 - 5, 10),
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            this.Controls.Add(lbl);
        }

        private void AddRowHeader(int rowIndex)
        {
            int yPos = GetYPosition(rowIndex);
            var lbl = new Label
            {
                Text = rowIndex.ToString(),
                AutoSize = true,
                Location = new Point(10, yPos),
                Font = new Font("Arial", 8, FontStyle.Bold)
            };
            this.Controls.Add(lbl);
        }

        #endregion

        #region --- SeatButton Creation & Appearance ---

        private SeatButton CreateSeatButton(SeatDto seat, int xPos, int yPos)
        {
            var btn = new SeatButton
            {
                Size = new Size(SeatWidth, SeatHeight),
                Location = new Point(xPos, yPos),
                Text = seat.SeatNumber,
                IsOccupied = seat.IsOccupied,
                SeatNumber = seat.SeatNumber,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8, FontStyle.Bold),
                Tag = seat
            };
            UpdateButtonAppearance(btn);
            btn.Click += SeatButton_Click;
            return btn;
        }

        private void UpdateButtonAppearance(SeatButton btn)
        {
            if (btn.IsOccupied)
            {
                btn.BackColor = OccupiedColor;
                btn.ForeColor = Color.White;
                btn.Enabled = false;
            }
            else if (btn.IsSelected)
            {
                btn.BackColor = SelectedColor;
                btn.ForeColor = Color.White;
                btn.Enabled = true;
            }
            else
            {
                btn.BackColor = AvailableColor;
                btn.ForeColor = Color.Black;
                btn.Enabled = true;
            }
        }

        #endregion

        #region --- Legend Creation ---

        private void CreateLegend()
        {
            int yPos = this.Height - 60;
            AddLegendItem("Доступно", AvailableColor, 10, yPos);
            AddLegendItem("Занято", OccupiedColor, 150, yPos);
            AddLegendItem("Выбрано", SelectedColor, 290, yPos);
        }

        private void AddLegendItem(string text, Color color, int xPos, int yPos)
        {
            var colorBox = new Panel
            {
                Size = new Size(20, 20),
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(xPos, yPos)
            };
            var lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(xPos + 25, yPos + 3),
                Font = new Font("Arial", 8)
            };
            this.Controls.Add(colorBox);
            this.Controls.Add(lbl);
        }

        #endregion

        #region --- Event Handling ---

        private void SeatButton_Click(object sender, EventArgs e)
        {
            if (sender is not SeatButton clicked) return;
            if (clicked.IsOccupied) return;

            if (_selectedSeatButton == clicked)
            {
                clicked.IsSelected = false;
                _selectedSeatButton = null;
                SelectedSeatNumber = null;
            }
            else
            {
                if (_selectedSeatButton != null)
                {
                    _selectedSeatButton.IsSelected = false;
                    UpdateButtonAppearance(_selectedSeatButton);
                }
                clicked.IsSelected = true;
                _selectedSeatButton = clicked;
                SelectedSeatNumber = clicked.SeatNumber;
            }
            UpdateButtonAppearance(clicked);
        }

        protected virtual void OnSeatSelected(string seatNumber)
        {
            SeatSelected?.Invoke(this, new SeatEventArgs { SeatNumber = seatNumber });
        }

        #endregion
    }

    /// <summary>Суудлын Button</summary>
    public class SeatButton : Button
    {
        public string SeatNumber { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsSelected { get; set; }
    }

    /// <summary>Суудлын сонголтын аргумент</summary>
    public class SeatEventArgs : EventArgs
    {
        public string SeatNumber { get; set; }
    }
}
