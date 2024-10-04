using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MovieProjectWebServices.Controllers;
using MoviesDatabase;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;

using MoviesDatabase.Repos;
using System.Text;

namespace MovieProjectWebServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string machineName = System.Environment.MachineName;
            var builder = WebApplication.CreateBuilder(args);

            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000") // React frontend
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });

            // Register repositories
            builder.Services.AddScoped<HasherService>();
            builder.Services.AddScoped<MovieRepository>();
            builder.Services.AddScoped<ScheduleRepository>();
            builder.Services.AddScoped<CinemaHallRepository>();
            builder.Services.AddScoped<AdminUserRepository>();
            builder.Services.AddScoped<ThemeRepository>();
            builder.Services.AddScoped<TicketRepository>();
            builder.Services.AddScoped<IRepository<ThemeModel>, Repository<ThemeModel>>();
            builder.Services.AddScoped<IRepository<CinemaHallModel>, Repository<CinemaHallModel>>();


            // Connection string
            string connectionString = machineName switch
            {
                "MSI" => builder.Configuration.GetConnectionString("OffSiteConnection"),
                "MJENSENLAPTOP" => builder.Configuration.GetConnectionString("HomeConnection"),
                _ => builder.Configuration.GetConnectionString("HomeConnection"),
            };

            // Add DB context
            builder.Services.AddDbContext<ContextDB>(options =>
                options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("MoviesDatabase")));


            ///////// Security //////////


            builder.Services.AddIdentityApiEndpoints<IdentityUser>()
                .AddEntityFrameworkStores<ContextDB>();


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "MovieProjectCookeAuth-00594907-acbc-4cb0-bcc4-3e918f709f35";
                    options.Cookie.MaxAge = TimeSpan.FromMinutes(15);
                    options.SlidingExpiration = true;
                    options.LoginPath = "/api/login";
                    options.AccessDeniedPath = "/";
                    options.Cookie.Path = "/";
                    options.Cookie.SameSite = SameSiteMode.None;
                });


            ///////// Security //////////


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigins");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }


    }
}
