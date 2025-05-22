using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore; // Add this import to resolve 'WebApplication'

namespace FlightStatusHub;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container  
        builder.Services.AddSignalR();
            ;
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();

            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline  
        app.UseCors("CorsPolicy");

        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
        //}

        app.MapHub<FlightStatusHub>("/flightStatusHub");


        app.Run();
    }
}
