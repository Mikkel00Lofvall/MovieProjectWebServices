﻿using Microsoft.AspNetCore.Authorization;
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
            try
            {
                (bool result, string message, ICollection<ScheduleModel> schedules) = await _repo.GetAll();
                if (result)
                {
                    return Ok(schedules);
                }

                else
                {
                    return Problem("No Users");
                }
            }
            catch (Exception ex) { return Problem(ex.Message); }
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
                   
                else return Ok(objects);
            }
            
            return Ok(objects);
        }

        [HttpGet("GetMovieAndScheduleByID/{id}")]
        public async Task<IActionResult> GetMovieAndSchedule(int id)
        {
            (bool result, string message, ScheduleModel schedule, MovieModel movie, List<TicketModel> tickets) = await _repo.GetMovieAndScheduleByID(id);

            if (!result) return Problem($"Result was not Succesful | Message: {message}");

            if (schedule == null) return Problem($"Result was not Succesful | Message: No Schedule in db by that schedule id");
            if (movie == null) return Problem($"Result was not Succesful | Message: No Movie in db connected to that schedule id");

            (result, message, var GottenObject) = await _cinemaHallRepository.GetHallBySchedule(id);

            if (!result) return Problem($"Result was not Succesful | Message: {message}");

            if (GottenObject == null) return Problem($"Result was not Succesful | Message: No Cinema Hall connected to that schedule id");

            HallDTO hallDTO = new HallDTO()
            {
                id = GottenObject.id,
                Name = GottenObject.Name,
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

            DateDTO dateModel = new DateDTO()
            {
                Year = schedule.Date.Year,
                Month = schedule.Date.Month,
                Day = schedule.Date.Day,
                Hour = schedule.Date.Hour,
                Minute = schedule.Date.Minute,
                Second = schedule.Date.Second,
            };

            ScheduleDTO scheduleDTO = new ScheduleDTO()
            {
                id = schedule.id,
                date = dateModel,
                MovieID = schedule.MovieId,
                Seats = schedule.Seats,
                Tickets = tickets,
            };

            SeatPageDTO seatPageDTO = new SeatPageDTO()
            {
                Hall = hallDTO,
                Movie = movieDTO,
                Schedule = scheduleDTO
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

                else return Problem(message);
            }

            else return NoContent();
        }


        [HttpDelete("DeleteSchedule/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            (bool result, string message, var resultSchedule) = await _repo.GetWithId(id);
            if (result == false) return Problem("Movie does not exist in db");

            if (resultSchedule == null)  return Problem(message);

            (result, message) = await _repo.Delete(id);

            if (result  == false) return Problem(message);

            return Ok();
        }


    }   
}
