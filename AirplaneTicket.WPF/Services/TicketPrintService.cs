using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Printing;

namespace AirplaneTicket.WPF.Services
{
    public class TicketPrintService
    {
        private TicketInfo _ticketToPrint;

        public void PrintTicket(TicketInfo ticket)
        {
            _ticketToPrint = ticket;

            // WPF Print Dialog ашиглах
            var printDialog = new PrintDialog();
            
            if (printDialog.ShowDialog() == true)
            {
                // Хэвлэх баримт бичиг үүсгэх
                var document = CreateTicketDocument();
                
                // Хэвлэх - IDocumentPaginatorSource interface ашиглах
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Онгоцны тасалбар");
            }
        }

        private FlowDocument CreateTicketDocument()
        {
            var document = new FlowDocument();
            document.PageWidth = 600;
            document.PageHeight = 800;
            document.PagePadding = new Thickness(50);
            document.FontFamily = new FontFamily("Arial");
            document.FontSize = 12;

            // Компанийн нэр
            var companyHeader = new Paragraph(new Run("АГААРЫН ТЭЭВЭР ХХК"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            document.Blocks.Add(companyHeader);

            // Тасалбарын гарчиг
            var ticketHeader = new Paragraph(new Run("ОНГОЦНЫ ТАСАЛБАР"))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            document.Blocks.Add(ticketHeader);

            // Зураас
            var separator1 = new Paragraph()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 2, 0, 0),
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(separator1);

            // Нислэгийн мэдээлэл хэсэг
            var flightSection = new Paragraph(new Run("НИСЛЭГИЙН МЭДЭЭЛЭЛ"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue,
                Margin = new Thickness(0, 0, 0, 10)
            };
            document.Blocks.Add(flightSection);

            // Нислэгийн дугаар
            var flightNumber = new Paragraph(new Run($"Нислэгийн дугаар: {_ticketToPrint.FlightNumber}"))
            {
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            document.Blocks.Add(flightNumber);

            // Маршрут
            var route = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            route.Inlines.Add(new Run($"Хөдөлгөх цэг: {_ticketToPrint.DepartureCity}"));
            route.Inlines.Add(new LineBreak());
            route.Inlines.Add(new Run($"Ирэх цэг: {_ticketToPrint.ArrivalCity}"));
            document.Blocks.Add(route);

            // Цагийн мэдээлэл
            var timeInfo = new Paragraph()
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            timeInfo.Inlines.Add(new Run($"Хөдөлгөх цаг: {_ticketToPrint.DepartureTime:dd.MM.yyyy HH:mm}"));
            timeInfo.Inlines.Add(new LineBreak());
            timeInfo.Inlines.Add(new Run($"Ирэх цаг: {_ticketToPrint.ArrivalTime:dd.MM.yyyy HH:mm}"));
            document.Blocks.Add(timeInfo);

            // Зураас
            var separator2 = new Paragraph()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(separator2);

            // Зорчигчийн мэдээлэл хэсэг
            var passengerSection = new Paragraph(new Run("ЗОРЧИГЧИЙН МЭДЭЭЛЭЛ"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue,
                Margin = new Thickness(0, 0, 0, 10)
            };
            document.Blocks.Add(passengerSection);

            // Зорчигчийн нэр
            var passengerName = new Paragraph(new Run($"Нэр: {_ticketToPrint.PassengerName}"))
            {
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };
            document.Blocks.Add(passengerName);

            // Паспортын дугаар
            var passportNumber = new Paragraph(new Run($"Паспортын дугаар: {_ticketToPrint.PassportNumber}"))
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            document.Blocks.Add(passportNumber);

            // Утасны дугаар
            var phoneNumber = new Paragraph(new Run($"Утасны дугаар: {_ticketToPrint.PhoneNumber}"))
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(phoneNumber);

            // Зураас
            var separator3 = new Paragraph()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 1, 0, 0),
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(separator3);

            // Суудлын мэдээлэл хэсэг
            var seatSection = new Paragraph(new Run("СУУДЛЫН МЭДЭЭЛЭЛ"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue,
                Margin = new Thickness(0, 0, 0, 10)
            };
            document.Blocks.Add(seatSection);

            // Суудлын дугаар (том үсгээр)
            var seatNumber = new Paragraph(new Run($"СУУДЛЫН ДУГААР: {_ticketToPrint.SeatNumber}"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                Margin = new Thickness(0, 0, 0, 10)
            };
            document.Blocks.Add(seatNumber);

            // Бүртгэлийн огноо
            var registrationDate = new Paragraph(new Run($"Бүртгэлийн огноо: {_ticketToPrint.RegistrationDate:dd.MM.yyyy HH:mm}"))
            {
                Margin = new Thickness(0, 0, 0, 20)
            };
            document.Blocks.Add(registrationDate);

            // Зураас
            var separator4 = new Paragraph()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 2, 0, 0),
                Margin = new Thickness(0, 0, 0, 15)
            };
            document.Blocks.Add(separator4);

            // Анхааруулга хэсэг
            var warningHeader = new Paragraph(new Run("АНХААРУУЛГА:"))
            {
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red,
                Margin = new Thickness(0, 0, 0, 10)
            };
            document.Blocks.Add(warningHeader);

            // Анхааруулгын жагсаалт
            var warnings = new List()
            {
                Margin = new Thickness(20, 0, 0, 15)
            };

            var warningItems = new string[]
            {
                "Нислэгээс 2 цагийн өмнө буудалд ирнэ үү",
                "Биеийн үнэмлэх авч явна уу",
                "Тасалбараа сайн хадгална уу",
                "Хөнгөн даацаа шалгана уу"
            };

            foreach (var warning in warningItems)
            {
                var listItem = new ListItem(new Paragraph(new Run(warning)));
                warnings.ListItems.Add(listItem);
            }
            document.Blocks.Add(warnings);

            // Баярлалаа
            var thankYou = new Paragraph(new Run("Танд баярлалаа!"))
            {
                FontSize = 12,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Blue,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            document.Blocks.Add(thankYou);

            return document;
        }
    }

    // Тасалбарын мэдээлэл
    public class TicketInfo
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }
} 