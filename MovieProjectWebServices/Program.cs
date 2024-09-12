using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MoviesDatabase;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<IRepository<MovieModel>, Repository<MovieModel>>();
            builder.Services.AddScoped<IRepository<GenreModel>, Repository<GenreModel>>();
            builder.Services.AddScoped<IRepository<CinemaHallModel>, Repository <CinemaHallModel>>();
            builder.Services.AddDbContext<ContextDB>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}
