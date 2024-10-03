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
            try
            {
                (bool result, string message, ICollection<CinemaHallModel> halls) = await repo.GetAll();
                if (result)
                {
                    return Ok(halls);
                }

                else
                {
                    return Problem("No Users");
                }
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("GetHallWithId/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool result, string message, var cinemaHall) = await repo.GetWithId(id);
            if (result) return Ok(cinemaHall);

            else return Problem(message);

        }


        [HttpPost("CreateHall")]
        public async Task<IActionResult> Create([FromBody] CinemaHallDTO DTO)
        {
            if (DTO != null)
            {

                CinemaHallModel hallModel = new CinemaHallModel();


                hallModel.Name = DTO.Name;
                hallModel.SeatsOnRow = DTO.SeatsOnARow;
                hallModel.RowsOfSeat = DTO.RowAmount;
                (bool result, string message) = await repo.Create(hallModel);
                if (result) return Ok();

                else return Problem(message);
            }

            else return NoContent();

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

                else return Problem("Movie does not exist in db");
            }

            else return Problem(message);

        }
    }
}
