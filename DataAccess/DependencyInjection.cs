using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            // Database байршлыг тохируулах (SQLite ашиглаж байна)
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=airport.db";
            
            // DbContext бүртгэх
            services.AddDbContext<AirportDbContext>(options =>
                options.UseSqlite(connectionString));
            
            // Repository бүртгэх - бүх моделүүдэд
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            
            // Тусгай моделүүдийн хувьд ч тодорхой Repository шаардлагатай бол нэмж болно:
            services.AddScoped<IRepository<Flight>, Repository<Flight>>();
            services.AddScoped<IRepository<Passenger>, Repository<Passenger>>();
            services.AddScoped<IRepository<Seat>, Repository<Seat>>();
            services.AddScoped<IRepository<FlightPassenger>, Repository<FlightPassenger>>();
            
            return services;
        }
    }
}
