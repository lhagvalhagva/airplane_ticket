using BusinessLogic.Services;
using DataAccess;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            return services;
        }
    }
} 