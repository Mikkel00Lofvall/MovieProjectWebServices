using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private Mock<ThemeRepository> _mockRepository;
        private List<ThemeModel> _mockEntityList;

        public ThemeRepoTest()
        {
            _mockRepository = new Mock<ThemeRepository>();

            // Initialize mock data for testing
            _mockEntityList = new List<ThemeModel>
            {
                new ThemeModel { id = 1, Name = "Fantasy" },
                new ThemeModel { id = 2, Name = "Horror" }
            };

            _mockRepository.Setup(repo => repo.GetAll()).ReturnsAsync(_mockEntityList);
        }

        // This Test will work, it is Intentional For Viewing purposes, in CinemaHall there is a delete that will Fail

        [Fact]
        public async Task Delete()
        {
            int ThemeID = 1;
            try
            {
                var (result, message, theme) = await _mockRepository.Object.GetWithId(ThemeID);

                if (result == false)
                {
                    throw new Exception(message);
                }

                (result, message) = await _mockRepository.Object.Delete(ThemeID);

                if (result == false)
                {
                    throw new Exception(message);
                }

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}