using BusinessLogic.Services;
using DataAccess;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace BusinessLogic
{
    /// <summary>
    /// Бизнес логикийн сервисүүдийг бүртгэх статик класс.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Бизнес логикийн сервисүүдийг бүртгэх өргөтгөл метод.
        /// </summary>
        /// <param name="services">Сервисийн цуглуулга</param>
        /// <param name="connectionString">Өгөгдлийн сангийн холболтын утга</param>
        /// <returns>Сервисийн цуглуулга</returns>
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, string connectionString)
        {
            // Хэрэв холболтын утга хоосон бол DataAccess хавтаст өгөгдлийн сан үүсгэх
            if (string.IsNullOrEmpty(connectionString))
            {
                // DataAccess.dll замыг олох
                string dataAccessDllPath = Path.GetDirectoryName(
                    Assembly.GetAssembly(typeof(AirportDbContext)).Location);
                string dbPath = Path.Combine(dataAccessDllPath, "airport.db");
                connectionString = $"Data Source={dbPath}";
            }

            // Өгөгдлийн сан бүртгэх
            services.AddDbContext<AirportDbContext>(options =>
                options.UseSqlite(connectionString));

            // Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Services
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IPassengerService, PassengerService>();
            services.AddScoped<IBoardingService, BoardingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IFlightPassengerService, FlightPassengerService>();

            return services;
        }

        /// <summary>
        /// Өгөгдлийн санг үүсгэж, анхны өгөгдлүүдийг оруулах
        /// </summary>
        public static async Task InitializeDatabaseAsync()
        {
            await DbInitializer.CreateAndInitializeDatabase();
        }
    }
} 