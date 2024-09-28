using Microsoft.AspNetCore.Authorization;
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
            (bool result, string message, ScheduleModel schedule, MovieModel movie) = await _repo.GetMovieAndScheduleByID(id);

            if (!result) return BadRequest($"Result was not Succesful | Message: {message}");

            if (schedule == null) return BadRequest($"Result was not Succesful | Message: No Schedule in db by that schedule id");

            if (schedule == null) return BadRequest($"Result was not Succesful | Message: No Movie in db by that schedule id");

            (result, message, var GottenObject) = await _cinemaHallRepository.GetHallBySchedule(id);

            if (!result) return BadRequest($"Result was not Succesful | Message: {message}");

            if (GottenObject == null) return BadRequest($"Result was not Succesful | Message: No Cinema Hall connected to that schedule id");

            HallDTO hallDTO = new HallDTO()
            {
                id = GottenObject.id,
                Name = GottenObject.Name,
                Seats = GottenObject.Seats,
                SeatsOnRow = GottenObject.SeatsOnRow,

            };

            MovieDTO movieDTO = new MovieDTO()
            {
                Name = movie.Name,
                Description = movie.Description,
                Details = movie.Details,
                ImagesBlobs = movie.ImagesBlobs,
                FrontPageImage = movie.FrontPageImage,
                TrailerLink = movie.TrailerLink,
            };

            ScheduleDTO scheduleDTO = new ScheduleDTO()
            {
                id = schedule.id,
                date = schedule.Date,
                MovieID = schedule.MovieId,
            };

            SeatPageDTO seatPageDTO = new SeatPageDTO()
            {
                Hall = hallDTO,
                Movie = movieDTO,
                Schedule = scheduleDTO,
            };

            return Ok(seatPageDTO);
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
