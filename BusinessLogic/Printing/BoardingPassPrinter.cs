using System;
using System.Drawing;
using System.Drawing.Printing;
using DataAccess.Models;

namespace BusinessLogic.Printing
{
    /// <summary>
    /// Онгоцны суух үнэмлэх (тасалбар) хэвлэх үйлчилгээний класс.
    /// </summary>
    public class BoardingPassPrinter
    {
        private BoardingPass _boardingPass;
        private readonly PrintDocument _printDocument;

        public BoardingPassPrinter()
        {
            _printDocument = new PrintDocument();
            _printDocument.PrintPage += PrintDocument_PrintPage;
        }

        /// <summary>
        /// Онгоцны суух үнэмлэх хэвлэх
        /// </summary>
        /// <param name="boardingPass">Хэвлэх онгоцны бүртгэлийн мэдээлэл</param>
        /// <returns>Хэвлэлтийн амжилттай эсэх</returns>
        public bool PrintBoardingPass(BoardingPass boardingPass)
        {
            if (boardingPass == null)
                throw new ArgumentNullException(nameof(boardingPass));

            _boardingPass = boardingPass;

            try
            {
                _printDocument.Print();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Зөвхөн хэвлэх агуулгыг буцаах (хэвлэх диалогийг ашиглахгүй гар аргаар хэвлэх)
        /// </summary>
        /// <param name="boardingPass">Хэвлэх онгоцны бүртгэлийн мэдээлэл</param>
        /// <returns>Хэвлэх агуулгын тайлбар</returns>
        public string GetPrintContent(BoardingPass boardingPass)
        {
            if (boardingPass == null)
                throw new ArgumentNullException(nameof(boardingPass));

            _boardingPass = boardingPass;

            try
            {
                return GenerateBoardingPassContent();
            }
            catch (Exception ex)
            {
                return $"Хэвлэх үед алдаа гарлаа: {ex.Message}";
            }
        }

        /// <summary>
        /// Хэвлэх хуудасны контент үүсгэх
        /// </summary>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_boardingPass == null)
                return;

            Graphics graphics = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10);
            Font smallFont = new Font("Arial", 8);
            
            float lineHeight = titleFont.GetHeight();
            float x = 50; // Left margin
            float y = 50; // Top margin
            
            // Онгоцны компанийн лого зураг
            try
            {
                Image logo = Image.FromFile("logo.png");
                graphics.DrawImage(logo, x, y, 80, 80);
                y += 100; // Зай үлдээх
            }
            catch (Exception)
            {
                // Лого олдохгүй бол текстээр харуулна
                graphics.DrawString("MIAT Mongolian Airlines", titleFont, Brushes.Blue, x, y);
                y += lineHeight * 2;
            }

            // Гарчиг
            graphics.DrawString("НИСЛЭГИЙН СУУХ ҮНЭМЛЭХ / BOARDING PASS", titleFont, Brushes.Black, x, y);
            y += lineHeight * 2;

            // Нислэгийн мэдээлэл - зүүн талд
            float leftColumnX = x;
            float currentY = y;
            
            graphics.DrawString("Нислэгийн дугаар / Flight:", headerFont, Brushes.Black, leftColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Flight.FlightNumber, normalFont, Brushes.Black, leftColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;
            
            graphics.DrawString("Гарах хот / From:", headerFont, Brushes.Black, leftColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Flight.DepartureCity, normalFont, Brushes.Black, leftColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;
            
            graphics.DrawString("Очих хот / To:", headerFont, Brushes.Black, leftColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Flight.ArrivalCity, normalFont, Brushes.Black, leftColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;
            
            graphics.DrawString("Огноо / Date:", headerFont, Brushes.Black, leftColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Flight.DepartureTime.ToString("yyyy-MM-dd"), normalFont, Brushes.Black, leftColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;
            
            graphics.DrawString("Цаг / Time:", headerFont, Brushes.Black, leftColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Flight.DepartureTime.ToString("HH:mm"), normalFont, Brushes.Black, leftColumnX + 20, currentY);
            
            // Зорчигчийн мэдээлэл - баруун талд
            float rightColumnX = x + 300;
            currentY = y;
            
            graphics.DrawString("Зорчигчийн нэр / Passenger:", headerFont, Brushes.Black, rightColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString($"{_boardingPass.Passenger.LastName} {_boardingPass.Passenger.FirstName}", normalFont, Brushes.Black, rightColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;
            
            graphics.DrawString("Паспортын дугаар / Passport:", headerFont, Brushes.Black, rightColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Passenger.PassportNumber, normalFont, Brushes.Black, rightColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;

            // Суудлын мэдээлэл
            graphics.DrawString("Суудал / Seat:", headerFont, Brushes.Black, rightColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.Seat.SeatNumber, new Font("Arial", 14, FontStyle.Bold), Brushes.Red, rightColumnX + 20, currentY);
            currentY += lineHeight * 1.5f;
            
            graphics.DrawString("Бүртгэсэн цаг / Check-in time:", headerFont, Brushes.Black, rightColumnX, currentY);
            currentY += lineHeight;
            graphics.DrawString(_boardingPass.CheckInTime.ToString("yyyy-MM-dd HH:mm"), normalFont, Brushes.Black, rightColumnX + 20, currentY);
        }
        
        /// <summary>
        /// Бортын нэмэлт агуулгыг текст хэлбэрээр үүсгэх
        /// </summary>
        private string GenerateBoardingPassContent()
        {
            if (_boardingPass == null)
                return "Бүртгэлийн мэдээлэл олдсонгүй";

            string content = "===== НИСЛЭГИЙН СУУХ ҮНЭМЛЭХ / BOARDING PASS =====\n\n";
            
            content += $"Нислэгийн дугаар / Flight: {_boardingPass.Flight.FlightNumber}\n";
            content += $"Гарах хот / From: {_boardingPass.Flight.DepartureCity}\n";
            content += $"Очих хот / To: {_boardingPass.Flight.ArrivalCity}\n";
            content += $"Огноо / Date: {_boardingPass.Flight.DepartureTime:yyyy-MM-dd}\n";
            content += $"Цаг / Time: {_boardingPass.Flight.DepartureTime:HH:mm}\n\n";
            
            content += $"Зорчигчийн нэр / Passenger: {_boardingPass.Passenger.LastName} {_boardingPass.Passenger.FirstName}\n";
            content += $"Паспортын дугаар / Passport: {_boardingPass.Passenger.PassportNumber}\n";
            content += $"Суудал / Seat: {_boardingPass.Seat.SeatNumber}\n";
            content += $"Бүртгэсэн цаг / Check-in time: {_boardingPass.CheckInTime:yyyy-MM-dd HH:mm}\n\n";
            
            content += "Анхааруулга: Нислэгийн цагаас 30 минутын өмнө нисэх буудалд ирж, онгоцны гарцаар бүртгүүлнэ үү.\n";
            content += "Notice: Please be at the boarding gate 30 minutes before the flight time.\n";
            
            content += $"BoardingPass ID: {_boardingPass.Id}";
            
            return content;
        }
    }
} 