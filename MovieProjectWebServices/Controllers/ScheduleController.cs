using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.DTO;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleRepository _repo;
        private readonly CinemaHallRepository _cinemaHallRepository;

        public ScheduleController(ScheduleRepository ScheduleRepo, CinemaHallRepository CinemaRepo) 
        { 
            this._repo = ScheduleRepo;
            this._cinemaHallRepository = CinemaRepo;
        }

        [HttpGet("GetSchedules")]
        public async Task<IActionResult> Get()
        {
            (bool result, string message, var schedules) = await _repo.GetAll();
            if (result)
            {
                return Ok(schedules);
            }

            return BadRequest(message);
            
        }

        [HttpGet("GetSchedulesWithMovieID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            List<object> objects = new();
            (bool success, string message, var schedules) = await _repo.GetSchedulesByMovieID(id);
            if (success) 
            {
                if (schedules.Count() > 0)
                {
                    foreach (var schedule in schedules)
                    {
                        (success, message, var cinemaHall) = await _cinemaHallRepository.GetWithId(schedule.HallId);

                        if (success) objects.Add(new { Schedule = schedule, Hall = cinemaHall });

                    }

                    return Ok(objects);
                }
                   
                else return BadRequest("No Schedules for this movie!");
            }
            
            return BadRequest(message);
        }

        [HttpGet("GetMovieAndScheduleByID/{id}")]
        public async Task<IActionResult> GetMovieAndSchedule(int id)
        {
            /*(bool result, string message, object data) = await _repo.GetMovieAndScheduleByID(id);
            if (result)
            {
                if (data != null)
                {
                    (result, message, var cinemaHall) = await _cinemaHallRepository.GetHallBySchedule(id);
                    if (result)
                    {
                        if (cinemaHall != null) return Ok(new { Data = data, Hall = cinemaHall });

                        else return BadRequest("No Hall Connected to that schedule id");
                    } 
                    else return BadRequest(message);
                }
                else return BadRequest("No Data");
            }

            return BadRequest(message);*/


            (bool result, string message, object data) = await _repo.GetMovieAndScheduleByID(id);

            if (!result) return BadRequest($"Result was not Succesful | Message: {message}");

            if (data == null) return BadRequest($"Result was not Succesful | Message: No Movie in db by that schedule id");

            (result, message, var GottenObject) = await _cinemaHallRepository.GetHallBySchedule(id);

            if (!result) return BadRequest($"Result was not Succesful | Message: {message}");

            if (GottenObject == null) return BadRequest($"Result was not Succesful | Message: No Cinema Hall connected to that schedule id");

            return Ok(new { Data = data, Hall = GottenObject });
        }


        [HttpPost("CreateSchedule")]

        public async Task<IActionResult> Create([FromBody] SchedulesDTO scheduleInput)
        {
            if (scheduleInput != null)
            {
                (bool result, string message) = await _repo.CreateScheduleAndInsertIntoHall(scheduleInput);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();
        }

        [HttpDelete("DeleteSchedule")]
        public async Task<IActionResult> Delete([FromBody] ScheduleModel inputSchedule)
        {
            (bool result, string message, var resultSchedule) = await _repo.GetWithId(inputSchedule.id);
            if (result)
            {
                if (resultSchedule != null)
                {
                    await _repo.Delete(resultSchedule);
                    return Ok();
                }

                else return BadRequest("Movie does not exist in db");
            }

            else return BadRequest(message);

        }


    }   
}
