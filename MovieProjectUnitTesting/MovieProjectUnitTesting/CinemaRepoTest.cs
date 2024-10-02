using MoviesDatabase;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace MovieProjectUnitTesting
{
    public class CinemaRepoTest
    {
        private DbContextOptions<ContextDB> options;
        private ContextDB _context;

        public CinemaRepoTest()
        {
            options = new DbContextOptionsBuilder<ContextDB>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ContextDB(options);
            _context.Database.EnsureCreated();

            _context.CinemaHall.Add(new CinemaHallModel { id = 1, Name = "Room 1", SeatsOnRow = 8, RowsOfSeat = 8 });
            _context.CinemaHall.Add(new CinemaHallModel { id = 2, Name = "Room 2", SeatsOnRow = 5, RowsOfSeat = 5 });
            _context.CinemaHall.Add(new CinemaHallModel { id = 3, Name = "Room 3", SeatsOnRow = 7, RowsOfSeat = 7 });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAll()
        {
            CinemaHallRepository repo = new CinemaHallRepository(_context);

            var result = await repo.GetAll();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetWithId()
        {
            CinemaHallRepository repo = new CinemaHallRepository(_context);

            (bool result, string message, CinemaHallModel model) = await repo.GetWithId(1);

            Assert.True(result, message);
            Assert.Equal("", message);
            Assert.NotNull(model);
            Assert.Equal(1, model.id);
        }

        [Fact]
        public async Task Create()
        {
            CinemaHallRepository repo = new CinemaHallRepository(_context);

            CinemaHallModel cinemaHallModel = new CinemaHallModel()
            {
                Name = "Room 4",
                SeatsOnRow = 9,
                RowsOfSeat = 9,
            };

            (bool result, string message) = await repo.Create(cinemaHallModel);

            Assert.True(result, message);
            Assert.Equal("", message);
        }

        [Fact]
        public async Task Delete()
        {
            int id = 1;
            CinemaHallRepository repo = new CinemaHallRepository(_context);

            (bool result, string message) = await repo.Delete(id);

            Assert.True(result, message);
            Assert.Equal("", message);

            var deletedCinemaHall = await _context.CinemaHall.FindAsync(id);
            Assert.Null(deletedCinemaHall);
        }
    }
}
