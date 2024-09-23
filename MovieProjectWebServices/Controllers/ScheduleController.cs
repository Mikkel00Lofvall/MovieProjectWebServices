using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleRepository repo;

        public ScheduleController(ScheduleRepository repo) { this.repo = repo; }

        [HttpGet("GetSchedules")]
        public async Task<IActionResult> Get()
        {
            (bool result, string message, var schedules) = await repo.GetAll();
            if (result)
            {
                return Ok(schedules);
            }

            return BadRequest(message);
            
        }

        [HttpGet("GetSchedulesWithMovieID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool success, string message, var schedules) = await repo.GetSchedulesByMovieID(id);
            if (success) 
            {
                if (schedules.Count() > 0) return Ok(schedules);
                else return Ok("No Schedules for this movie!");
            }
            
            return BadRequest(message);
        }

        [HttpPost("CreateSchedule/{id}")]
        public async Task<IActionResult> Create(int id, [FromBody] DateModel date)
        {
            if (id != null && date != null)
            {

                (bool result, string message) = await repo.UpdateMovieWithSchedule(id, date);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }

        [HttpDelete("DeleteSchedule")]
        public async Task<IActionResult> Delete([FromBody] ScheduleModel inputSchedule)
        {
            (bool result, string message, var resultSchedule) = await repo.GetWithId(inputSchedule.id);
            if (result)
            {
                if (resultSchedule != null)
                {
                    await repo.Delete(resultSchedule);
                    return Ok();
                }

                else return BadRequest("Movie does not exist in db");
            }

            else return BadRequest(message);

        }
    }   
}
