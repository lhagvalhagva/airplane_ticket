using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OrderForm.Models;

namespace OrderForm.Controls
{
    public class SimpleSeatMap : Panel
    {
        // Константы для размеров и расстояний
        private const int SEAT_WIDTH = 35;
        private const int SEAT_HEIGHT = 35;
        private const int SPACING = 5;
        private const int AISLE_WIDTH = 20;
        
        // Цвета для разных типов мест
        private static readonly Color AVAILABLE_COLOR = Color.LightGreen;
        private static readonly Color OCCUPIED_COLOR = Color.Red;
        private static readonly Color SELECTED_COLOR = Color.Blue;
        
        // Кнопки мест и выбранное место
        private List<SeatButton> _seatButtons = new List<SeatButton>();
        private SeatButton _selectedSeatButton = null;
        private string _selectedSeatNumber = null;
        
        // Событие при выборе места
        public event EventHandler<SeatEventArgs> SeatSelected;
        
        // Свойство для получения выбранного места
        public string SelectedSeatNumber 
        { 
            get { return _selectedSeatNumber; }
            private set 
            { 
                _selectedSeatNumber = value;
                OnSeatSelected(value);
            }
        }
        
        public SimpleSeatMap()
        {
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Size = new Size(500, 400);
            this.AutoScroll = true;
        }
        
        // Создание карты мест
        public void CreateSeatMap(List<SeatDto> seats)
        {
            // Очистка предыдущих элементов
            this.Controls.Clear();
            _seatButtons.Clear();
            _selectedSeatButton = null;
            SelectedSeatNumber = null;
            
            if (seats == null || seats.Count == 0)
                return;
                
            // Находим максимальный ряд и колонки
            int maxRow = seats.Max(s => s.Row);
            var columns = seats.Select(s => s.Column).Distinct().OrderBy(c => c).ToList();
                
            // Добавляем заголовки колонок
            for (int col = 0; col < columns.Count; col++)
            {
                int xPos = CalculateXPosition(col, columns);
                
                Label lblColumn = new Label()
                {
                    Text = columns[col],
                    AutoSize = true,
                    Location = new Point(xPos + SEAT_WIDTH/2 - 5, 10),
                    Font = new Font("Arial", 8, FontStyle.Bold)
                };
                this.Controls.Add(lblColumn);
            }
            
            // Добавляем ряды и места
            for (int row = 1; row <= maxRow; row++)
            {
                // Номер ряда
                Label lblRow = new Label()
                {
                    Text = row.ToString(),
                    AutoSize = true,
                    Location = new Point(10, row * (SEAT_HEIGHT + SPACING) + 15),
                    Font = new Font("Arial", 8, FontStyle.Bold)
                };
                this.Controls.Add(lblRow);
                
                // Места в ряду
                for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                {
                    string column = columns[colIndex];
                    
                    // Ищем соответствующее место в данных
                    var seat = seats.FirstOrDefault(s => s.Row == row && s.Column == column);
                    if (seat == null)
                        continue;
                        
                    int xPos = CalculateXPosition(colIndex, columns);
                    
                    // Создаем кнопку места
                    SeatButton seatButton = new SeatButton()
                    {
                        Size = new Size(SEAT_WIDTH, SEAT_HEIGHT),
                        Location = new Point(xPos, row * (SEAT_HEIGHT + SPACING) + 15),
                        Text = seat.SeatNumber,
                        IsOccupied = seat.IsOccupied,
                        SeatNumber = seat.SeatNumber,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Arial", 8, FontStyle.Bold),
                        Tag = seat
                    };
                    
                    // Обновляем внешний вид кнопки
                    UpdateButtonAppearance(seatButton);
                    
                    // Добавляем обработчик клика
                    seatButton.Click += SeatButton_Click;
                    
                    // Добавляем кнопку на форму и в список
                    this.Controls.Add(seatButton);
                    _seatButtons.Add(seatButton);
                }
            }
            
            // Добавляем легенду
            CreateLegend();
        }
        
        // Расчет X-позиции для места с учетом прохода
        private int CalculateXPosition(int colIndex, List<string> columns)
        {
            int xPos = 30 + colIndex * (SEAT_WIDTH + SPACING);
            
            // Если прошли половину мест, добавляем проход
            if (colIndex >= columns.Count / 2)
                xPos += AISLE_WIDTH;
                
            return xPos;
        }
        
        // Создание легенды
        private void CreateLegend()
        {
            int yPos = this.Height - 60;
            
            // Доступные места
            DrawLegendItem("Доступно", AVAILABLE_COLOR, 10, yPos);
            
            // Занятые места
            DrawLegendItem("Занято", OCCUPIED_COLOR, 150, yPos);
            
            // Выбранное место
            DrawLegendItem("Выбрано", SELECTED_COLOR, 290, yPos);
        }
        
        // Рисование элемента легенды
        private void DrawLegendItem(string text, Color color, int xPos, int yPos)
        {
            Panel colorBox = new Panel()
            {
                Size = new Size(20, 20),
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(xPos, yPos)
            };
            
            Label lblText = new Label()
            {
                Text = text,
                AutoSize = true,
                Location = new Point(xPos + 25, yPos + 3),
                Font = new Font("Arial", 8)
            };
            
            this.Controls.Add(colorBox);
            this.Controls.Add(lblText);
        }
        
        // Обработчик клика на место
        private void SeatButton_Click(object sender, EventArgs e)
        {
            if (sender is SeatButton clickedSeat && !clickedSeat.IsOccupied)
            {
                // Если уже выбрано это место, снимаем выбор
                if (_selectedSeatButton == clickedSeat)
                {
                    clickedSeat.IsSelected = false;
                    _selectedSeatButton = null;
                    SelectedSeatNumber = null;
                }
                // Иначе выбираем новое место
                else
                {
                    // Если уже выбрано другое место, снимаем с него выбор
                    if (_selectedSeatButton != null)
                    {
                        _selectedSeatButton.IsSelected = false;
                        UpdateButtonAppearance(_selectedSeatButton);
                    }
                    
                    // Выбираем новое место
                    clickedSeat.IsSelected = true;
                    _selectedSeatButton = clickedSeat;
                    SelectedSeatNumber = clickedSeat.SeatNumber;
                }
                
                // Обновляем внешний вид кнопки
                UpdateButtonAppearance(clickedSeat);
            }
        }
        
        // Обновление внешнего вида кнопки места
        private void UpdateButtonAppearance(SeatButton button)
        {
            if (button.IsOccupied)
            {
                button.BackColor = OCCUPIED_COLOR;
                button.ForeColor = Color.White;
                button.Enabled = false;
            }
            else if (button.IsSelected)
            {
                button.BackColor = SELECTED_COLOR;
                button.ForeColor = Color.White;
                button.Enabled = true;
            }
            else
            {
                button.BackColor = AVAILABLE_COLOR;
                button.ForeColor = Color.Black;
                button.Enabled = true;
            }
        }
        
        // Очистка выбранного места
        public void ClearSelectedSeat()
        {
            if (_selectedSeatButton != null)
            {
                _selectedSeatButton.IsSelected = false;
                UpdateButtonAppearance(_selectedSeatButton);
                _selectedSeatButton = null;
                SelectedSeatNumber = null;
            }
        }
        
        // Вызов события выбора места
        protected virtual void OnSeatSelected(string seatNumber)
        {
            SeatSelected?.Invoke(this, new SeatEventArgs { SeatNumber = seatNumber });
        }
    }
    
    // Кнопка места
    public class SeatButton : Button
    {
        public string SeatNumber { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsSelected { get; set; }
    }
    
    // Аргументы события выбора места
    public class SeatEventArgs : EventArgs
    {
        public string SeatNumber { get; set; }
    }
}
