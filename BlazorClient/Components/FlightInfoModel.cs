using System;
using DataAccess.Models;

namespace BlazorClient.Components
{
    public class FlightInfoModel
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Status { get; set; } // Enum төлөвийг integer төлөөлөх

        // Харуулахад хэрэгтэй нэмэлт шинж чанарууд
        public string Time => DepartureTime.ToString("HH:mm");
        public string Destination => ArrivalCity;
        public string Flight => FlightNumber;
        public string Gate => $"Gate {Id}";
        public string StatusText => GetStatusText(Status);
        
        private string GetStatusText(int status) => ((FlightStatus)status) switch
        {
            FlightStatus.CheckingIn => "Бүртгэж байна",
            FlightStatus.Boarding => "Онгоцонд сууж байна",
            FlightStatus.Departed => "Ниссэн",
            FlightStatus.Delayed => "Хойшилсон",
            FlightStatus.Cancelled => "Цуцалсан",
            _ => "Тодорхойгүй"
        };
    }
}