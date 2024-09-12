using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemaHallController : ControllerBase
    {
        private readonly IRepository<CinemaHallModel> repo;

        public CinemaHallController(IRepository<CinemaHallModel> repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetHalls")]
        public async Task<IActionResult> Get()
        {
            var cinemaHall = await repo.GetAll();

            return Ok(cinemaHall);
        }

        [HttpPost("GetHallWithId")]
        public async Task<IActionResult> Get([FromBody] CinemaHallModel hall)
        {
            (bool result, string message, var cinemaHall) = await repo.GetWithId(hall.id);
            if (result) return Ok(cinemaHall);

            else return BadRequest(message);

        }

        [HttpPost("CreateHall")]
        public async Task<IActionResult> Create([FromBody] CinemaHallModel hall)
        {
            if (hall != null)
            {
                (bool result, string message) = await repo.Create(hall);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }

        [HttpDelete("DeleteHall")]
        public async Task<IActionResult> Delete([FromBody] CinemaHallModel hall)
        {
            (bool result, string message, var cinemaHall) = await repo.GetWithId(hall.id);
            if (result)
            {
                if (cinemaHall != null)
                {
                    await repo.Delete(cinemaHall);
                    return Ok();
                }

                else return BadRequest("Movie does not exist in db");
            }

            else return BadRequest(message);

        }
    }
}
