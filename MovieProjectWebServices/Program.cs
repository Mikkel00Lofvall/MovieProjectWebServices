using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MoviesDatabase;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Models.Test;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string machineName = System.Environment.MachineName;
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000") // React frontend running on port 3000
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });

            builder.Services.AddScoped<MovieRepository>();
            builder.Services.AddScoped<ScheduleRepository>();
            builder.Services.AddScoped<IRepository<ThemeModel>, Repository<ThemeModel>>();
            builder.Services.AddScoped<IRepository<TestModel>, Repository<TestModel>>();
            builder.Services.AddScoped<IRepository<CinemaHallModel>, Repository <CinemaHallModel>>();

            string connectionString = "";

            if (machineName == "MSI") connectionString = builder.Configuration.GetConnectionString("OffSiteConnection");

            else if (machineName == "MJENSENLAPTOP") connectionString = builder.Configuration.GetConnectionString("HomeConnection");
                
            else connectionString = builder.Configuration.GetConnectionString("HomeConnection");
                

            builder.Services.AddDbContext<ContextDB>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.MigrationsAssembly("MoviesDatabase")));


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigins");

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}
