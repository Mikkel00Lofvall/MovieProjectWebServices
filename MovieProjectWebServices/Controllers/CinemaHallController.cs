using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.DTO;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemaHallController : ControllerBase
    {
        private readonly CinemaHallRepository repo;

        public CinemaHallController(CinemaHallRepository repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetHalls")]
        public async Task<IActionResult> Get()
        {
            var cinemaHall = await repo.GetAll();

            return Ok(cinemaHall);
        }

        [HttpGet("GetHallWithId/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool result, string message, var cinemaHall) = await repo.GetWithId(id);
            if (result) return Ok(cinemaHall);

            else return BadRequest(message);

        }

        char GetNextLetter(char letter)
        {
            if (letter == 'Z')
            {
                return 'A';
            }
            return (char)(letter + 1);
        }

        [HttpPost("CreateHall")]
        public async Task<IActionResult> Create([FromBody] CinemaHallDTO DTO)
        {
            if (DTO != null)
            {
                char currentLetter = 'A';
                CinemaHallModel hallModel = new CinemaHallModel();
                List<SeatModel> seatModels = new List<SeatModel>();

                for (int row = 0; row < DTO.RowAmount; row++)
                {
                    for (int seat = 0; seat < DTO.SeatsOnARow; seat++)
                    {
                        SeatModel seatModel = new SeatModel()
                        {
                            RowName = $"{currentLetter}",
                            RowNumber = seat + 1,
                            IsTaken = false
                        };

                        seatModels.Add(seatModel);
                    }

                    currentLetter = GetNextLetter(currentLetter);
                }

                hallModel.Name = DTO.Name;
                hallModel.Seats = seatModels;
                hallModel.SeatsOnRow = DTO.SeatsOnARow;
                (bool result, string message) = await repo.Create(hallModel);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }

        [HttpDelete("DeleteHall/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            (bool result, string message, var cinemaHall) = await repo.GetWithId(id);
            if (result)
            {
                if (cinemaHall != null)
                {
                    await repo.Delete(id);
                    return Ok();
                }

                else return BadRequest("Movie does not exist in db");
            }

            else return BadRequest(message);

        }
    }
}
