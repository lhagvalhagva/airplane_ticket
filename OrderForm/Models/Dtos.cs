using System;
using System.Collections.Generic;

namespace OrderForm.Models
{
    // Нислэгийн төлөв - DataAccess модельтой нийцүүлсэн
    public enum FlightStatus
    {
        CheckingIn,      // Бүртгэж байна
        Boarding,        // Онгоцонд сууж байна
        Departed,        // Ниссэн
        Delayed,         // Хойшилсон
        Cancelled        // Цуцалсан
    }

    // Нислэгийн мэдээлэл - DataAccess модельтой нийцүүлсэн
    public class FlightDto
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty; // Origin -> DepartureCity
        public string ArrivalCity { get; set; } = string.Empty; // Destination -> ArrivalCity
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public FlightStatus Status { get; set; } = FlightStatus.CheckingIn;
        
        // UI шаардлагатай нэмэлт талбар
        public string Departure => DepartureCity; // UI-д харуулахад хялбар болгох
        public string Destination => ArrivalCity; // UI-д харуулахад хялбар болгох
    }

    // Зорчигчийн мэдээлэл - DataAccess модельтой нийцүүлсэн
    public class PassengerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Nationality { get; set; } = string.Empty; // Харьяалал
        
        // UI шаардлагатай нэмэлт талбарууд
        public bool CheckedIn { get; set; } // Бүртгэгдсэн эсэх - UI шаардлагатай
        public string SeatNumber { get; set; } = string.Empty; // UI шаардлагатай
    }

    // Суудлын мэдээлэл - DataAccess модельтой нийцүүлсэн
    public class SeatDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; } = false;
        public int FlightId { get; set; }
        
        // UI шаардлагатай нэмэлт талбарууд
        public string PassengerName { get; set; } = string.Empty; // UI шаардлагатай
        public int Row { get; set; } // UI шаардлагатай - суудлын зураглалд
        public string Column { get; set; } = string.Empty; // UI шаардлагатай - суудлын зураглалд
    }

    // Онгоцны тасалбар мэдээлэл
    public class BoardingPassDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } // Seat.SeatNumber-ийн утга
        public DateTime CheckInTime { get; set; }
    }
}