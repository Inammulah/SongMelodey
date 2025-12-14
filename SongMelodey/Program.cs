using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using SongMelodey.Models;
using SongMelodey.Services;

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

            // Database connection - MUST be registered BEFORE services that use DbContext
            builder.Services.AddDbContext<SongsMedleyMakerAndDjContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbConStr"));
            });

            // Register services in correct order
            builder.Services.AddSingleton<FFprobeService>();
            builder.Services.AddScoped<ITrimService, TrimService>();
            builder.Services.AddScoped<IMedleyService, MedleyService>();


            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // File Upload (200MB limit)
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 200 * 1024 * 1024; // 200MB
            });

            // CORS if needed (add if you have frontend on different port)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
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

            // Use CORS
            app.UseCors("AllowAll");

            // Redirect root → Swagger - FIXED VERSION
            app.MapGet("/", () =>
            {
                // Return a redirect response
                return Microsoft.AspNetCore.Http.Results.Redirect("/swagger");
            });

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