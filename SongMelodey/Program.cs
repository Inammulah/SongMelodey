using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SongMelodey.Models;
using SongMelodey.Services;
using SongMelodey.Services.SongMedleyAPI.Services;

namespace SongMelodey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ===============================
            // 1. Add services (DI container)
            // ===============================
            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // File Upload (200MB limit)
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // 200MB
            });

            // Register TrimService
         //   builder.Services.AddScoped<ITrimService, TrimService>();
            builder.Services.AddSingleton<FFprobeService>();

            // Configuration
            var config = builder.Configuration;

            // Database connection
            builder.Services.AddDbContext<SongsMedleyMakerAndDjContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("dbConStr"));
            });


            // ===============================
            // 2. Build the app
            // ===============================
            var app = builder.Build();


            // ===============================
            // 3. Middleware pipeline
            // ===============================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Song Melodey API V1");
                });
            }

            // Redirect root → Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();

            // Controllers
            app.MapControllers();

            // Run the app
            app.Run();
        }
    }
}
