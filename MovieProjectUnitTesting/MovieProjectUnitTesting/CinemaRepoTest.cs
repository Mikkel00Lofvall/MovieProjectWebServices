using Moq;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieProjectUnitTesting
{
    public class CinemaRepoTest
    {
        private Mock<CinemaHallRepository> _mockRepository;
        private List<CinemaHallModel> _mockEntityList;

        public CinemaRepoTest()
        {
            _mockRepository = new Mock<CinemaHallRepository>();

            // Initialize mock data for testing
            _mockEntityList = new List<CinemaHallModel>
            {
                new CinemaHallModel { 
                    id = 1, 
                    Name = "Room 1", 
                },
                new CinemaHallModel { id = 2, Name = "Room 2" }
            };

            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(_mockEntityList);
        }

        // This Test will Fail, it is Intentional For Viewing purposes, in theme there is a delete that will work

        public async Task Delete()
        {
            try
            {
                int id = 3;
                var (result, message, hall) = await _mockRepository.Object.GetWithId(id);


                if (result == false) throw new Exception(message);


                _mockRepository.Object.Delete(id);

            }

            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
