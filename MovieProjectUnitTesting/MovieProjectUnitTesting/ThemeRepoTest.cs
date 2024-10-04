using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using MoviesDatabase.DTO;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;
using Xunit;
using Xunit.Sdk;

namespace MoviesDatabase.Test
{
    public class ThemeRepoTest
    {
        private DbContextOptions<ContextDB> options;
        private ContextDB _context;

        public ThemeRepoTest()
        {
            options = new DbContextOptionsBuilder<ContextDB>()
              .UseInMemoryDatabase(databaseName: "dummyDatabase")
              .Options;

            _context = new ContextDB(options);
            _context.Database.EnsureDeleted();

            _context.Themes.Add(new ThemeModel {id = 1, Name = "Horror" });
            _context.Themes.Add(new ThemeModel { id = 2, Name = "Fantasy" });
            _context.Themes.Add(new ThemeModel { id = 3, Name = "Romance", });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll()
        {
            ThemeRepository repo = new ThemeRepository(_context);

            var
                result = await repo.GetAll();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetWithId()
        {
            ThemeRepository repo = new ThemeRepository(_context);

            (bool result, string message, ThemeModel model) = await repo.GetWithId(1);

            Assert.True(result, message);
            Assert.Equal("", message);
            Assert.NotNull(model);
            Assert.Equal(1, model.id);
        }

        [Fact]
        public async Task Create()
        {
            ThemeRepository repo = new ThemeRepository(_context);

            ThemeModel cinemaHallModel = new ThemeModel()
            {
                Name = "Documentary",
            };

            (bool result, string message) = await repo.Create(cinemaHallModel);

            Assert.True(result, message);
            Assert.Equal("", message);
        }

        // Will Intentionally fail since there are no id 5 in the database
        [Fact]
        public async Task Delete()
        {
            int id = 5;
            ThemeRepository repo = new ThemeRepository(_context);

            (bool result, string message) = await repo.Delete(id);

            Assert.True(result, message);
            Assert.Equal("", message);

            var deletedCinemaHall = await _context.CinemaHall.FindAsync(id);
            Assert.Null(deletedCinemaHall);
        }
    }
}